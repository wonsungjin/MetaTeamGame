using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class UIManager : MonoBehaviour
{
    GameObject battleSceneUI = null;
    GameObject ResultSceneUI = null;
    GameObject winUI = null;
    GameObject loseUI = null;

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
        battleSceneUI.SetActive(true);
    }
    #endregion


    #region RoundResultScene
    public void ResultUnitPosition()
    {
        playerPosition = GameObject.Find("ResultPlayerPosition").GetComponentsInChildren<Transform>();
    }

    public void ResultSceneInit()
    {
        ResultSceneUI = GameObject.Find("ResultSceneCanvas");
        winUI = GameObject.Find("ResultWin");
        loseUI = GameObject.Find("ResultLose");
        ResultSceneUI.SetActive(false);
    }

    public void OnResultUI()
    {
        ResultSceneUI.SetActive(true);
    }

    public void PlayerSetArrangement()
    {
        for (int i = 0; i < playerArrangement.Length; i++)
        {
            if (playerArrangement[i] != null) { playerArrangement[i].transform.position = playerPosition[i].position; }
        }
    }

    public void ResetPlayerUnit()
    {
        for (int i = 0; i < playerArrangement.Length; i++) { Destroy(playerArrangement[i]); }
    }

    public void PlayerBattleWin(bool isWin)
    {
        winUI.SetActive(isWin);
    }

    public void PlayerBattleLose(bool isWin)
    {
        loseUI.SetActive(isWin);
    }
    #endregion

    public IEnumerator COR_MoveToResultScene(bool Win)
    {
        yield return new WaitForSeconds(1f);
        Camera.main.gameObject.transform.position = new Vector3(40, 0, -10);

        GameMGR.Instance.audioMGR.BattleSceneBGM(false);
        if (Win)
        {
            GameMGR.Instance.audioMGR.BattleRoundResult(Win);
            GameMGR.Instance.uiManager.PlayerBattleWin(Win);
        }

        else if (!Win)
        {
            GameMGR.Instance.audioMGR.BattleRoundResult(Win);
            GameMGR.Instance.uiManager.PlayerBattleLose(!Win);
        }

        // 무승부 로직 추가필요
        yield return new WaitForSeconds(5f);

        GameMGR.Instance.uiManager.ResetPlayerUnit(); // Unit Reset
        GameMGR.Instance.spawner.TestButton();
    }
}
