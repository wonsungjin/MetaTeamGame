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

    public TextMeshProUGUI timerTXT = null;

    public int shopMoney = 0;
    public int goldCount = 0;
    public int shopLevel = 1;
    public float timer = 60f;
    private bool isScene = false;

    public void Init_Scene2()
    {
        storePannel = GameObject.Find("StorePannel");
        skillExplantion2 = GameObject.Find("Explantion");
        skillExplantionText[0] = GameObject.Find("cardName").GetComponent<TextMeshProUGUI>();
        skillExplantionText[1] = GameObject.Find("level1").GetComponent<TextMeshProUGUI>();
        skillExplantionText[2] = GameObject.Find("level2").GetComponent<TextMeshProUGUI>();
        skillExplantionText[3] = GameObject.Find("level3").GetComponent<TextMeshProUGUI>();
        timerSlider = GameObject.Find("ImageFill").GetComponent<Image>();
        reFreshButton = GameObject.Find("ReFreshButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("LevelUPButton").GetComponent<Button>();
        optionButton = GameObject.Find("OptionButton").GetComponent<Button>();
        NowShopLevelTXT = GameObject.Find("NowShopLevelTXT").GetComponent<TextMeshProUGUI>();
        goldTXT = GameObject.Find("GoldTXT").GetComponent<TextMeshProUGUI>();
        shopLevelTXT = GameObject.Find("ShopLevelUpTXT").GetComponent<TextMeshProUGUI>();
        timerTXT = GameObject.Find("TimerTXT").GetComponent<TextMeshProUGUI>();
        sell = GameObject.Find("Sell");

        InitUI();

        Debug.Log(timerSlider);
        Debug.Log(timerTXT);
    }


    private void Update()
    {
        if (!isScene) return;

        // 시간이 변경한 만큼 slider Value 변경을 합니다.
        timer -= Time.deltaTime;
        timerSlider.fillAmount = timer / 100 * 1.667f;
        timerTXT.text = string.Format("Time : {0:N0}sec", timer);
    }

    void InitUI()
    {
        skillExplantion2.SetActive(false);
        sell.gameObject.SetActive(false);
        isScene = true;
    }
}