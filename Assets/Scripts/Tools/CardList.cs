using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new CardList", menuName = "ScriptableObjects/CardList")]
public class CardList : ScriptableObject
{
    [SerializeField] internal  Dictionary<int,List<CardInfo>> cardInfo;
    public void InitCustom(int tier)
    {
        List<CardInfo> list = null;
        bool listCheck = cardInfo.TryGetValue(tier, out list);
        if (listCheck == false)
        {
            list = new List<CardInfo>();
        }
    }

}