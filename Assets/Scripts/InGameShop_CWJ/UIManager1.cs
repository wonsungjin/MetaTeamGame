using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    public GameObject storePannel = null;
    public GameObject sell = null;
    public GameObject sellTXT = null;
    public Button reFreshButton = null;
    public Button levelUpButton = null;
    public Button optionButton = null;
    public Image timerSlider = null;
    public TextMeshProUGUI NowShopLevelTXT = null;
    public TextMeshProUGUI goldTXT = null;
    public TextMeshProUGUI shopLevelTXT = null;
    public Image[] unitSprite = null;
    public GameObject playerBatchUI = null;
    public TextMeshProUGUI playerName = null;

    public TextMeshProUGUI timerTXT = null;

    public int shopMoney = 0;
    public int goldCount = 0;
    public int shopLevel = 0;
    public int hireUnitCost = 3; // 유닛 고용 비용
    public float timer = 60f;
    private bool isScene = false;
    public bool isTimerFast = false;

    public bool isTimeOver = false;
    bool isTimeOEnd = false;
    public void Init_Scene2()
    {
        unitSprite = new Image[7];
        blackUI = GameObject.Find("BlackUI");
        storePannel = GameObject.Find("StorePannel");
        skillExplantion2 = GameObject.Find("Explantion");
        battleSceneUI = GameObject.Find("BattleSceneCanvas");
        finalSceneUI = GameObject.Find("FinalSceneCanvas");
        skillExplantionText[0] = GameObject.Find("cardName").GetComponent<TextMeshProUGUI>();
        skillExplantionText[1] = GameObject.Find("level1").GetComponent<TextMeshProUGUI>();
        skillExplantionText[2] = GameObject.Find("level2").GetComponent<TextMeshProUGUI>();
        skillExplantionText[3] = GameObject.Find("level3").GetComponent<TextMeshProUGUI>();
        unitSprite[6] = GameObject.Find("CardImage").GetComponent<Image>();
        timerSlider = GameObject.Find("ImageFill").GetComponent<Image>();
        reFreshButton = GameObject.Find("ReFreshButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("LevelUPButton").GetComponent<Button>();
        optionButton = GameObject.Find("OptionButton").GetComponent<Button>();
        NowShopLevelTXT = GameObject.Find("NowShopLevelTXT").GetComponent<TextMeshProUGUI>();
        goldTXT = GameObject.Find("GoldTXT").GetComponent<TextMeshProUGUI>();
        shopLevelTXT = GameObject.Find("ShopLevelUpTXT").GetComponent<TextMeshProUGUI>();
        timerTXT = GameObject.Find("TimerTXT").GetComponent<TextMeshProUGUI>();
        playerName = GameObject.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        sell = GameObject.Find("Sell");
        playerBatchUI = GameObject.Find("PlayerBatchUI");
        unitSprite[0] = GameObject.Find("Unit1").GetComponent<Image>();
        unitSprite[1] = GameObject.Find("Unit2").GetComponent<Image>();
        unitSprite[2] = GameObject.Find("Unit3").GetComponent<Image>();
        unitSprite[3] = GameObject.Find("Unit4").GetComponent<Image>();
        unitSprite[4] = GameObject.Find("Unit5").GetComponent<Image>();
        unitSprite[5] = GameObject.Find("Unit6").GetComponent<Image>();

        finalSceneUI.SetActive(false);
        playerBatchUI.SetActive(false);
        Faid(blackUI, faidType.Out, 0.03f);
        InitUI();
    }

    private void Update()
    {
        if (!isScene) return;

        if (isTimeOEnd == false)
        {
            // 시간이 변경한 만큼 slider Value 변경을 합니다.
            timer -= Time.deltaTime;
            timerSlider.fillAmount = timer / 100 * 1.667f;
            timerTXT.text = string.Format("Time : {0:N0}sec", timer);
        }

        if (timer <= 15f) timerSound();
        else if (timerSlider.fillAmount <= 0f)
        {
            GameMGR.Instance.timerSound.TimeSoundEnd();
            isTimeOEnd = true;
            timer = 0f;
        }
    }

    void timerSound()
    {
        Debug.Log("여기로 들어옴");
        GameMGR.Instance.timerSound.TimeSound();
        isTimerFast = true;
    }

    void InitUI()
    {
        skillExplantion2.SetActive(false);
        sell.gameObject.SetActive(false);
        //battleSceneUI.SetActive(false);
        isScene = true;
    }
}