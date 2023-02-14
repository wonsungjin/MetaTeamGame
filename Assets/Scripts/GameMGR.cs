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

    private void Awake()
    {
        WaitForSeconds ww = new WaitForSeconds(1f);
        Init(1);
    }
    private void Start()
    {
        metaTrendAPI.GetUserProfile();
        metaTrendAPI.GetSessionID();
        StartCoroutine(COR_GetCoin());
        DontDestroyOnLoad(Instance);

    }
    CustomDeck lookCustomDeck;//
    CustomDeck myCustomDeck;//
    public void Save_MyCustomDeck(CustomDeck customDeck)
    {
        lookCustomDeck = customDeck;
        myCustomDeck = lookCustomDeck;
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
    IEnumerator COR_GetCoin()
    {
        //   metaTrendAPI.GetCoin(100);
        yield return null;
        //yield return new WaitUntil(() => stayAPI[0]);
        //yield return new WaitUntil(() => stayAPI[1]);
        dataBase.Login();
        Debug.Log("???"+metaTrendAPI.GetZera());
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
        else if (num==2)
        {
            spawner = FindObjectOfType<Spawner>();
            uiManager = FindObjectOfType<UIManager>();
            batch = FindObjectOfType<Batch>();
            batch.Init();
            uiManager.Init_Scene2();
            spawner.gameObject.GetPhotonView().RPC("StartSetting", RpcTarget.MasterClient);
            battleLogic = FindObjectOfType<BattleLogic>();
            timerSound = FindObjectOfType<TimerSound>();

            audioMGR.StoreSceneBGM(true);

            // battle Scene
            uiManager.BattleUIInit();

            // result Scene
            uiManager.ResultSceneInit();
            uiManager.PlayerBattleWin(false);
            uiManager.PlayerBattleLose(false);
        }

        // BattleScene
        else if (num == 3)
        {
            uiManager.OnBattleUI();

            uiManager.ResultUnitPosition();
            audioMGR.StoreSceneBGM(false);
            audioMGR.BattleSceneBGM(true);
        }

        // RoundScene
        else if (num == 4)
        {
            // battle Scene
            uiManager.BattleUIInit();
            audioMGR.BattleSceneBGM(false);
            uiManager.OnResultUI();
        }

        // RoundScene
        else if (num == 5)
        {
            GameMGR.Instance.spawner.ResetStore();

            // result Scene
            uiManager.PlayerBattleWin(false);
            uiManager.PlayerBattleLose(false);
        }
    }
}
