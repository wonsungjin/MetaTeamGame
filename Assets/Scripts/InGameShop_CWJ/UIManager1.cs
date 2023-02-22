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
    public TextMeshProUGUI hpTXT = null;
    public TextMeshProUGUI shopLevelTXT = null;
    public TextMeshProUGUI PlayerLifeTXT = null;
    public Image userProfile = null;
    public Image[] userProfileImage = null;
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
    private int nowhp = 20;

    public bool isTimeOver = false;
    bool isTimeOEnd = false;

    WaitForSeconds wait = new WaitForSeconds(0.1f);

    public void Init_Scene2()
    {
        unitSprite = new Image[7];
        userProfileImage = new Image[8];
        blackUI = GameObject.Find("BlackUI");
        storePannel = GameObject.Find("StorePannel");
        skillExplantion2 = GameObject.Find("Explantion");
        finalSceneUI = GameObject.Find("FinalSceneCanvas");
        skillExplantionText[0] = GameObject.Find("cardName").GetComponent<TextMeshProUGUI>();
        skillExplantionText[1] = GameObject.Find("level1").GetComponent<TextMeshProUGUI>();
        skillExplantionText[2] = GameObject.Find("level2").GetComponent<TextMeshProUGUI>();
        skillExplantionText[3] = GameObject.Find("level3").GetComponent<TextMeshProUGUI>();
        unitSprite[6] = GameObject.Find("CardImage").GetComponent<Image>();
        timerSlider = GameObject.Find("ImageFill").GetComponent<Image>();
        userProfile = GameObject.Find("UserProfileImage").GetComponent<Image>();
        reFreshButton = GameObject.Find("ReFreshButton").GetComponent<Button>();
        levelUpButton = GameObject.Find("LevelUPButton").GetComponent<Button>();
        optionButton = GameObject.Find("OptionButton").GetComponent<Button>();
        NowShopLevelTXT = GameObject.Find("NowShopLevelTXT").GetComponent<TextMeshProUGUI>();
        PlayerLifeTXT = GameObject.Find("PlayerLife").GetComponent<TextMeshProUGUI>();
        goldTXT = GameObject.Find("GoldTXT").GetComponent<TextMeshProUGUI>();
        hpTXT = GameObject.Find("HpTXT").GetComponent<TextMeshProUGUI>();
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
        userProfileImage[0] = GameObject.Find("UserImage1").GetComponent<Image>();
        userProfileImage[1] = GameObject.Find("UserImage2").GetComponent<Image>();
        userProfileImage[2] = GameObject.Find("UserImage3").GetComponent<Image>();
        userProfileImage[3] = GameObject.Find("UserImage4").GetComponent<Image>();
        userProfileImage[4] = GameObject.Find("UserImage5").GetComponent<Image>();
        userProfileImage[5] = GameObject.Find("UserImage6").GetComponent<Image>();
        userProfileImage[6] = GameObject.Find("UserImage7").GetComponent<Image>();
        userProfileImage[7] = GameObject.Find("UserImage8").GetComponent<Image>();

        finalSceneUI.SetActive(false);
        playerBatchUI.SetActive(false);
        InitUI();
        StartCoroutine(COR_DelayBlackUI());
    }
    IEnumerator COR_DelayBlackUI()
    {
        yield return new WaitForSeconds(2f);
        Faid(blackUI, faidType.Out, 0.03f);
        for (int i = 0; i < userProfileImage.Length; i++)
        {
            if (GameMGR.Instance.batch.PlayerProfileList.Count < i+1) break;
        userProfileImage[i].sprite = Resources.Load<Sprite>($"Sprites/Profile/{GameMGR.Instance.batch.PlayerProfileList[i]}");
    }
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

            if (timer <= 15f && timer >= 1f) timerSound();
            else if (timer <= 0f)
            {
                Debug.Log("0이됐다 시간이");
                GameMGR.Instance.timerSound.TimeSoundEnd();
                isTimeOEnd = true;
            }
        }
    }

    void timerSound()
    {
        GameMGR.Instance.timerSound.TimeSound();
        isTimerFast = true;
    }

    void InitUI()
    {
        skillExplantion2.SetActive(false);
        sell.gameObject.SetActive(false);
        isScene = true;
        hpTXT.text = nowhp.ToString();
    }
}