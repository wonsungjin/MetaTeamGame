using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCards : MonoBehaviour
{
    [SerializeField] private CardUI card;
    [SerializeField] private GameObject[] direction;
    [SerializeField] private GameObject[] tier;
    private Dictionary<int, List<CardInfo>> customDeckList = new Dictionary<int, List<CardInfo>>();
    void Start()
    {
        CardUI obj = null;
        CardInfo[] cards = Resources.LoadAll<CardInfo>($"ScriptableDBs/");
        for(int i = 0; i<cards.Length;i++)
        {
            AddTierList(cards[i].tier, cards[i]);
        }
        for (int tierNum = 1; tierNum < 7; tierNum++)
        {            
            List<CardInfo> list = GetTierList(tierNum);
            if (list==null) continue;
            for (int i = 0; i < list.Count; i++)
            {
                obj = GameObject.Instantiate<CardUI>(card);
                obj.transform.SetParent(tier[tierNum-1].transform);
                obj.transform.localScale = Vector3.one;
                obj.SetMyInfo(list[i].objName);
            }
        }
    }
    private int yPos;
    public void OnClick_Move_DownCamera()
    {
        if (yPos > -50)
        {
            if (yPos == 0) if(!direction[0].activeSelf) direction[0].SetActive(true);
            yPos -= 10;
            if (yPos <= -50)  direction[1].SetActive(false);
            StartCoroutine(COR_OnClick_Move_Camera(0));
        }
    }
    public void OnClick_Move_UpCamera()
    {
        if (yPos < 0)
        {
            if (yPos <= -50) if (!direction[1].activeSelf) direction[1].SetActive(true);
            yPos += 10;
            if (yPos >= 0)  direction[0].SetActive(false);
            StartCoroutine(COR_OnClick_Move_Camera(1));
        }
    }
    private WaitForSeconds delay = new WaitForSeconds(0.01f);
    IEnumerator COR_OnClick_Move_Camera(int direction)
    {
        if (direction == 0)//down
        {
            while (Camera.main.transform.position.y > yPos)
            {
                Camera.main.transform.Translate(Vector2.down/2);
                yield return delay;
            }
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yPos,-10);
        }
        else//up
        {
            while (Camera.main.transform.position.y < yPos)
            {
                Camera.main.transform.Translate(Vector2.up/2);
                yield return delay;
            }
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, yPos,-10);

        }
        

    }
    public void AddTierList(int tier, CardInfo name)
    {
        Debug.Log(tier);
        Debug.Log(name);
        List<CardInfo> list = null;
        bool listCheck = customDeckList.TryGetValue(tier, out list);
        if (listCheck == false)
        {
            list = new List<CardInfo>();
            customDeckList.Add(tier, list);
        }
        list.Add(name);
    }
    public List<CardInfo> GetTierList(int tier)
    {
        Debug.Log(tier);
        Debug.Log(name);
        List<CardInfo> list = null;
        bool listCheck = customDeckList.TryGetValue(tier, out list);
        return list;
        
    }
}
