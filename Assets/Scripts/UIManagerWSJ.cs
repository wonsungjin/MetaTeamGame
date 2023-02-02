using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    [Header("Pannel")]
    [SerializeField] private GameObject lobbyPannel;
    [SerializeField] private GameObject myDeckPannel;
    [SerializeField] private GameObject packChoicePannel;
    [SerializeField] private GameObject customPannel;
    [SerializeField] public GameObject cardPannel;
    [Header("PackList")]
    [SerializeField] private GameObject myPackList;
    [SerializeField] private MyDeck packButton;
    [SerializeField] private GameObject packAddButton;
    [Header("CardInfo")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI attackValue;
    [SerializeField] private TextMeshProUGUI hpValue;
    [SerializeField] private TextMeshProUGUI atkHpValue;
    [SerializeField] private TextMeshProUGUI[] skillExplantion;

    [Header("Tier")]
    [SerializeField] public Transform[] tier;
    public void Init_Scene1()
    {
        lobbyPannel = GameObject.Find("LobbyPannel");
        myDeckPannel = GameObject.Find("MyDeckPannel");
        customPannel = GameObject.Find("CustomPannel");
        cardPannel = GameObject.Find("CardPannel");
        packAddButton = GameObject.Find("PackAddButton");
        packChoicePannel = GameObject.Find("PackChoicePannel");
        myPackList = GameObject.Find("PackList");
        cardImage= GameObject.Find("CardImage").GetComponent<Image>();
        cardName = GameObject.Find("UNITNAME").GetComponent<TextMeshProUGUI>();
        attackValue = GameObject.Find("UNITATKValue").GetComponent<TextMeshProUGUI>();
        hpValue = GameObject.Find("UNITHPValue").GetComponent<TextMeshProUGUI>();
        atkHpValue = GameObject.Find("UNITATKHP").GetComponent<TextMeshProUGUI>();
        skillExplantion = new TextMeshProUGUI[3];
        tier = new Transform[6];
        skillExplantion[0] = GameObject.Find("Skillexplanation1").GetComponent<TextMeshProUGUI>();
        skillExplantion[1] = GameObject.Find("Skillexplanation2").GetComponent<TextMeshProUGUI>();
        skillExplantion[2] = GameObject.Find("Skillexplanation3").GetComponent<TextMeshProUGUI>();
        tier[0] = GameObject.Find("Tier1Content").transform;
        tier[1] = GameObject.Find("Tier2Content").transform;
        tier[2] = GameObject.Find("Tier3Content").transform;
        tier[3] = GameObject.Find("Tier4Content").transform;
        tier[4] = GameObject.Find("Tier5Content").transform;
        tier[5] = GameObject.Find("Tier6Content").transform;
        customPannel.SetActive(false);
        packChoicePannel.SetActive(false);
        cardPannel.SetActive(false);
        myDeckPannel.SetActive(false);
    }
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
        GameMGR.Instance.uiManager.SetParentPackAddButton();

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
    public void OnClick_Join_MyDeckInfo(CustomDeck customDeck)
    {
        GameMGR.Instance.Save_MyCustomDeck(customDeck);
        myDecks = FindObjectsOfType<MyDeck>();
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(true);
    }
    public void OnClick_Move_Home()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        packChoicePannel.SetActive(false);
        myDeckPannel.SetActive(false);
        lobbyPannel.SetActive(true);
        cardPannel.SetActive(false);
    }
    public MyDeck[] myDecks;
    public void OnClick_Move_Back()
    {
        GameMGR.Instance.customDeckShop.OnClick_Exit_CustomDeckShop();
        cardPannel.SetActive(false);
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
    public void OnPointerEnter_CardInfo(CardInfo cardInfo)
    {
        cardPannel.SetActive(true);
        atkHpValue.text = $"{cardInfo.attackValue}/{cardInfo.hp}";
        attackValue.text = $"{cardInfo.attackValue}";
        hpValue.text = $"{cardInfo.hp}";
        cardName.text = $"{cardInfo.objName}";
        cardImage.sprite = Resources.Load<Sprite>($"Sprites/{cardInfo.objName}");
        skillExplantion[0].text = cardInfo.GetSkillExplantion(1);
        skillExplantion[1].text = cardInfo.GetSkillExplantion(2);
        skillExplantion[2].text = cardInfo.GetSkillExplantion(3);

    }
}
