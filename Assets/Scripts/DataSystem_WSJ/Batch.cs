using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviourPun
{
    Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();
    List<Card> cardList;

    Transform[] myCardPosition = null;
    Transform[] enemyCardPosition = null;

    bool isMinePlayerNum = true;

    // 한시적 스탯 증가 구현을 위한 추가 코드 - HCU *수정됨
    int tempHp = 0;
    int tempAtk = 0;
    int tempExp = 0;
    int tempLevel = 0;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        unitPlacement();
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
    public void SetBatch(int playerNum, Card card, int tempHp, int tempAtk, int tempExp, int tempLevel)
    {
        cardList = null;
        Card instance = Resources.Load<Card>($"Prefabs/{card.name}");
        bool listCheck = playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == false)
        {
            cardList = new List<Card>();
        }
        instance.ChangeValue(CardStatus.Hp, card.curHP + tempHp);
        instance.ChangeValue(CardStatus.Attack, card.curAttackValue + tempAtk);
        instance.ChangeValue(CardStatus.Exp, card.curEXP + tempExp);
        instance.ChangeValue(CardStatus.Level, card.level + tempLevel);
        cardList.Add(instance);
    }

    public List<Card> GetBatch(int playerNum)
    {
        cardList = null;
        bool listCheck = playerList.TryGetValue(playerNum, out cardList);
        return cardList;
    }
    public void unitPlacement()
    {
        // 유닛 배치 정보
        // 선공 후공 정보
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[0], i, GameMGR.Instance.matching[0] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }

        // 매칭된 상대방의 상점에서 받아온 유닛 배치 정보
        for (int i = 0; i < 6; i++)
        {
            GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[1], i, GameMGR.Instance.matching[1] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);
        }
    }

    // 배틀씬 유닛 배치
    /// <summary>
    ///  playerNum : Photon Customproperties 고유번호, 
    ///  cardNum : 카드 배치 순서, 
    ///  myCard : 본인 카드 여부
    /// </summary>
    /// <param name="CreateBatch"></param>
    public Card CreateBatch(int playerNum, int cardNum, bool myCard = true)
    {
        List<Card> cardList = null;
        playerList.TryGetValue(playerNum, out cardList);
        Card unitCard = GameObject.Instantiate<Card>(cardList[cardNum]);

        // player Unit 위치 설정
        if (myCard == true)
        {
            unitCard.transform.position = myCardPosition[cardNum + 1].position;
        }

        // enemy Unit 위치 설정
        else if (myCard == false)
        {
            unitCard.transform.position = enemyCardPosition[cardNum + 1].position;
        }

        else
        {
            Debug.Log("CreateBatch : myCard 값 확인필요");
        }
        return unitCard;
    }


}
