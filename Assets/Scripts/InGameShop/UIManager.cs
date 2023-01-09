using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    [SerializeField] Text goldTXT = null;
    [SerializeField] Text notGoldTXT = null;

    [SerializeField] public GameObject sell = null;
    [SerializeField] Text sellTxt = null;

    public int goldCount = 10;

    // Public 프로퍼티로 선언해서 외부에서 private 멤버변수에 접근만 가능하게 구현
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
        sell.gameObject.SetActive(false);
        sellTxt.gameObject.SetActive(false);

        if (null == instance)
        {
            // 씬 시작될때 인스턴스 초기화, 씬을 넘어갈때도 유지되기위한 처리
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instance가, GameManager가 존재한다면 GameObject 제거 
            Destroy(this.gameObject);
        }
        notGoldTXT.gameObject.SetActive(false);
    }

    private void Update()
    {
        goldTXT.text = "Gold : " + goldCount.ToString();
        if(sell.activeSelf == false)
        {
            sellTxt.gameObject.SetActive(false);
        }
        else
        {
            sellTxt.gameObject.SetActive(true);
        }
    }

    public IEnumerator COR_NotGold()
    {
        notGoldTXT.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        notGoldTXT.gameObject.SetActive(false);
    }
}
