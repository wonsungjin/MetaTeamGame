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
    [SerializeField] private GameObject cardPannel;
    [Header("PackList")]
    [SerializeField] private GameObject myPackList;
    [SerializeField] private MyDeck packButton;
    [SerializeField] private GameObject packAddButton;
    [SerializeField] private ToggleGroup toggleGroup;
    [Header("CardInfo")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI attackValue;
    [SerializeField] private TextMeshProUGUI hpValue;
    [SerializeField] private TextMeshProUGUI myDeckName;
    [SerializeField] private TextMeshProUGUI[] skillExplantion;

    [Header("Tier")]
    [SerializeField] public Transform myContent;
    [SerializeField] public Transform[] tier1;
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
        myDeckName = GameObject.Find("MyDeckName").GetComponent<TextMeshProUGUI>();
        hpValue = GameObject.Find("UNITHPValue").GetComponent<TextMeshProUGUI>();
        skillExplantion = new TextMeshProUGUI[3];
        tier1 = new Transform[6];
        skillExplantion[0] = GameObject.Find("Skillexplanation1").GetComponent<TextMeshProUGUI>();
        skillExplantion[1] = GameObject.Find("Skillexplanation2").GetComponent<TextMeshProUGUI>();
        skillExplantion[2] = GameObject.Find("Skillexplanation3").GetComponent<TextMeshProUGUI>();
        myContent = GameObject.Find("MyContent").transform;
        tier1[0] = GameObject.Find("ContentTier1").transform;
        tier1[1] = GameObject.Find("ContentTier2").transform;
        tier1[2] = GameObject.Find("ContentTier3").transform;
        tier1[3] = GameObject.Find("ContentTier4").transform;
        tier1[4] = GameObject.Find("ContentTier5").transform;
        tier1[5] = GameObject.Find("ContentTier6").transform;
        toggleGroup = FindObjectOfType<ToggleGroup>();
        customPannel.SetActive(false);
        packChoicePannel.SetActive(false);
        cardPannel.SetActive(false);
        myDeckPannel.SetActive(false);
    }
    public void SetMyDeckName(string name)
    {
        myDeckName.text = name;
    }
    public void OnCilck_Join_PackChoice()
    {
        lobbyPannel.SetActive(false);
        packChoicePannel.SetActive(true);
    }
    public void CreateMyPackButton(CustomDeck customDeck)
    {
        MyDeck obj = GameObject.Instantiate<MyDeck>(packButton);
        obj.transform.GetChild(3).GetComponent<Toggle>().group = toggleGroup;
        obj.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = customDeck.DeckName;
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
        attackValue.text = $"{cardInfo.atk}";
        hpValue.text = $"{cardInfo.hp}";
        cardName.text = $"{cardInfo.objName}";
        cardImage.sprite = Resources.Load<Sprite>($"Sprites/Nomal/{cardInfo.objName.Replace(" ","")}");
        skillExplantion[0].text = cardInfo.GetSkillExplantion(1);
        skillExplantion[1].text = cardInfo.GetSkillExplantion(2);
        skillExplantion[2].text = cardInfo.GetSkillExplantion(3);

    }
}
