using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public partial class UIManager : MonoBehaviour
{
    GameObject battleSceneUI = null;
    GameObject battleOptionPanel = null;

    GameObject ResultSceneUI = null;
    GameObject winUI = null;
    GameObject loseUI = null;

    TextMeshProUGUI winText = null;
    TextMeshProUGUI loseText = null;

    [SerializeField] bool isOption = true;

    public GameObject finalSceneUI = null;

    public GameObject[] playerArrangement = new GameObject[6];
    public Transform[] playerPosition = new Transform[6];

    public int curRound = 0;

    #region BattleScene
    public void BattleUIInit()
    {
        battleSceneUI = GameObject.Find("BattleSceneCanvas");
        battleOptionPanel = GameObject.Find("OptionPanel");
        if (battleOptionPanel.activeSelf)
        {
            battleOptionPanel.SetActive(false);
        } 

        battleSceneUI.SetActive(false);
        isOption = battleSceneUI.activeSelf;
    }

    public void OnBattleUI()
    {
        battleSceneUI.SetActive(true);

    }

    public void BattleOption()
    {
        isOption = !isOption;
        battleOptionPanel.SetActive(isOption);
    }

    #endregion


    #region RoundResultScene
    public void ResultUnitPosition()
    {
        int count = 0;

        playerPosition = GameObject.Find("ResultBackGround").GetComponentsInChildren<Transform>();

        foreach (Transform child in playerPosition)
        {
            playerPosition[count] = child;
            count++;
        }
    }

    public void ResultSceneInit()
    {
        ResultSceneUI = GameObject.Find("ResultSceneCanvas");

        winUI = GameObject.Find("ResultWin");
        winText = GameObject.Find("WinRoundText").GetComponent<TextMeshProUGUI>();

        loseUI = GameObject.Find("ResultLose");
        loseText = GameObject.Find("LoseRoundText").GetComponent<TextMeshProUGUI>();

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
            if (playerArrangement[i] != null)
            {
                playerArrangement[i].transform.localScale = new Vector3(2f, 2f, 2f);
                playerArrangement[i].transform.GetChild(0).position = playerPosition[i + 1].position;
            }
        }
    }

    public void ResetPlayerUnit()
    {
        for (int i = 0; i < playerArrangement.Length; i++)
        {
            if (playerArrangement[i] != null)
            {
                // GameMGR.Instance.objectPool.DestroyPrefab(playerArrangement[i]);
                Destroy(playerArrangement[i]);
            }
        }
    }

    public void PlayerBattleWin(bool isWin)
    {
        winText.text = "Round" + curRound;
        winUI.SetActive(isWin);
    }

    public void PlayerBattleLose(bool isWin)
    {
        loseText.text = "Round" + curRound;
        loseUI.SetActive(isWin);
    }
    #endregion

    public IEnumerator COR_MoveToResultScene(bool Win)
    {
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
        yield return new WaitForSeconds(0.1f);
        GameMGR.Instance.spawner.TestButton();

        // Move the camera position to the store scene
        Camera.main.gameObject.transform.position = new Vector3(0, 0, -10);

        if (winUI.activeSelf == true) { winUI.SetActive(false); }
        if (loseUI.activeSelf == true) { loseUI.SetActive(false); }
        if (ResultSceneUI.activeSelf == true) { ResultSceneUI.SetActive(false); }
        GameMGR.Instance.uiManager.storePannel.SetActive(true);
        GameMGR.Instance.Init(5);
    }
}
