using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{

    [Header("슬라이더")]
    [SerializeField] public Slider timerSlider = null;
    [Header("버튼")]
    [SerializeField] public Button reFreshButton = null;
    [SerializeField] public Button levelUpButton = null;
    [SerializeField] public Button infoButton = null;
    [SerializeField] public Button optionButton = null;
    [Header("텍스트")]
    [SerializeField] public Text NowShopLevelTXT = null;
    [SerializeField] public Text goldTXT = null;
    [SerializeField] public Text shopLevelTXT = null;
    [SerializeField] public Text sellTXT = null;
    [SerializeField] public Text timerTXT = null;
    [SerializeField] public GameObject sell = null;

    public int shopMoney = 0;
    public int goldCount = 10;
    public int shopLevel = 1;
    public float timer = 60f;
    private bool isScene = false;

    public void Init_Scene2()
    {
        skillExplantion2 = GameObject.Find("Explantion");
        skillExplantionText[0] = GameObject.Find("cardName").GetComponent<TextMeshProUGUI>();
        skillExplantionText[1] = GameObject.Find("level1").GetComponent<TextMeshProUGUI>();
        skillExplantionText[2] = GameObject.Find("level2").GetComponent<TextMeshProUGUI>();
        skillExplantionText[3] = GameObject.Find("level3").GetComponent<TextMeshProUGUI>();
        timerSlider = GameObject.Find("TimerSlider").GetComponent<Slider>();
        reFreshButton = GameObject.Find("ReFreshButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("LevelUPButton").GetComponent<Button>();
        infoButton = GameObject.Find("InfoButton").GetComponent<Button>();
        optionButton = GameObject.Find("OptionButton").GetComponent<Button>();
        NowShopLevelTXT = GameObject.Find("NowShopLevelTXT").GetComponent<Text>();
        goldTXT = GameObject.Find("GoldTXT").GetComponent<Text>();
        shopLevelTXT = GameObject.Find("ShopLevelUpTXT").GetComponent<Text>();
        sellTXT = GameObject.Find("SellTXT").GetComponent<Text>();
        timerTXT = GameObject.Find("TimerTXT").GetComponent<Text>();
        sell = GameObject.Find("Sell");
        skillExplantion2.SetActive(false);
        timerSlider.maxValue = timer;
        sell.gameObject.SetActive(false);
        sellTXT.gameObject.SetActive(false);

        isScene = true;
    }



    private void Update()
    {
        if (!isScene) return;


            // 시간이 변경한 만큼 slider Value 변경을 합니다.
            timer -= Time.deltaTime;
            timerSlider.value = timer;
            timerTXT.text = string.Format("Timer : {0:N0}", timer);

            goldTXT.text = "" + goldCount.ToString();
            shopLevelTXT.text = "Shop Gold :" + shopMoney.ToString();
            NowShopLevelTXT.text = "Shop Level :" + shopLevel.ToString();
        

        if (sell.activeSelf == false)
        {
            sellTXT.gameObject.SetActive(false);
        }
        else
        {
            sellTXT.gameObject.SetActive(true);
        }
    }
}

//41 23 56 70
//4 23 5 70
//42 31 47 60
//40 27 35
//0