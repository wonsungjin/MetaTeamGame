using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class  UIManager : MonoBehaviour
{
    GameObject battleSceneUI = null;
    GameObject ResultSceneUI = null;

    public GameObject[] playerArrangement = new GameObject[6];
    public Position[] playerPosition = new Position[6];

    public void BattleUIInit()
    {
        battleSceneUI = GameObject.Find("BattleSceneCanvas");
        battleSceneUI.SetActive(false);
    }

    public void OnBattleUI()
    {
        battleSceneUI.SetActive(true);
    }

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
