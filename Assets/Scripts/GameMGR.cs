using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMGR : Singleton<GameMGR>
{
    public MetaTrendAPI metaTrendAPI;
    public DataBase dataBase;
    public ObjectPool objectPool;

    public CardCreate cardCreate;
    public CustomDeckShop customDeckShop;
    public ShopCards shopCards;

    public UIManager uiManager;
    public Spawner spawner;

    public Batch batch;



    private void Start()
    {
        Init(1);
        //metaTrendAPI.GetUserProfile();
        //metaTrendAPI.GetSessionID();
        //StartCoroutine(COR_GetCoin());
        dataBase.Login();
        DontDestroyOnLoad(gameObject);

    }
    CustomDeck lookCustomDeck;//�� �� ����
    CustomDeck myCustomDeck;//�� ������ �����ҽ� Ȯ��;
    public void Save_MyCustomDeck(CustomDeck customDeck)
    {
        lookCustomDeck = customDeck;
    }
    public void OnClick_Save_MyCustomDeck()
    {
        myCustomDeck = lookCustomDeck;
        uiManager.OnClick_Move_Home();

        Debug.Log(myCustomDeck.tier_1[0]);
    }
    public CustomDeck Get_CustomDeck()
    {
        return myCustomDeck;
    }
    public void OnClick_Move_Matching()
    {
        if (myCustomDeck != null)
            SceneManager.LoadScene("StoreScene");
        else Debug.Log("����");
    }


    IEnumerator COR_GetCoin()
    {
        yield return new WaitForSeconds(0.5f);
        metaTrendAPI.GetCoin(100);

        yield return new WaitForSeconds(1f);
        dataBase.Login();
        Debug.Log(metaTrendAPI.GetZera());
    }
    public void Init(int num)
    {
        if (num == 1)
        {
            uiManager = FindObjectOfType<UIManager>();
            metaTrendAPI = GetComponent<MetaTrendAPI>();
            cardCreate = GetComponent<CardCreate>();
            shopCards = GetComponent<ShopCards>();
            customDeckShop = GetComponent<CustomDeckShop>();
            dataBase = GetComponent<DataBase>();
            objectPool = GetComponent<ObjectPool>();
            uiManager.Init_Scene1();
        }
        else if (num==2)
        {
            spawner = FindObjectOfType<Spawner>();
            batch = FindObjectOfType<Batch>();
            uiManager = FindObjectOfType<UIManager>();
            uiManager.Init_Scene2();
        }
        else if(num==3)
        {

        }
    }
}
