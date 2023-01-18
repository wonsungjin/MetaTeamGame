using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviourPun
{
    [SerializeField] Node[] monsterTrans;
    [Header("몬스터 프리팹")]
    public List<string> monsterNames = new List<string>();


    [Header("상점 위치")]
    public int createdPlace = 3;
    public bool isFreeze = false;
    public int randomTrans = 0;
    CustomDeck customDeck = new CustomDeck();
    int randomNum;
    bool isFirstStart = false;

    public GameObject[] cardBatch = new GameObject[6];

    public void SetMyDeckSetting()
    {
        customDeck = new CustomDeck();
        customDeck.tier_1.Add("Darkswarm");
        customDeck.tier_1.Add("slavetrader");
        customDeck.tier_1.Add("littlegolem");
        customDeck.tier_1.Add("KaboomG");
        customDeck.tier_1.Add("richrich");
        customDeck.tier_1.Add("slavetrader");
        customDeck.tier_1.Add("Salamander");
        customDeck.tier_1.Add("orcslave");

        customDeck.tier_2.Add("scarpgolem");
        customDeck.tier_2.Add("ironingot");
        customDeck.tier_2.Add("Penetratermaster");
        customDeck.tier_2.Add("fanatic");
        customDeck.tier_2.Add("dirtyrat");
        customDeck.tier_2.Add("Felbeast");
        customDeck.tier_2.Add("gladiator");
        customDeck.tier_2.Add("orcmage");

        customDeck.tier_3.Add("reflectgolem");
        customDeck.tier_3.Add("Shylock");
        customDeck.tier_3.Add("Stonegolem");
        customDeck.tier_3.Add("pigeonter");
        customDeck.tier_3.Add("lizard");
        customDeck.tier_3.Add("angryrhinoceros");
        customDeck.tier_3.Add("orcbountyhunter");
        customDeck.tier_3.Add("orcjudge");

        customDeck.tier_4.Add("firegolem");
        customDeck.tier_4.Add("crystalgolem");
        customDeck.tier_4.Add("usurer");
        customDeck.tier_4.Add("frostghost");
        customDeck.tier_4.Add("DreadfulEtty");
        customDeck.tier_4.Add("swarmofrats");
        customDeck.tier_4.Add("Kodovist");
        customDeck.tier_4.Add("BubblingBerserker");

        for (int i = 0; i < customDeck.tier_1.Count; i++)
        {
            monsterNames.Add(customDeck.tier_1[i]);
        }
        for (int i = 0; i < customDeck.tier_2.Count; i++)
        {
            monsterNames.Add(customDeck.tier_2[i]);
        }
        for (int i = 0; i < customDeck.tier_3.Count; i++)
        {
            monsterNames.Add(customDeck.tier_3[i]);
        }
        for (int i = 0; i < customDeck.tier_4.Count; i++)
        {
            monsterNames.Add(customDeck.tier_4[i]);
        }
    }
    private void Start()
    {
        SetMyDeckSetting();
        // 처음에 카드 생성
        if (isFirstStart == false)
        {
            GameMGR.Instance.uiManager.shopLevel = 1;

            for (int i = 0; i < createdPlace; i++)
            {
                randomNum = Random.Range(0, 8);
                Debug.Log(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"));
                //GameObject mon = Instantiate(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"), monsterTrans[randomTrans].transform.position, Quaternion.identity);
                GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"), monsterTrans[randomTrans].transform.position, Quaternion.identity);

                randomTrans++;

                isFirstStart = true;

                GameMGR.Instance.uiManager.shopMoney = 7;
            }
            randomTrans = 0;
        }
    }

    private void Update()
    {
        Reset_NotMoney();
    }

    // 레디 버튼 누르면 이루어짐 몬스터 삭제 , 시간 초기화 , 머니 초기화
    List<Card> cardList = new List<Card>();
    public void OnCLick_ReadyButton()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            GameMGR.Instance.batch.gameObject.GetPhotonView().RPC("SetBatch", RpcTarget.MasterClient, (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"], cardList[i]);
        }
        GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
        for (int i = 0; i < monster.Length; i++)
        {
            GameMGR.Instance.uiManager.goldCount = 10;
            //Destroy(monster[i]);
            GameMGR.Instance.objectPool.DestroyPrefab(monster[i]);

        }
        ChooseRandomCard();
        if (GameMGR.Instance.uiManager.shopLevel < 6 && GameMGR.Instance.uiManager.shopMoney > 0)
            GameMGR.Instance.uiManager.shopMoney--;
        GameMGR.Instance.uiManager.timer = 60f;
        Reset_NotMoney();
    }

    public void OnClick_ShopLevelUp()
    {
        if (GameMGR.Instance.uiManager.shopMoney <= GameMGR.Instance.uiManager.goldCount)
        {
            GameMGR.Instance.uiManager.shopLevel++;
            GameMGR.Instance.uiManager.goldCount -= GameMGR.Instance.uiManager.shopMoney;

            // 함수 호출 레벨 업 후 돈?
            ShopLevelUp();
        }
    }

    // 용병고용소의 레벨 업 할때마다 머니 
    void ShopLevelUp()
    {
        switch (GameMGR.Instance.uiManager.shopLevel)
        {
            case 2:
                GameMGR.Instance.uiManager.shopMoney = 8;
                break;
            case 3:
                GameMGR.Instance.uiManager.shopMoney = 9;
                break;
            case 4:
                GameMGR.Instance.uiManager.shopMoney = 10;
                break;
            case 5:
                GameMGR.Instance.uiManager.shopMoney = 11;
                break;
        }
    }

    // 리롤
    public void OnClick_Reset_Monster()
    {
        if (GameMGR.Instance.uiManager.goldCount > 0)
        {
            GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");
            GameMGR.Instance.uiManager.goldCount--;

            for (int i = 0; i < monster.Length; i++)
            {
                //Destroy(monster[i]);
                GameMGR.Instance.objectPool.DestroyPrefab(monster[i]);
            }
            ChooseRandomCard();
        }
    }

    // 돈이 없을때 버튼들 끄는 함수
    void Reset_NotMoney()
    {
        if (GameMGR.Instance.uiManager.goldCount <= 0)
        {
            GameMGR.Instance.uiManager.reFreshButton.interactable = false;
        }
        else
            GameMGR.Instance.uiManager.reFreshButton.interactable = true;

        if (GameMGR.Instance.uiManager.shopLevel > 5)
        {
            GameMGR.Instance.uiManager.levelUpButton.interactable = false;
            GameMGR.Instance.uiManager.shopLevelTXT.enabled = false;
        }
        else
        {
            if (GameMGR.Instance.uiManager.goldCount < GameMGR.Instance.uiManager.shopMoney)
            {
                GameMGR.Instance.uiManager.levelUpButton.interactable = false;
            }
            else
                GameMGR.Instance.uiManager.levelUpButton.interactable = true;
        }
    }

    // 랜덤으로 카드 선택 함수
    // 스테이지 마다 퍼센트율을 정해 카드들 소환
    void ChooseRandomCard()
    {
        if (GameMGR.Instance.uiManager.shopLevel > 5)
        {
            for (int i = 0; i < 6; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                if (monsterTrans[i].isNotSpawn == false)
                {
                    randomCard = Random.Range(0, 100);
                    // 1티어
                    if (randomCard < 10)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 10 && randomCard < 30)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 30 && randomCard < 55)
                    {
                        SummonMonster(16, 24);
                    }
                    else if (randomCard > 55 && randomCard < 75)
                    {
                        SummonMonster(24, 32);
                    }
                    else if (randomCard > 75 && randomCard < 90)
                    {
                        SummonMonster(32, 40);
                    }
                    else
                    {
                        SummonMonster(40, 48);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
        }

        if (GameMGR.Instance.uiManager.shopLevel == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (monsterTrans[i].isNotSpawn == false)
                {

                    SummonMonster(0, 8);
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }

        if (GameMGR.Instance.uiManager.shopLevel == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 10);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    if (randomCard < 7)
                    {
                        SummonMonster(0, 8);
                    }
                    else
                    {
                        SummonMonster(8, 16);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (GameMGR.Instance.uiManager.shopLevel == 3)
        {
            for (int i = 0; i < 4; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 10);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 6)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 6 && randomCard < 9)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else
                    {
                        SummonMonster(16, 24);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (GameMGR.Instance.uiManager.shopLevel == 4)
        {
            for (int i = 0; i < 5; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 100);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 40)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 40 && randomCard < 65)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 65 && randomCard < 85)
                    {
                        SummonMonster(16, 24);
                    }
                    else
                    {
                        SummonMonster(24, 32);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
            createdPlace++;
        }
        if (GameMGR.Instance.uiManager.shopLevel == 5)
        {

            for (int i = 0; i < 6; i++)
            {
                // 랜덤 퍼센트율
                int randomCard;
                randomCard = Random.Range(0, 100);
                if (monsterTrans[i].isNotSpawn == false)
                {
                    // 1티어
                    if (randomCard < 25)
                    {
                        SummonMonster(0, 8);
                    }
                    // 2티어
                    else if (randomCard > 25 && randomCard < 45)
                    {
                        SummonMonster(8, 16);
                    }
                    // 3티어
                    else if (randomCard > 45 && randomCard < 70)
                    {
                        SummonMonster(16, 24);
                    }
                    else if (randomCard > 70 && randomCard < 90)
                    {
                        SummonMonster(24, 32);
                    }
                    else
                    {
                        SummonMonster(32, 40);
                    }
                }
                else
                {
                    randomTrans++;
                }
            }
            randomTrans = 0;
        }
    }

    // 용병들 Instantiate 하는 함수
    void SummonMonster(int a, int b)
    {
        randomNum = Random.Range(a, b);

        //GameObject mon = Instantiate(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"), monsterTrans[randomTrans].transform.position, Quaternion.identity);
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"), monsterTrans[randomTrans].transform.position, Quaternion.identity);

        randomTrans++;
    }
}
