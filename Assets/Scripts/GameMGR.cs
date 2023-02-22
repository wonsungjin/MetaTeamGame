using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public partial class GameMGR : Singleton<GameMGR>
{
    public MetaTrendAPI metaTrendAPI;
    public DataBase dataBase;
    public ObjectPool objectPool;
    public AudioMGR audioMGR;

    public CustomDeckShop customDeckShop;
    public ShopCards shopCards;

    public UIManager uiManager;
    public Spawner spawner;
    public PhotonLauncher photonLauncher;
    public BattleLogic battleLogic;

    public Batch batch;
    public TimerSound timerSound;

    public ChainBroken chainBroken;
    public NodeCollider nodeCollider;
    public BattleZone battleZone;
    public SellInCollider sellInCollider;
    private void Awake()
    {
        WaitForSeconds ww = new WaitForSeconds(1f);
        Init(1);
        randomValue = new int[200];
    }
    private void Start()
    {
        metaTrendAPI.GetUserProfile();
        metaTrendAPI.GetSessionID();
        metaTrendAPI.GetDummyPool();
        StartCoroutine(COR_GetCoin());
        DontDestroyOnLoad(Instance);

    }

    public CustomDeck myCustomDeck;//
    public void Save_MyCustomDeck(CustomDeck customDeck)
    {
        myCustomDeck = customDeck;
    }
    public CustomDeck Get_CustomDeck()
    {
        return myCustomDeck;
    }
    public void OnClick_Move_Matching()
    {
        if (myCustomDeck != null)
        {
            photonLauncher.OnClick_Join_Room();
            // SceneManager.LoadScene("StoreScene");
        }
        else Debug.Log("덱선택");
    }

    public bool[] stayAPI = new bool[2];
    IEnumerator COR_Delay()
    {
        yield return new WaitForSeconds(2f);
        if (stayAPI[0] == false && stayAPI[1] == false) uiManager.loginSystemUI.SetActive(true);
        while (stayAPI[0] == false && stayAPI[1] == false)
        {
            yield return new WaitForSeconds(2f);
            metaTrendAPI.GetUserProfile();
            metaTrendAPI.GetSessionID();
        }
    }
    IEnumerator COR_GetCoin()
    {
        //   metaTrendAPI.GetCoin(100);
        //yield return null;
        StartCoroutine(COR_Delay());
        yield return new WaitUntil(() => stayAPI[0]);
        yield return new WaitUntil(() => stayAPI[1]);
        yield return new WaitUntil(() => GameMGR.Instance.metaTrendAPI.res_UserProfile.userProfile.public_address != null);
        uiManager.Faid(uiManager.loginSystemUI, faidType.Out, 0.03f);
        dataBase.Login();
        PhotonNetwork.LocalPlayer.NickName = GameMGR.Instance.dataBase.userName;
        Debug.Log("???" + metaTrendAPI.GetZera());
        yield break;
    }
    public void Init(int num)
    {
        if (num == 1)
        {
            audioMGR = GetComponent<AudioMGR>();
            uiManager = FindObjectOfType<UIManager>();
            metaTrendAPI = GetComponent<MetaTrendAPI>();
            shopCards = GetComponent<ShopCards>();
            customDeckShop = GetComponent<CustomDeckShop>();
            dataBase = GetComponent<DataBase>();
            objectPool = GetComponent<ObjectPool>();
            photonLauncher = FindObjectOfType<PhotonLauncher>();
            uiManager.Init_Scene1();
            shopCards.Init();
        }
        else if (num == 2)
        {
            spawner = FindObjectOfType<Spawner>();
            uiManager = FindObjectOfType<UIManager>();
            batch = FindObjectOfType<Batch>();
            batch.Init();
            uiManager.Init_Scene2();
            spawner.gameObject.GetPhotonView().RPC("StartSetting", RpcTarget.MasterClient);
            battleLogic = FindObjectOfType<BattleLogic>();
            timerSound = FindObjectOfType<TimerSound>();
            chainBroken = FindObjectOfType<ChainBroken>();
            nodeCollider = FindObjectOfType<NodeCollider>();
            sellInCollider = FindObjectOfType<SellInCollider>();
            battleZone = FindObjectOfType<BattleZone>();
            audioMGR.StoreSceneBGM(true);

            // lobby timerInit
            uiManager.isLobby = false;

            // battle Scene
            uiManager.BattleUIInit();

            // result Scene
            uiManager.ResultSceneInit();
            uiManager.PlayerBattleWin(false);
            uiManager.PlayerBattleLose(false);
            uiManager.PlayerBattleDraw(false);
        }

        // BattleScene
        else if (num == 3)
        {
            GameMGR.Instance.isBattleNow = true;

            uiManager.OnBattleUI();
            uiManager.curRound++;

            uiManager.ResultUnitPosition();
            uiManager.battleSceneUI.SetActive(true);
            audioMGR.StoreSceneBGM(false);
            audioMGR.BattleSceneBGM(true);

        }

        // RoundScene
        else if (num == 4)
        {
            // battle Scene
            // uiManager.BattleUIInit();
            audioMGR.BattleSceneBGM(false);
            uiManager.battleSceneUI.SetActive(false);
            uiManager.OnResultUI();
        }

        // RoundScene
        else if (num == 5)
        {
            GameMGR.Instance.isBattleNow = false;
            GameMGR.Instance.spawner.ResetStore();

            GameMGR.Instance.Event_TurnStart();

            // result Scene
            uiManager.PlayerBattleWin(false);
            uiManager.PlayerBattleLose(false);
            uiManager.PlayerBattleDraw(false);
        }
    }
}
