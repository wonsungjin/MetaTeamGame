using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Batch : MonoBehaviourPun
{

    public void NewCreateBatch(int playerNum,  bool myCard = true)
    {
        List<Card> cardList = null;
        GameMGR.Instance.playerList.TryGetValue(playerNum, out cardList);
        for (int i = 0; i < cardList.Count; i++)
        { 
            if(cardList[i] == null) continue;
            Debug.Log("생성" + cardList[i].name);
            Card unitCard = GameObject.Instantiate<Card>(cardList[i]);

            // player Unit 위치 설정
            if (myCard == true)
            {
                unitCard.transform.position = myCardPosition[i + 1].position;
                if(i<3) GameMGR.Instance.battleLogic.playerForwardUnits.Add(unitCard.gameObject);
                else GameMGR.Instance.battleLogic.playerBackwardUnits.Add(unitCard.gameObject);
            }

            // enemy Unit 위치 설정
            else if (myCard == false)
            {
                unitCard.transform.position = enemyCardPosition[i + 1].position;
                unitCard.SetFlip(true);
                if (i < 3) GameMGR.Instance.battleLogic.enemyForwardUnits.Add(unitCard.gameObject);         
                else GameMGR.Instance.battleLogic.enemyBackwardUnits.Add(unitCard.gameObject);                
            }
        else
        {
            Debug.Log("CreateBatch : myCard 값 확인필요");
        }
        }
    }



}
