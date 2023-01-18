using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    [Header("��ư")]
    public Button ReFreshButton = null;
    public Button LevelUpButton = null;
    [Header("�ؽ�Ʈ")]
    [SerializeField] Text NowShopLevelTXT = null;
    [SerializeField] Text goldTXT = null;
    public Text shopLevelTXT = null;
    [SerializeField] Text sellTxt = null;

    [SerializeField] public GameObject sell = null;

    public int shopMoney = 0;
    public int goldCount = 10;
    public int shopLevel = 1;

    // Public ������Ƽ�� �����ؼ� �ܺο��� private ��������� ���ٸ� �����ϰ� ����
    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        // ó�� ���۽� �ʱ�ȭ
        FirstReset();

        if (null == instance)
        {
            // �� ���۵ɶ� �ν��Ͻ� �ʱ�ȭ, ���� �Ѿ���� �����Ǳ����� ó��
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instance��, GameManager�� �����Ѵٸ� GameObject ���� 
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        goldTXT.text = "Gold : " + goldCount.ToString();
        shopLevelTXT.text = "Shop Gold :" + shopMoney.ToString();
        NowShopLevelTXT.text = "Shop Level :" + shopLevel.ToString();

        if(sell.activeSelf == false)
        {
            sellTxt.gameObject.SetActive(false);
        }
        else
        {
            sellTxt.gameObject.SetActive(true);
        }
    }



    private void FirstReset()
    {
        sell.gameObject.SetActive(false);
        sellTxt.gameObject.SetActive(false);
    }
}