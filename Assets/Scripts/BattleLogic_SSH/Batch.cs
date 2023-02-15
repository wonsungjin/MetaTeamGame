using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Batch : MonoBehaviourPun
{

    public Transform[] myCardPosition = null;
    public Transform[] enemyCardPosition = null;
    List<int> CustomNumberList = new List<int>();
    bool isMinePlayerNum = true;
    [SerializeField] GameObject playerRanking;
    [SerializeField] Transform playerRankingUi;
    int tempHp = 0;
    int tempAtk = 0;
    int tempExp = 0;
    int tempLevel = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) FinalCardUi();
    }
    public void FinalCardUi()
    {
        GameMGR.Instance.uiManager.finalSceneUI.SetActive(true);
        for (int i = 0; i < CustomNumberList.Count; i++)
        {
            List<Card> cardList = null;
            GameObject unitCard = GameObject.Instantiate<GameObject>(playerRanking);
            unitCard.transform.SetParent(playerRankingUi);
            GameMGR.Instance.playerList.TryGetValue(CustomNumberList[i], out cardList);
            
            for (int j = 0; j < cardList.Count; j++)
            {
                if (cardList[j] == null)
                {
                    unitCard.transform.GetChild(8 + j).GetComponent<CardUI>().OffFrame();
                continue;
                }
                unitCard.transform.GetChild(8 + j).GetComponent<CardUI>().SetMyInfo(cardList[j].name.Replace("(Clone)", ""));
                unitCard.transform.GetChild(8 + j).GetComponent<CardUI>().OffFrame();
                unitCard.transform.GetChild(8 + j).GetComponent<CardUI>().SpriteNone();

            }
        }
    }
    public void Init()
    {
        StartCoroutine(COR_SetCustomDelay());
        GameObject temporaryPlayerObjects = GameObject.Find("PlayerPosition");
        GameObject temporaryEnemyObjects = GameObject.Find("EnemyPosition");
        playerRankingUi = GameObject.Find("playerRankingUI").transform;
        myCardPosition = temporaryPlayerObjects.transform.GetComponentsInChildren<Transform>();
        enemyCardPosition = temporaryEnemyObjects.transform.GetComponentsInChildren<Transform>();
    }
    IEnumerator COR_SetCustomDelay()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            CustomNumberList.Add((int)PhotonNetwork.PlayerList[i].CustomProperties["Number"]);
        }
    }

    [PunRPC]
    public void ClearBatch(int playerNum)
    {
        List<Card> cardList = null;
        bool listCheck = GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == true)
        {
            cardList.Clear();
        }
    }
    [PunRPC]
    public void SetBatch(int playerNum, string cardName, int hp, int attackValue, int exp, int level)
    {
        List<Card> cardList = null;
        Card card = null;
        GameObject instance = Resources.Load<GameObject>($"Prefabs/{cardName}");
        Debug.Log(cardName);
        Debug.Log(cardName);
        bool listCheck = GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        if (listCheck == false)
        {
            cardList = new List<Card>();
        }
        if (cardName != "")
        {
            card = instance.GetComponentInChildren<Card>();
            Debug.Log(instance);
            if (instance == null) Debug.Log("sjf");
            card.SetMyInfo(cardName,false);
            card.curHP = hp;
            card.curAttackValue = attackValue;
            card.curEXP = exp;
            card.level = level;
            cardList.Add(card);
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
        // ���� ��ġ ����
        // ���� �İ� ����

        GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[0], GameMGR.Instance.matching[0] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);


        // ��Ī�� ������ �������� �޾ƿ� ���� ��ġ ����

        GameMGR.Instance.batch.CreateBatch(GameMGR.Instance.matching[1], GameMGR.Instance.matching[1] == (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"]);

    }

    // ��Ʋ�� ���� ��ġ
    /// <summary>
    ///  playerNum : Photon Customproperties ������ȣ, 
    ///  cardNum : ī�� ��ġ ����, 
    ///  myCard : ���� ī�� ����
    /// </summary>
    /// <param name="CreateBatch"></param>
    public void CreateBatch(int playerNum, bool myCard = true)
    {
        List<Card> cardList = null;
        GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);

        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i] == null) continue;
            Debug.Log("cardList name" + cardList[i].name);
            //GameObject unitCard = GameObject.Instantiate<GameObject>(cardList[i].gameObject);
            Debug.Log(Resources.Load<GameObject>($"Prefabs/{cardList[i].name}"));
            GameObject unitCard = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{cardList[i].name}"),Vector3.zero,Quaternion.identity);
            unitCard.GetComponentInChildren<Card>().ChangeCard(cardList[i]);
            Debug.Log(unitCard.name);

            // player Unit ��ġ ����
            if (myCard == true)
            {
                Debug.Log(i);
                unitCard.transform.position = myCardPosition[i + 1].position;
                if (i < 3) { GameMGR.Instance.battleLogic.playerForwardUnits[i] = unitCard.gameObject; }
                else { GameMGR.Instance.battleLogic.playerBackwardUnits[i - 3] = unitCard.gameObject; }

                // add result unit
                GameMGR.Instance.uiManager.playerArrangement[i] = GameObject.Instantiate<GameObject>(cardList[i].gameObject);
            }

            // enemy Unit ��ġ ����
            else if (myCard == false)
            {
                unitCard.transform.position = enemyCardPosition[i + 1].position;
                unitCard.GetComponentInChildren<Card>().SetFlip(true);
                if (i < 3) { GameMGR.Instance.battleLogic.enemyForwardUnits[i] = unitCard.gameObject; }
                else { GameMGR.Instance.battleLogic.enemyBackwardUnits[i - 3] = unitCard.gameObject; }
            }
            else { Debug.Log("CreateBatch : myCard �� Ȯ���ʿ�"); }
        }
        if (myCard) 
        {
            GameMGR.Instance.battleLogic.InitPlayerList();
        }
        else GameMGR.Instance.battleLogic.InitEnemyList();
    }

}
