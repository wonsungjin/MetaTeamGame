using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameMGR : Singleton<GameMGR>
{
    public UIMGR uIMGR;
    public MetaTrendAPI metaTrendAPI;
    public DataBase dataBase;
    public CardCreate cardCreate;
    public CardList cardList;
    public CustomDeckShop customDeckShop;
    public ObjectPool objectPool;
    public ShopCards shopCards;
    private void Start()
    {
        GetComponentAgain();
        metaTrendAPI.GetUserProfile();
        metaTrendAPI.GetSessionID();
        StartCoroutine(COR_GetCoin());
        DontDestroyOnLoad(gameObject);
    }
    CustomDeck myCustomDeck;
    public void Save_MyCustomDeck(CustomDeck customDeck)
    {
        myCustomDeck = customDeck;
        Debug.Log("µ¶º±≈√");
    }


    IEnumerator COR_GetCoin()
    {
        yield return new WaitForSeconds(0.5f);
        metaTrendAPI.GetCoin(100);

        yield return new WaitForSeconds(1f);
        dataBase.Login();
        Debug.Log(metaTrendAPI.GetZera());
    }
    private void GetComponentAgain()
    {
        uIMGR = FindObjectOfType<UIMGR>();
        metaTrendAPI = GetComponent<MetaTrendAPI>();
        cardCreate = GetComponent<CardCreate>();
        shopCards = GetComponent<ShopCards>();
        customDeckShop = GetComponent<CustomDeckShop>();
        dataBase = GetComponent<DataBase>();
        objectPool = GetComponent<ObjectPool>();
        cardList = Resources.Load<CardList>("CardList");
    }
}
