using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    public GameObject[] playerForwardUnits = null;
    public GameObject[] playerBackwardUnits = null;
    public GameObject[] enemyForwardUnits = null;
    public GameObject[] enemyBackwardUnits = null;

    public GameObject[] playerUnits = new GameObject[6]; // player attack unit
    public GameObject[] enemyUnits = new GameObject[6]; // enemy atack unit
    public List<GameObject> playerAttackList = new List<GameObject>();
    public List<GameObject> enemyAttackList = new List<GameObject>();


    public bool isFirstAttack = true; // isFirstAttack = true => Player fisrt attack
    public bool isWaitAttack = false; // wait for attack
    private bool isDraw = false;


    private int randomArrayNum = 0;


    public int curLife = 20;

    public int[] exArray = new int[200];


    public delegate IEnumerator WaitCOR(GameObject targetUnit);

    private void Start()
    {
        Init();
        PhotonNetwork.FetchServerTimestamp();
    }

    private void Init()
    {
        exArray = new int[200];
        playerForwardUnits = new GameObject[3];
        playerBackwardUnits = new GameObject[3];
        enemyForwardUnits = new GameObject[3];
        enemyBackwardUnits = new GameObject[3];
    }

    #region PlayerList �ʱ�ȭ
    public void InitPlayerList()
    {
        playerAttackList.Clear();
        for (int i = 0; i < playerForwardUnits.Length; i++) { playerUnits[i] = playerForwardUnits[i]; if (playerForwardUnits[i] != null) playerAttackList.Add(playerForwardUnits[i]); }
        for (int i = 0; i < playerBackwardUnits.Length; i++) { playerUnits[i + 3] = playerBackwardUnits[i]; if (playerBackwardUnits[i] != null) playerAttackList.Add(playerBackwardUnits[i]); }
    }
    #endregion

    #region EnemyList
    public void InitEnemyList()
    {
        enemyAttackList.Clear();
        for (int i = 0; i < enemyForwardUnits.Length; i++)
        {
            enemyUnits[i] = enemyForwardUnits[i];
            if (enemyForwardUnits[i] != null) enemyAttackList.Add(enemyForwardUnits[i]);
        }
        for (int i = 0; i < enemyBackwardUnits.Length; i++) { enemyUnits[i + 3] = enemyBackwardUnits[i]; if (enemyBackwardUnits[i] != null) enemyAttackList.Add(enemyBackwardUnits[i]); }
    }
    #endregion

    #region 
    // 시작시 실행되는 부분
    public void AttackLogic()
    {
        isDraw = false;

        Debug.Log("AttackLogic : " + isFirstAttack);
        isWaitAttack = false;
        StartCoroutine(BattleStart(isFirstAttack));
    }
    #endregion
    public GameObject PlayerAttackUnit()
    {
        if (playerAttackList.Count == 0) return null;

        return playerAttackList[0];
    }
    public GameObject EnemyAttackUnit()
    {
        if (enemyAttackList.Count == 0) return null;

        return enemyAttackList[0];
    }
    int count;
    public GameObject PlayerUnit()
    {
        int ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        GameObject retunrUnit = null;
        while (playerForwardUnits[ran] == null)
        {
            Debug.Log(ran);
            count++;
            if (count > 1000) { Debug.Log("무한반복~"); count = 0; break; }
            if (playerForwardUnits[0] == null && playerForwardUnits[1] == null && playerForwardUnits[2] == null) break;
            ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        }
        retunrUnit = playerForwardUnits[ran];
        if (retunrUnit != null) return retunrUnit;
        while (playerBackwardUnits[ran] == null)
        {
            Debug.Log(ran);
            count++;
            if (count > 1000) { Debug.Log("무한반복~"); count = 0; break; }
            if (playerBackwardUnits[0] == null && playerBackwardUnits[1] == null && playerBackwardUnits[2] == null) break;
            ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        }
        retunrUnit = playerBackwardUnits[ran];
        return retunrUnit;
    }
    public GameObject EnemyUnit()
    {
        int ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        GameObject retunrUnit = null;
        while (enemyForwardUnits[ran] == null)
        {
            Debug.Log(ran);

            count++;
            if (count > 1000) { Debug.Log("무한반복~"); count = 0; break; }
            if (enemyForwardUnits[0] == null && enemyForwardUnits[1] == null && enemyForwardUnits[2] == null) break;
            ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        }
        retunrUnit = enemyForwardUnits[ran];
        if (retunrUnit != null) return retunrUnit;
        while (enemyBackwardUnits[ran] == null)
        {
            Debug.Log(ran);

            count++;
            if (count > 1000) { Debug.Log("무한반복~"); count = 0; break; }
            if (enemyBackwardUnits[0] == null && enemyBackwardUnits[1] == null && enemyBackwardUnits[2] == null) break;
            ran = GameMGR.Instance.GetRandomValue(0, 0, true);
        }
        retunrUnit = enemyBackwardUnits[ran];
        return retunrUnit;
    }
    #region Player PreemptiveAttack
    // Player Attack
    IEnumerator BattleStart(bool first)
    {
        GameMGR.Instance.i = 0;

        bool eventExist = GameMGR.Instance.Event_BattleStart();
        if(eventExist) yield return new WaitForSeconds(1f);
        // enemy backward unit attack possible
        if (first)
            while (PlayerUnit() != null && EnemyUnit() != null)
            {
                if(playerAttackList.Count==0)
                {
                    for (int i = 0; i < playerForwardUnits.Length; i++) if (playerForwardUnits[i] != null) playerAttackList.Add(playerForwardUnits[i]); 
                    for (int i = 0; i < playerBackwardUnits.Length; i++) if (playerBackwardUnits[i] != null) playerAttackList.Add(playerBackwardUnits[i]); 
                }
                if (enemyAttackList.Count == 0)
                {
                    for (int i = 0; i < enemyForwardUnits.Length; i++) if (enemyForwardUnits[i] != null) enemyAttackList.Add(enemyForwardUnits[i]);
                    for (int i = 0; i < enemyBackwardUnits.Length; i++) if (enemyBackwardUnits[i] != null) enemyAttackList.Add(enemyBackwardUnits[i]); 
                }
                Debug.Log(PlayerUnit() + "??" + EnemyUnit());
                if (PlayerAttackUnit() != null)
                {
                    PlayerAttackUnit().GetComponentInChildren<AttackLogic>().UnitAttack(EnemyUnit(),true);
                    yield return new WaitUntil(() => isWaitAttack);
                    isWaitAttack = false;
                    
                }
                yield return new WaitForSeconds(0.5f);
                if (PlayerUnit() == null && EnemyUnit() == null) { PlayerBattleDraw(); yield break; }

 
                if (EnemyAttackUnit() != null)
                {
                    EnemyAttackUnit().GetComponentInChildren<AttackLogic>().UnitAttack(PlayerUnit(),false);
                    yield return new WaitUntil(() => isWaitAttack);
                    isWaitAttack = false;

                }
                yield return new WaitForSeconds(0.5f);

                if (PlayerUnit() == null && EnemyUnit() == null) { PlayerBattleDraw(); yield break; }
            }
        else
            while (PlayerUnit() != null && EnemyUnit() != null)
            {
                
                if (playerAttackList.Count == 0)
                {

                    for (int i = 0; i < playerForwardUnits.Length; i++) {  if (playerForwardUnits[i] != null) {  playerAttackList.Add(playerForwardUnits[i]); } }
                    for (int i = 0; i < playerBackwardUnits.Length; i++) if (playerBackwardUnits[i] != null) playerAttackList.Add(playerBackwardUnits[i]);
                }
                if (enemyAttackList.Count == 0)
                {
                    for (int i = 0; i < enemyForwardUnits.Length; i++) if (enemyForwardUnits[i] != null) enemyAttackList.Add(enemyForwardUnits[i]);
                    for (int i = 0; i < enemyBackwardUnits.Length; i++) if (enemyBackwardUnits[i] != null) enemyAttackList.Add(enemyBackwardUnits[i]);
                }
                Debug.Log(PlayerUnit() + "??" + EnemyUnit());
                if (EnemyAttackUnit() != null)
                {
                    EnemyAttackUnit().GetComponentInChildren<AttackLogic>().UnitAttack(PlayerUnit(),false);
                    yield return new WaitUntil(() => isWaitAttack);
                    isWaitAttack = false;
                   
                }
                yield return new WaitForSeconds(0.5f);
                if (PlayerUnit() == null && EnemyUnit() == null) { PlayerBattleDraw(); yield break; }



                if (PlayerAttackUnit() != null)
                {
                    PlayerAttackUnit().GetComponentInChildren<AttackLogic>().UnitAttack(EnemyUnit(),true);
                    yield return new WaitUntil(() => isWaitAttack);
                    isWaitAttack = false;
                    
                }
                yield return new WaitForSeconds(0.5f);

                if (PlayerUnit() == null && EnemyUnit() == null) { PlayerBattleDraw(); yield break; }

            }

        if (EnemyUnit() == null) { PlayerBattleWin(); yield break; }
        else if (PlayerUnit() == null) { PlayerBattleLose(); yield break; }

    }
    #endregion

    // �¸� ��
    private void PlayerBattleWin()
    {
        if (isDraw == true) return;
        // GameObject.Find("firstAttack").GetComponent<TextMeshProUGUI>().text = isFirstAttack.ToString();
        // GameObject.Find("resultText").GetComponent<TextMeshProUGUI>().text = "win";
        Debug.LogError("Player Win");

        GameMGR.Instance.uiManager.PlayerSetArrangement();
        GameMGR.Instance.Init(4);

        StartCoroutine(GameMGR.Instance.uiManager.COR_MoveToResultScene(true, false, false));
    }

    // �й� ��
    private void PlayerBattleLose()
    {
        // GameObject.Find("firstAttack").GetComponent<TextMeshProUGUI>().text = isFirstAttack.ToString();
        // GameObject.Find("resultText").GetComponent<TextMeshProUGUI>().text = "lose";
        if (isDraw == true) return;
        Debug.LogError("Player Lose");


        curLife -= 10;
        GameMGR.Instance.uiManager.ChangeLife(curLife);

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", curLife } });

        if (curLife == 0)
        {
            GameMGR.Instance.uiManager.battleSceneUI.SetActive(false);
            StartCoroutine(GameMGR.Instance.batch.FinalCardUi());
        }

        else if (curLife >= 1)
        {
            StartCoroutine(GameMGR.Instance.uiManager.COR_MoveToResultScene(false, true, false));

            GameMGR.Instance.uiManager.PlayerSetArrangement();
            GameMGR.Instance.Init(4);
        }
    }

    private void PlayerBattleDraw()
    {
        Debug.LogError("Player Draw");

        isDraw = true;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Life", curLife } });

        GameMGR.Instance.uiManager.PlayerSetArrangement();
        GameMGR.Instance.Init(4);


        StartCoroutine(GameMGR.Instance.uiManager.COR_MoveToResultScene(false, false, true));
    }
}
