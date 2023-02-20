using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCards : MonoBehaviour
{
    [SerializeField] private CardUI card;
    public Dictionary<int, List<CardInfo>> customDeckList = new Dictionary<int, List<CardInfo>>();
    public List<CardUI> clearList = new List<CardUI>();
    public void Init()
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
                if (list[i].appear == "FALSE")
                {
                    list.Remove(list[i]);
                continue;
                }
                obj = GameObject.Instantiate<CardUI>(card);
                obj.transform.SetParent(GameMGR.Instance.uiManager.tier1[tierNum-1].transform);
                obj.transform.localScale = Vector3.one;
                obj.SetMyInfo(list[i].name);
                obj.OffFrame();
                clearList.Add(obj);
            }
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
    public  void ClearCardUIClick()
    {

        for(int i =0; i< clearList.Count;i++)
        {
            clearList[i].ClearClick();
        }
    }
}
