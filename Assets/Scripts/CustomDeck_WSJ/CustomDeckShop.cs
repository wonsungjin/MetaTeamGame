using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomDeckShop : MonoBehaviour
{
    public CustomDeck customDeck = new CustomDeck();
    private Dictionary<int, List<string>> customDeckList = new Dictionary<int, List<string>>();
    public bool isJoinShop { get; private set; }
    public void OnClick_Create_CustomDeck()
    {
        Debug.Log("�ƿ�");
        List<string> list = null;
        if (customDeckList.TryGetValue(1, out list)) customDeck.tier_1 = list;
        if (customDeckList.TryGetValue(2, out list)) customDeck.tier_2 = list;
        if (customDeckList.TryGetValue(3, out list)) customDeck.tier_3 = list;
        if (customDeckList.TryGetValue(4, out list)) customDeck.tier_4 = list;
        if (customDeckList.TryGetValue(5, out list)) customDeck.tier_5 = list;
        if (customDeckList.TryGetValue(6, out list)) customDeck.tier_6 = list;
        //if (customDeck.tier_1.Count < 8
        // && customDeck.tier_2.Count < 8
        // && customDeck.tier_3.Count < 8
        // && customDeck.tier_4.Count < 8
        // && customDeck.tier_5.Count < 8
        // && customDeck.tier_6.Count < 8) return;
        GameMGR.Instance.dataBase.inventoryData.AddCustomDeck(customDeck);
        customDeck = new CustomDeck();
        GameMGR.Instance.uiManager.OnClick_Move_Back();
    }
    public void OnClick_Join_CustomDeckShop()
    {
        isJoinShop = true;
    }
    public void OnClick_Exit_CustomDeckShop()
    {
        isJoinShop = false;
    }
    public int AddTierList(int tier, string name)
    {
        Debug.Log(tier);
        Debug.Log(name);
        List<string> list = null;
        bool listCheck = customDeckList.TryGetValue(tier, out list);
        if (listCheck == false)
        {
            list = new List<string>();
            customDeckList.Add(tier,list);
        }
        if (list.Count < 8)
        {
            if (list.Contains(name) == false) list.Add(name);
            else { list.Remove(name); Debug.Log("����"); }
        }
        else
        {
            list.Remove(name); Debug.Log("����");
            return list.Count+1;
        }
        return 0;
    }
    public void ClearCustomDeckList()
    {
        customDeckList.Clear();
        GameMGR.Instance.shopCards.ClearCardUIClick();
    }
}