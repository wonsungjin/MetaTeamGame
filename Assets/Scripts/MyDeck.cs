using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDeck : MonoBehaviour
{
    private CustomDeck myDeck = null;
    [SerializeField] private CardUI card;
    public void SetMyDeck(CustomDeck customDeck)
    {
        myDeck = customDeck;
    }
    public void OnClick_Move_MyDeckInfo()//내일 덱 ui매니저한테 넘겨서 거기서 인스턴스 하세요
    {
        GameMGR.Instance.uIMGR.OnClick_Join_MyDeckInfo();
        CardUI obj = null;
        if(myDeck.tier_1[0]!="")
        for (int i = 0; i < myDeck.tier_1.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(0,myDeck, obj,i);
        }
        if (myDeck.tier_2[0] != "")
            for (int i = 0; i < myDeck.tier_2.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(1, myDeck, obj, i);
        }
        if (myDeck.tier_3[0] != "")
            for (int i = 0; i < myDeck.tier_3.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(2, myDeck, obj, i);
        }
        if (myDeck.tier_4[0] != "")
            for (int i = 0; i < myDeck.tier_4.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(3, myDeck, obj, i);
        }
        if (myDeck.tier_5[0] != "")
            for (int i = 0; i < myDeck.tier_5.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(4, myDeck, obj, i);
        }
        if (myDeck.tier_6[0] != "")
            for (int i = 0; i < myDeck.tier_6.Count; i++)
        {
            obj = GameObject.Instantiate<CardUI>(card);
            GameMGR.Instance.uIMGR.MyDeckTier(5, myDeck, obj, i);
        }

    }
}



