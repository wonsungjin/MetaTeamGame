using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviour
{
    Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();
    public void SetBatch(int playerNum,string objName,int level,int hp,int attackValue)
    {
        List<Card> list = null;
        Card instance = Resources.Load<Card>($"Prefabs/{objName}");
        bool listCheck = playerList.TryGetValue(playerNum, out list);
        if (listCheck == false)
        {
            list = new List<Card>();
        }
        instance.ChangeValue("hp", hp);
        instance.ChangeValue("attack", attackValue);
        instance.ChangeValue("level", level); 
        list.Add(instance);
    }
    public void CreateBatch(int playerNum,int cardNum,bool myCard=true)
    {
        List<Card> list = null;
        playerList.TryGetValue(playerNum, out list);
        Card a = GameObject.Instantiate<Card>(list[cardNum]);
        if (myCard == true)
        {
            a.transform.position = new Vector3(0, 0, 0);

        }
        else
        {

        }
    }
}
