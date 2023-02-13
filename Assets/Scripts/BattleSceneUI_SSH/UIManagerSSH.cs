using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class  UIManager : MonoBehaviour
{
    GameObject battleSceneUI = null;

    public GameObject[] playerArrangement = new GameObject[6];
    public Transform[] playerPosition = new Transform[6];

    #region BattleScene
    public void BattleUIInit()
    {
        battleSceneUI = GameObject.Find("BattleSceneCanvas");

        battleSceneUI.SetActive(false);
    }


    public void OnBattleUI()
    {
        
    }
    #endregion


    #region RoundResultScene
    private void ResultSceneInit()
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
    #endregion
}
