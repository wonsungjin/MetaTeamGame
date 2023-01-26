using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviourPun
{
    [SerializeField] Node[] monsterTrans;
    [SerializeField] Node[] specialTrans;
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
        customDeck= GameMGR.Instance.Get_CustomDeck();
        if (customDeck.tier_1 != null) 
        for (int i = 0; i < customDeck.tier_1.Count; i++)
        {
                monsterNames.Add(customDeck.tier_1[i]);
        }
        if (customDeck.tier_2 != null)
            for (int i = 0; i < customDeck.tier_2.Count; i++)
        {
                monsterNames.Add(customDeck.tier_2[i]);
        }
        if (customDeck.tier_3 != null)
            for (int i = 0; i < customDeck.tier_3.Count; i++)
        {
                monsterNames.Add(customDeck.tier_3[i]);
        }
        if (customDeck.tier_4 != null)
            for (int i = 0; i < customDeck.tier_4.Count; i++)
            {
                 monsterNames.Add(customDeck.tier_4[i]);
            }
        if (customDeck.tier_5 != null)
            for (int i = 0; i < customDeck.tier_5.Count; i++)
            {
                monsterNames.Add(customDeck.tier_5[i]);
            }
        if (customDeck.tier_6 != null)
            for (int i = 0; i < customDeck.tier_6.Count; i++)
            {
                monsterNames.Add(customDeck.tier_6[i]);
            }
    }
    private void Start()
    {
        GameMGR.Instance.Init(2);
        SetMyDeckSetting();
        // 처음에 카드 생성
        if (isFirstStart == false)
        {
            GameMGR.Instance.uiManager.shopLevel = 1;

            for (int i = 0; i < createdPlace; i++)
            {
                randomNum = Random.Range(0, 8);
                Debug.Log(monsterNames[randomNum]);
                Debug.Log(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"));
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
        GameMGR.Instance.uiManager.goldCount = 10;
        for (int i = 0; i < monster.Length; i++)
        {
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
                        SummonMonster(0, customDeck.tier_1.Count);
                    }
                    // 2티어
                    else if (randomCard > 10 && randomCard < 30)
                    {
                        SummonMonster(customDeck.tier_1.Count, customDeck.tier_1.Count+ customDeck.tier_2.Count);
                    }
                    // 3티어
                    else if (randomCard > 30 && randomCard < 55)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count, customDeck.tier_1.Count + customDeck.tier_2.Count+ customDeck.tier_3.Count);
                    }
                    else if (randomCard > 55 && randomCard < 75)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count);
                    }
                    else if (randomCard > 75 && randomCard < 90)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count+customDeck.tier_5.Count);
                    }
                    else
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count + customDeck.tier_5.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count + customDeck.tier_5.Count+ customDeck.tier_5.Count);
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

                    SummonMonster(0, customDeck.tier_1.Count);
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
                        SummonMonster(0, customDeck.tier_1.Count);
                    }
                    else
                    {
                        SummonMonster(customDeck.tier_1.Count, customDeck.tier_1.Count + customDeck.tier_2.Count);
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
                        SummonMonster(0, customDeck.tier_1.Count);
                    }
                    // 2티어
                    else if (randomCard > 6 && randomCard < 9)
                    {
                        SummonMonster(customDeck.tier_1.Count, customDeck.tier_1.Count + customDeck.tier_2.Count);
                    }
                    // 3티어
                    else
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count);
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
                        SummonMonster(0, customDeck.tier_1.Count);
                    }
                    // 2티어
                    else if (randomCard > 40 && randomCard < 65)
                    {
                        SummonMonster(customDeck.tier_1.Count, customDeck.tier_1.Count + customDeck.tier_2.Count);
                    }
                    // 3티어
                    else if (randomCard > 65 && randomCard < 85)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count);
                    }
                    else
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count);
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
                        SummonMonster(0, customDeck.tier_1.Count);
                    }
                    // 2티어
                    else if (randomCard > 25 && randomCard < 45)
                    {
                        SummonMonster(customDeck.tier_1.Count, customDeck.tier_1.Count + customDeck.tier_2.Count);
                    }
                    // 3티어
                    else if (randomCard > 45 && randomCard < 70)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count);
                    }
                    else if (randomCard > 70 && randomCard < 90)
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count);
                    }
                    else
                    {
                        SummonMonster(customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count, customDeck.tier_1.Count + customDeck.tier_2.Count + customDeck.tier_3.Count + customDeck.tier_4.Count + customDeck.tier_5.Count);
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
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum]}"), monsterTrans[randomTrans].transform.position, Quaternion.identity);

        randomTrans++;
    }
    public void SpecialMonster()
    {
        int randomNum1 = Random.Range(8, 16);
        int randomNum2 = Random.Range(16, 24);
        int randomNum3 = Random.Range(24, 32);
        int randomNum4 = Random.Range(32, 40);
        int randomNum5 = Random.Range(40, 48);

        switch (GameMGR.Instance.uiManager.shopLevel)
        {
            case 1:
                GameObject[] trans = GameObject.FindGameObjectsWithTag("SpecialZone");
                if (trans == null)
                    return;
                else
                {
                    GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum1]}"), trans[0].transform.position, Quaternion.identity);
                }
                break;
            case 2:
                trans = GameObject.FindGameObjectsWithTag("SpecialZone");
                if (trans == null)
                    return;
                else
                {
                    GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum2]}"), trans[0].transform.position, Quaternion.identity);
                }
                break;
            case 3:
                trans = GameObject.FindGameObjectsWithTag("SpecialZone");
                if (trans == null)
                    return;
                else
                {
                    GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum3]}"), trans[0].transform.position, Quaternion.identity);
                }
                break;
            case 4:
                trans = GameObject.FindGameObjectsWithTag("SpecialZone");
                if (trans == null)
                    return;
                else
                {
                    GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum4]}"), trans[0].transform.position, Quaternion.identity);
                }
                break;
            case 5:
                trans = GameObject.FindGameObjectsWithTag("SpecialZone");
                if (trans == null)
                    return;
                else
                {
                    GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{monsterNames[randomNum5]}"), trans[0].transform.position, Quaternion.identity);
                }
                break;
        }
    }
}
