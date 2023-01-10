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
    [SerializeField] public GameObject[] tier;
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
        GameMGR.Instance.uIMGR.SetParentPackAddButton();

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
    }
    public void OnClick_Join_MyDeckInfo()
    {
        myDecks = FindObjectsOfType<MyDeck>();
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(true);
    }
    public void OnClick_Move_Home()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        packChoicePannel.SetActive(false);
        lobbyPannel.SetActive(true);
    }
    public MyDeck[] myDecks;
    public void OnClick_Move_Back()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        if (myDeckPannel.activeSelf)
        {
            for (int i = 0; i < myDecks.Length; i++) myDecks[i].DelateMyDeckList();
            myDeckPannel.SetActive(false);
        }
        packChoicePannel.SetActive(true);
        if (customPannel.activeSelf)
        {
            GameMGR.Instance.customDeckShop.ClearCustomDeckList();
            customPannel.SetActive(false);
        }
    }
}
