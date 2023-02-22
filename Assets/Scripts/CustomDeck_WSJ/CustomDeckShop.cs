using System.Collections.Generic;
using UnityEngine;

public class CustomDeckShop : MonoBehaviour
{
    public CustomDeck customDeck = new CustomDeck();
    private Dictionary<int, List<string>> customDeckList = new Dictionary<int, List<string>>();
    public bool isJoinShop { get; private set; }
    public void OnClick_Create_CustomDeck()
    {
        Debug.Log("아오");
        List<string> list = null;
        if (customDeckList.TryGetValue(1, out list)) customDeck.tier_1 = list;
        if (customDeckList.TryGetValue(2, out list)) customDeck.tier_2 = list;
        if (customDeckList.TryGetValue(3, out list)) customDeck.tier_3 = list;
        if (customDeckList.TryGetValue(4, out list)) customDeck.tier_4 = list;
        if (customDeckList.TryGetValue(5, out list)) customDeck.tier_5 = list;
        if (customDeckList.TryGetValue(6, out list)) customDeck.tier_6 = list;
        if (customDeck.tier_1.Count <1
         || customDeck.tier_2.Count <1
         || customDeck.tier_3.Count <1
         || customDeck.tier_4.Count <1
         || customDeck.tier_5.Count <1
         || customDeck.tier_6.Count <1)
        {
            GameMGR.Instance.uiManager.SetNameMakeUI(false);
            return;
        }

        if (customDeck.tier_1.Count < 4
         || customDeck.tier_2.Count < 4
         || customDeck.tier_3.Count < 4
         || customDeck.tier_4.Count < 4
         || customDeck.tier_5.Count < 4
         || customDeck.tier_6.Count < 4)
        {
            RandomAddDeck();
        }
        GameMGR.Instance.dataBase.inventoryData.AddCustomDeck(customDeck);
        GameMGR.Instance.uiManager.SetNameMakeUI(false);
        customDeck = new CustomDeck();
        GameMGR.Instance.uiManager.OnClick_Move_Back();
    }
    public void Create_CustomDeck()
    {
        Debug.Log("아오");
        List<string> list = null;
        customDeck.DeckName = "Free Pack";
        if (customDeckList.TryGetValue(1, out list)) customDeck.tier_1 = list;
        if (customDeckList.TryGetValue(2, out list)) customDeck.tier_2 = list;
        if (customDeckList.TryGetValue(3, out list)) customDeck.tier_3 = list;
        if (customDeckList.TryGetValue(4, out list)) customDeck.tier_4 = list;
        if (customDeckList.TryGetValue(5, out list)) customDeck.tier_5 = list;
        if (customDeckList.TryGetValue(6, out list)) customDeck.tier_6 = list;
        if (customDeck.tier_1.Count < 4
         || customDeck.tier_2.Count < 4
         || customDeck.tier_3.Count < 4
         || customDeck.tier_4.Count < 4
         || customDeck.tier_5.Count < 4
         || customDeck.tier_6.Count < 4)
        {
            RandomAddDeck();
        }
        GameMGR.Instance.dataBase.inventoryData.AddCustomDeck(customDeck);
        customDeck = new CustomDeck();
    }
    public void OnClick_Join_NameMakeUI()
    {
        GameMGR.Instance.uiManager.SetNameMakeUI(true);
    }
    public void OnChageName(string st)
    {
        customDeck.DeckName = st;
        if (customDeck.DeckName == "Free Pack") customDeck.DeckName = "My Pack";
    }
    public void OnClick_Join_CustomDeckShop()
    {
        isJoinShop = true;
        for (int i = 0; i < 6; i++)
            GameMGR.Instance.uiManager.tierCountText[i].text = "0/14";
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
            customDeckList.Add(tier, list);
        }
        if (list.Count < 2)
        {
            if (list.Contains(name) == false)
            {
                list.Add(name);
                GameMGR.Instance.uiManager.tierCountText[tier - 1].text = $"{list.Count}/14";
            }
            else
            {
                list.Remove(name);
                GameMGR.Instance.uiManager.tierCountText[tier - 1].text = $"{list.Count}/14";
                Debug.Log("삭제");
            }
        }
        else
        {
            list.Remove(name); Debug.Log("삭제");
            GameMGR.Instance.uiManager.tierCountText[tier - 1].text = $"{list.Count}/14";
            return list.Count + 1;
        }
        return 0;
    }
    public void ClearCustomDeckList()
    {
        customDeckList.Clear();
        GameMGR.Instance.shopCards.ClearCardUIClick();
    }
    public void RandomAddDeck()
    {
        List<CardInfo> shopList = null;
        List<string> list = null;
        bool value;
        bool listCheck;
        int ran;
        for (int tier = 1; tier < 7; tier++)
        {
            value = GameMGR.Instance.shopCards.customDeckList.TryGetValue(tier, out shopList);
            listCheck = customDeckList.TryGetValue(tier, out list);
            if (list == null) list = new List<string>();
            ran = Random.Range(0, shopList.Count);
            if (tier == 1) list = customDeck.tier_1;
            else if (tier == 2) list = customDeck.tier_2;
            else if (tier == 3) list = customDeck.tier_3;
            else if (tier == 4) list = customDeck.tier_4;
            else if (tier == 5) list = customDeck.tier_5;
            else if (tier == 6) list = customDeck.tier_6;

            if (value != false)
            {
                while (list.Count < 4)
                {
                    while (list.Contains(shopList[ran].objName) == true || shopList[ran].appear == "FALSE")
                    {
                        ran = Random.Range(0, shopList.Count);
                    }
                    list.Add(shopList[ran].objName);
                }
            }
        }
    }
}