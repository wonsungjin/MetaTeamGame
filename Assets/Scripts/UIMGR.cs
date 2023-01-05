using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMGR : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPannel;
    [SerializeField] private GameObject myDeckPannel;
    [SerializeField] private GameObject packChoicePannel;
    [SerializeField] private GameObject customPannel;
    [SerializeField] private GameObject myPackList;
    [SerializeField] private MyDeck packButton;
    [SerializeField] private GameObject packAddButton;
    [SerializeField] private GameObject ShopCards;
    [SerializeField] private GameObject[] tier;
    public void OnCilck_Join_PackChoice()
    {
        lobbyPannel.SetActive(false);
        packChoicePannel.SetActive(true);
    }
    public void CreateMyPackButton(CustomDeck customDeck)
    {
        MyDeck obj = GameObject.Instantiate<MyDeck>(packButton);
        obj.SetMyDeck(customDeck);
        obj.transform.SetParent(myPackList.transform);
        obj.transform.localScale = Vector3.one;

    }
    public void SetParentPackAddButton()
    {
        packAddButton.transform.SetParent(null);
        packAddButton.transform.SetParent(myPackList.transform);
        packAddButton.transform.localScale = Vector3.one;
    }
    public void OnClick_Join_Custom()
    {
        GameMGR.Instance.customDeckShop.OnClick_Join_CustomDeckShop();
        packChoicePannel.SetActive(false);
        customPannel.SetActive(true);
        ShopCards.SetActive(true);
    }
    public void OnClick_Join_MyDeckInfo()
    {
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(true);
    }
    public void OnClick_Move_Home()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        packChoicePannel.SetActive(false);
        lobbyPannel.SetActive(true);
    }
    public void OnClick_Move_Back()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        packChoicePannel.SetActive(true);
        if (myDeckPannel.activeSelf)
        {
            myDeckPannel.SetActive(false);
        }
        customPannel.SetActive(false);
    }
    List<GameObject> list = new List<GameObject>();
    public void MyDeckTier(int tierNum,CustomDeck customDeck, CardUI cardUI,int num)
    {
        cardUI.transform.SetParent(tier[tierNum].transform);
        cardUI.transform.localScale = Vector3.one;
        if(tierNum==0) cardUI.SetMyInfo(customDeck.tier_1[num]);
        else if(tierNum==1) cardUI.SetMyInfo(customDeck.tier_2[num]);
        else if (tierNum==2) cardUI.SetMyInfo(customDeck.tier_3[num]);
        else if (tierNum==3) cardUI.SetMyInfo(customDeck.tier_4[num]);
        else if (tierNum==4) cardUI.SetMyInfo(customDeck.tier_5[num]);
        else if (tierNum==5) cardUI.SetMyInfo(customDeck.tier_6[num]);
    }
}
