using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class  UIManager : MonoBehaviour
{
    GameObject battleSceneUI = null;
    GameObject ResultSceneUI = null;
    GameObject winUI = null;
    GameObject loseUI = null;
    
    public GameObject[] playerArrangement = new GameObject[6];
    public Transform[] playerPosition = new Transform[6];

    private void Awake()
    {
        ResultUnitPosition();
    }

    #region BattleScene
    public void BattleUIInit()
    {
        battleSceneUI = GameObject.Find("BattleSceneCanvas");
        winUI = GameObject.Find("ResultWin");
        loseUI = GameObject.Find("ResultLose");

        winUI.SetActive(false);
        loseUI.SetActive(false);
        battleSceneUI.SetActive(false);
    }


    public void OnBattleUI()
    {
        
    }
    #endregion


    #region RoundResultScene
    private void ResultUnitPosition()
    {
            playerPosition = GameObject.Find("ResultPlayerPosition").GetComponentsInChildren<Transform>();
    }

    public void ResultSceneInit()
    {
        ResultSceneUI = GameObject.Find("ResultSceneCanvas");
        ResultSceneUI.SetActive(false);
        PlayerSetArrangement();
    }

    public void PlayerSetArrangement()
    {
        for (int i = 0; i < playerArrangement.Length; i++)
        {
            playerArrangement[i].transform.position = playerPosition[i].position;
        }
    }

    public void ResetPlayerUnit()
    {
        for (int i = 0; i < playerArrangement.Length; i++) { Destroy(playerArrangement[i]); }        
    }

    public void PlayerBattleWin()
    {
        winUI.SetActive(true);
    }

    public void PlayerBattleLose()
    {
        loseUI.SetActive(true);
    }
    #endregion
}
