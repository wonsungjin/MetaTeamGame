using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDeck : MonoBehaviour
{
    private CustomDeck myDeck = null;
    List<GameObject> myDeckList = new List<GameObject>();
    [SerializeField] private CardUI card;
    private bool isJoin;
    public void SetMyDeck(CustomDeck customDeck)
    {
        myDeck = customDeck;
    }
    public void OnClick_Delete_MyDeck()
    {
        Debug.Log("삭제");
        GameMGR.Instance.dataBase.inventoryData.DeleteCustomDeck(myDeck.Num);
        Destroy(gameObject);
    }
    public void OnClick_Move_MyDeckInfo()//내 덱 누를때
    {
        if (isJoin)//두번째부터는 생성 해둔애들 풀링
        {
            Debug.Log("머지");
            for (int i = 0; i < myDeckList.Count; i++)
                myDeckList[i].gameObject.SetActive(true);
            GameMGR.Instance.uIMGR.OnClick_Join_MyDeckInfo(myDeck);
        }
        else//처음 들어올때 덱에든 유닛을 모두 생성
        {
            isJoin = true;
            CardUI obj = null;
            if (myDeck.tier_1.Count > 0&&myDeck.tier_1[0]!="")
                for (int i = 0; i < myDeck.tier_1.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_1[i].Replace(" ",""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[0].transform);
                    obj.transform.localScale = Vector3.one;
                }
            //Debug.Log(myDeck.tier_2[0]);
            if (myDeck.tier_2.Count > 0 &&myDeck.tier_2[0] != "" )
                for (int i = 0; i < myDeck.tier_2.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_2[i].Replace(" ", ""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[1].transform);
                    obj.transform.localScale = Vector3.one;
                }
            if (myDeck.tier_3.Count > 0&&myDeck.tier_3[0] != "")
                for (int i = 0; i < myDeck.tier_3.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_3[i].Replace(" ", ""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[2].transform);
                    obj.transform.localScale = Vector3.one;
                }
            if (myDeck.tier_4.Count > 0&&myDeck.tier_4[0] != "")
                for (int i = 0; i < myDeck.tier_4.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_4[i].Replace(" ", ""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[3].transform);
                    obj.transform.localScale = Vector3.one;
                }
            if (myDeck.tier_5.Count > 0&& myDeck.tier_5[0] != "")
                for (int i = 0; i < myDeck.tier_5.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_5[i].Replace(" ", ""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[4].transform);
                    obj.transform.localScale = Vector3.one;
                }
            if (myDeck.tier_6.Count > 0&& myDeck.tier_6[0] != "")
                for (int i = 0; i < myDeck.tier_6.Count; i++)
                {
                    obj = GameObject.Instantiate<CardUI>(card);
                    myDeckList.Add(obj.gameObject);
                    obj.SetMyInfo(myDeck.tier_6[i].Replace(" ", ""));
                    obj.transform.SetParent(GameMGR.Instance.uIMGR.tier[5].transform);
                    obj.transform.localScale = Vector3.one;
                }
            GameMGR.Instance.uIMGR.OnClick_Join_MyDeckInfo(myDeck);
        }

    }
    public void DelateMyDeckList()
    {
        Debug.Log("머지실행");
        for (int i = 0; i < myDeckList.Count; i++)
            myDeckList[i].gameObject.SetActive(false);
    }
}



