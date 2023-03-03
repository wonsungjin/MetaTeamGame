using MongoDB.Bson.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using Photon.Pun;

public partial class UIManager : MonoBehaviour
{
    public GameObject battleSceneUI = null;
    GameObject battleOptionPanel = null;
    GameObject leavPanel = null;
    GameObject battleLife = null;

    GameObject ResultSceneUI = null;
    GameObject winUI = null;
    GameObject loseUI = null;
    GameObject drawUI = null;
    GameObject SoundPanel = null;

    public GameObject finalSceneUI = null;
    public GameObject[] playerArrangement = new GameObject[6];

    TextMeshProUGUI winText = null;
    TextMeshProUGUI loseText = null;
    TextMeshProUGUI drawText = null;
    TextMeshProUGUI lifeText = new TextMeshProUGUI();

    public AudioSource BattleBGMAudio = null;
    public AudioSource BattleSFXAudio = null;


    bool isOption = true;

    Sprite changeImage = null;

    public Image[] lifeImage = new Image[20];

    public Transform[] playerPosition = new Transform[6];

    public int curRound = 0;

    Slider SFXSlider = null;
    Slider BGMSlider = null;

    #region BattleScene
    public void BattleUIInit()
    {
        battleSceneUI = GameObject.Find("BattleSceneCanvas");
        battleOptionPanel = GameObject.Find("OptionPanel");
        leavPanel = GameObject.Find("LeavePanel");
        SoundPanel = GameObject.Find("SoundPanel");
        lifeText = GameObject.Find("CurLife").gameObject.GetComponent<TextMeshProUGUI>();
        battleLife = GameObject.Find("BattleLife");

        SFXSlider = GameObject.Find("SFXSlider").gameObject.GetComponent<Slider>();
        BGMSlider = GameObject.Find("BGMSlider").gameObject.GetComponent<Slider>();

        SoundPanel.SetActive(false);
        battleSceneUI.SetActive(false);
        // isOption = battleSceneUI.activeSelf;
    }

    public void OnBattleUI()
    {
        battleSceneUI.SetActive(true);
        BattleBGMAudio = GameMGR.Instance.audioMGR.BattleBGM;
        BattleSFXAudio = GameMGR.Instance.audioMGR.BattleAudio;

        lifeText.text = GameMGR.Instance.battleLogic.curLife.ToString();
        if (battleOptionPanel != null) { battleOptionPanel.SetActive(false); }
        if (leavPanel != null) { leavPanel.SetActive(false); }
    }
    /*
        public void BattleOption()
        {
            isOption = !isOption;
            battleOptionPanel.SetActive(isOption);
        }
    */
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
        SoundPanel.SetActive(false);
        battleSceneUI.SetActive(false);

        ResultSceneUI = GameObject.Find("ResultSceneCanvas");

        winUI = GameObject.Find("ResultWin");
        winText = GameObject.Find("WinRoundText").GetComponent<TextMeshProUGUI>();

        loseUI = GameObject.Find("ResultLose");
        loseText = GameObject.Find("LoseRoundText").GetComponent<TextMeshProUGUI>();

        drawUI = GameObject.Find("ResultDraw");
        drawText = GameObject.Find("DrawRoundText").GetComponent<TextMeshProUGUI>();

        lifeImage = GameObject.Find("Life").GetComponentsInChildren<Image>();

        ResultSceneUI.SetActive(false);
    }

    public void OnResultUI()
    {
        ResultSceneUI.SetActive(true);
        lifeText.text = GameMGR.Instance.battleLogic.curLife.ToString();
    }

    public void PlayerSetArrangement()
    {
        for (int i = 0; i < playerArrangement.Length; i++)
        {
            if (playerArrangement[i] != null)
            {
                playerArrangement[i].transform.localScale = Vector3.one * 2;

                playerArrangement[i].transform.position = playerPosition[i + 1].position;
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

    public void PlayerBattleDraw(bool isDraw)
    {
        drawText.text = "Round" + curRound;
        drawUI.SetActive(isDraw);
    }

    public void ChangeLife(int Life)
    {
        changeImage = Resources.Load<Sprite>($"Sprites/Nomal/Icon_ItemIcon_Skull");
        lifeImage[19 - Life].sprite = changeImage;
    }

    public IEnumerator COR_MoveToResultScene(bool Win, bool Lose, bool Draw)
    {
        Camera.main.gameObject.transform.position = new Vector3(40, 0, -10);

        GameMGR.Instance.audioMGR.BattleSceneBGM(false);

        GameMGR.Instance.uiManager.PlayerBattleWin(Win);
        GameMGR.Instance.uiManager.PlayerBattleLose(Lose);
        GameMGR.Instance.uiManager.PlayerBattleDraw(Draw);

        // win case
        if (Win) { GameMGR.Instance.audioMGR.BattleRoundResult(Win); }

        // lose case
        else if (Lose) { GameMGR.Instance.audioMGR.BattleRoundResult(Lose); }

        // draw case
        else if (Draw) { GameMGR.Instance.audioMGR.BattleRoundResult(Draw); }

        GameMGR.Instance.batch.gameObject.GetPhotonView().RPC("LifeSave", RpcTarget.All, (int)PhotonNetwork.LocalPlayer.CustomProperties["Number"], (int)PhotonNetwork.LocalPlayer.CustomProperties["Life"]);

        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            StartCoroutine(GameMGR.Instance.batch.FinalCardUi());
        }
        else
        {

            GameMGR.Instance.uiManager.ResetPlayerUnit(); // Unit Reset
            yield return new WaitForSeconds(0.1f);
            GameMGR.Instance.spawner.TestButton();

            // Move the camera position to the store scene
            Camera.main.gameObject.transform.position = new Vector3(0, 0, -10);

            if (winUI.activeSelf == true) { winUI.SetActive(false); }
            if (loseUI.activeSelf == true) { loseUI.SetActive(false); }
            if (drawUI.activeSelf == true) { drawUI.SetActive(false); }
            if (ResultSceneUI.activeSelf == true) { ResultSceneUI.SetActive(false); }
            GameMGR.Instance.uiManager.storePannel.SetActive(true);
            GameMGR.Instance.uiManager.hpTXT.text = lifeText.text;

            GameMGR.Instance.Init(5);
        }
    }

    // Exit program
    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void BattleSceneVolumeManager()
    {
        BattleBGMAudio.volume = BGMSlider.value;
        BattleSFXAudio.volume = SFXSlider.value;
    }
}
