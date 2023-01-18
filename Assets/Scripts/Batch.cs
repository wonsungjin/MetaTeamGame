using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batch : MonoBehaviourPun
{
    Dictionary<int, List<Card>> playerList = new Dictionary<int, List<Card>>();

    // 상점의 배치 정보를 전달 받음
    [PunRPC]
    public void SetBatch(int playerNum,Card card)
    {
        List<Card> list = null;
        Card instance = Resources.Load<Card>($"Prefabs/{card.name}");
        bool listCheck = playerList.TryGetValue(playerNum, out list);
        if (listCheck == false)
        {
            list = new List<Card>();
        }
        instance.ChangeValue("hp", card.curHP);
        instance.ChangeValue("attack", card.curAttackValue);
        instance.ChangeValue("level", card.level);
        instance.ChangeValue("exp", card.curEXP);
        list.Add(instance);
    }

    // 배틀씬 유닛 배치
    public Card CreateBatch(int playerNum,int cardNum,bool myCard=true)
    {
        List<Card> list = null;
        playerList.TryGetValue(playerNum, out list);
        Card unitCard = GameObject.Instantiate<Card>(list[cardNum]);
        if (myCard == true)
        {
            unitCard.transform.position = new Vector3(0, 0, 0);
        }
        else
        {

        }

        return unitCard;
    }
}
