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
        
        CardInfo[] cards = Resources.LoadAll<CardInfo>($"ScriptableDBs/");
        for(int i = 0; i<cards.Length;i++)
        {
            Debug.Log(cards[i].tier + "~~~~~~~~추가~~~~~~~~~~~" + cards[i].objName);

            if(cards[i].appear=="TRUE") AddTierList(cards[i].tier, cards[i]);
        }
        StartCoroutine(CreateInit());
        
    }
    IEnumerator CreateInit()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => GameMGR.Instance.dataBase.isFindUnit);
        CardUI obj = null;
        for (int tierNum = 1; tierNum < 7; tierNum++)
        {
            List<CardInfo> list = GetTierList(tierNum);

                List<string> hashList = GameMGR.Instance.dataBase.unitData.hashtable["A" + tierNum] as List<string>;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].appear == "FALSE")
                {
                    Debug.Log("삭제&^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^" + list[i].objName);
                    //list.Remove(list[i]);
                    continue;
                }
                Debug.Log("생성~~~~~~~~~~~~~~~~~~~" + list[i].objName);

                obj = GameObject.Instantiate<CardUI>(card);
                obj.transform.localScale = Vector3.one;
                Debug.Log(GameMGR.Instance.dataBase.unitData.hashtable["A" + tierNum] as List<string>);
                    obj.transform.SetParent(GameMGR.Instance.uiManager.tier1[tierNum - 1].transform);
                if (hashList != null)
                    if (hashList.Contains(list[i].objName.Replace(" ", "")) == false)
                    {
                        obj.DonHave();
                    }
                    else obj.transform.SetAsFirstSibling();
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
