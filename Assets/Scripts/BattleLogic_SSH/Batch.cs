using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Batch : MonoBehaviourPun
{

    Transform[] myCardPosition = null;
    Transform[] enemyCardPosition = null;

    bool isMinePlayerNum = true;

    // 한시적 스탯 증가 구현을 위한 추가 코드 - HCU *수정됨
    int tempHp = 0;
    int tempAtk = 0;
    int tempExp = 0;
    int tempLevel = 0;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        GameObject temporaryPlayerObjects = GameObject.Find("PlayerPosition");
        GameObject temporaryEnemyObjects = GameObject.Find("EnemyPosition");
        myCardPosition = temporaryPlayerObjects.transform.GetComponentsInChildren<Transform>();
        enemyCardPosition = temporaryEnemyObjects.transform.GetComponentsInChildren<Transform>();
    }

    // 상점의 배치 정보를 전달 받음 *수정됨
    [PunRPC]
    public void SetBatch(int playerNum,string cardName, int hp,int attackValue,int exp,int level)
    {
        List<Card> cardList = null;
        Card instance = Resources.Load<Card>($"Prefabs/{cardName}");
        Debug.Log(cardName);
        Debug.Log(cardName);
        bool listCheck = GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == false)
        {
            cardList = new List<Card>();
        }
        if (cardName != "")
        {
            instance.SetMyInfo(cardName);
            instance.ChangeValue(CardStatus.Hp, hp);
            instance.ChangeValue(CardStatus.Attack, attackValue);
            instance.ChangeValue(CardStatus.Exp, exp);
            instance.ChangeValue(CardStatus.Level, level);
            cardList.Add(instance);
        }
        else cardList.Add(null);
        

        GameMGR.Instance.playerList.TryAdd(playerNum, cardList);
    }

    public List<Card> GetBatch(int playerNum)
    {
        List<Card> cardList = null;
        bool listCheck = GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        return cardList;
    }
    public void UnitPlacement()
    {
        // 유닛 배치 정보
        // 선공 후공 정보

            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[0], GameMGR.Instance.matching[0] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
  

        // 매칭된 상대방의 상점에서 받아온 유닛 배치 정보

            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[1], GameMGR.Instance.matching[1] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        
    }

    // 배틀씬 유닛 배치
    /// <summary>
    ///  playerNum : Photon Customproperties 고유번호, 
    ///  cardNum : 카드 배치 순서, 
    ///  myCard : 본인 카드 여부
    /// </summary>
    /// <param name="CreateBatch"></param>
    public void CreateBatch(int playerNum,  bool myCard = true)
    {
        List<Card> cardList = null;
        GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        for (int i = 0; i < cardList.Count; i++)
        { 
            if(cardList[i] == null) continue;
            Debug.Log("생성" + cardList[i].name);
            Card unitCard = GameObject.Instantiate<Card>(cardList[i]);

            // player Unit 위치 설정
            if (myCard == true)
            {
                unitCard.transform.position = myCardPosition[i + 1].position;
                if(i<3) GameMGR.Instance.battleLogic.playerForwardUnits.Add(unitCard.gameObject);
                else GameMGR.Instance.battleLogic.playerBackwardUnits.Add(unitCard.gameObject);
            }

            // enemy Unit 위치 설정
            else if (myCard == false)
            {
                unitCard.transform.position = enemyCardPosition[i + 1].position;
                unitCard.SetFlip(true);
                if (i < 3) GameMGR.Instance.battleLogic.enemyForwardUnits.Add(unitCard.gameObject);
                else GameMGR.Instance.battleLogic.enemyBackwardUnits.Add(unitCard.gameObject);                
            }
        else
        {
            Debug.Log("CreateBatch : myCard 값 확인필요");
        }
        }
    }



}
