using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ResultSceneUI : MonoBehaviour
{
    public GameObject[] playerArrangement = new GameObject[6];
    public Position[] playerPosition = new Position[6];


    private void Init()
    {
        PlayerSetArrangement();
        PlayerSetPosition();
    }

    private void PlayerSetArrangement()
    {
        for (int i = 0; i < playerArrangement.Length; i++)
        {

        }
    }

    private void PlayerSetPosition()
    {
        
    }

    public void PlayerBattleWin()
    {

    }

    public void PlayerBattleLose()
    {

    }
}
