using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Unity.VisualScripting;
using System.Data;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    public GameObject[] playerForwardUnits = null; // player 전열
    public GameObject[] playerBackwardUnits = null; // player 후열
    public GameObject[] enemyForwardUnits = null; // enemy 전열
    public GameObject[] enemyBackwardUnits = null; // enemy 후열

    [SerializeField] private GameObject[] playerAttackArray = new GameObject[6]; // player attack unit
    [SerializeField] private GameObject[] enemyAttackArray = new GameObject[6]; // enemy atack unit

    private bool isPlayerPreemptiveAlive = true; // player 전열 생존 여부
    private bool isEnemyPreemptiveAlive = true; // enemy 전열 생존 여부
    private bool isFirstAttack = true; // 선공 후공에 따른 bool 변수 => true : Player 선공
    private bool isResurrection = true; // 소환 특성에 따른 bool 변수

    private int playerTurnCount = 0; // Player Turn Count
    private int enemyTurnCount = 0; // Enemy Turn Count
    private int randomArrayNum = 0;
    private int isPlayerAliveCount = 0;
    private int isEnemyAliveCount = 0;

    private int[] exArray = new int[100];

    private int playerCurRound = 0;
    private int enemyCurRound = 0;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        playerForwardUnits = new GameObject[3];
        playerBackwardUnits = new GameObject[3];
        enemyForwardUnits = new GameObject[3];
        enemyBackwardUnits = new GameObject[3];
    }

    #region PlayerList 초기화
    public void InitPlayerList()
    {
        // player 공격리스트 추가
        for (int i = 0; i < playerForwardUnits.Length; i++) { playerAttackArray[i] = playerForwardUnits[i]; }
        for (int i = 0; i < playerBackwardUnits.Length; i++) { playerAttackArray[i + 3] = playerBackwardUnits[i]; }
    }
    #endregion

    #region EnemyList 초기화
    public void InitEnemyList()
    {
        // enemy 공격리스트 추가
        for (int i = 0; i < enemyForwardUnits.Length; i++) { enemyAttackArray[i] = enemyForwardUnits[i]; }
        for (int i = 0; i < enemyBackwardUnits.Length; i++) { enemyAttackArray[i + 3] = enemyBackwardUnits[i]; }
    }
    #endregion

    #region 전투 로직 
    // 인게임 전투 로직
    public void AttackLogic()
    {
        // 내가 선공일 경우
        if (isFirstAttack) { PreemptiveAttack(); }
        // 상대방이 선공일 경우
        //else if (!isFirstAttack) { SubordinatedAttack(); }
        else { Debug.Log("선공 후공이 정해지지 않음"); }
    }
    #endregion

    public void AliveUnit()
    {
        // player forward unit이 없을 경우
        for (int i = 0; i < playerForwardUnits.Length; i++)
        {
            if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
        }

        if (isPlayerAliveCount == playerForwardUnits.Length) { isPlayerPreemptiveAlive = false; }

        // enemy forward unit이 없을 경우
        for (int i = 0; i < enemyForwardUnits.Length; i++)
        {
            if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
        }

        if (isEnemyAliveCount == enemyForwardUnits.Length) { isEnemyPreemptiveAlive = false; }

        isPlayerAliveCount = 0;
        isEnemyAliveCount = 0;
    }


    #region Player 선제 공격
    // Player 선제 공격
    public void PreemptiveAttack()
    {
        // 전열 배치여부 확인
        AliveUnit();

        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++) exArray[i] = GameMGR.Instance.randomValue[i];
        Debug.Log("player 선공");

        while (true)
        {
            Debug.Log("Player가 공격 시작");
            // 랜덤 수를 가지고 있는 배열 1바퀴 돌았을 때 0번째로 초기화
            if (randomArrayNum == exArray.Length) { randomArrayNum = 0; }
            // [Player -> Enemy Attack] 적의 전열이 살아있는 경우
            if (isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // 피격 가능한 적이 나올때까지 randomArray 순회
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isEnemyAliveCount = 0;

                    for (int i = 0; i < enemyForwardUnits.Length; i++)
                    {
                        if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }
                        if (isEnemyAliveCount == enemyForwardUnits.Length) 
                        {
                            isEnemyAliveCount = 0;
                            isEnemyPreemptiveAlive = false; 
                        }
                    }
                    isEnemyAliveCount = 0;
                    if (isEnemyPreemptiveAlive == false) { break; }
                }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // 공격 가능한 플레이어가 나올때 까지 playerturnCount 증가
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        Debug.Log("공격가능한 player unit이 없음");
                        break;
                    }
                }

                Debug.Log("Player Attack Unit name : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("Enemy forward hit unit : " + enemyForwardUnits[exArray[randomArrayNum]].name);
                // 플레이어 유닛이 적 전열 유닛 랜덤 공격
                playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 배열에서 삭제
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyForwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyForwardUnits[exArray[randomArrayNum]] = null;

                        break;
                    }
                    else { Debug.Log("enemyAttackArray 탐색중"); }
                }

                for (int i = 0; i < enemyForwardUnits.Length; i++)
                {
                    if (enemyForwardUnits[i] == null)
                    {
                        isEnemyAliveCount++;
                    }
                }
                if (enemyForwardUnits.Length == isEnemyAliveCount)
                {
                    isEnemyAliveCount = 0;
                    isEnemyPreemptiveAlive = false;
                }

                isEnemyAliveCount = 0;

                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                // 1턴 종료에 따른 턴 변수 증가 
                playerTurnCount++;
            }

            // 적의 전열이 전멸한 경우
            // 후열을 공격 가능한 상태로 변경
            else if (!isEnemyPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (enemyBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                // 공격 가능한 플레이어가 나올때 까지 playerTurnCount 증가
                while (playerAttackArray[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerAttackArray.Length <= playerTurnCount) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackArray.Length; i++)
                    {
                        if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackArray.Length)
                    {
                        isPlayerAliveCount = 0;
                        Debug.Log("공격가능한 player unit이 없음");
                        break;
                    }
                }

                Debug.Log("playerTurnCount : " + playerTurnCount);
                Debug.Log("player attack unit : " + playerAttackArray[playerTurnCount].name);
                Debug.Log("enemy backward hit unit : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                // 플레이어 유닛이 적 후열 유닛 랜덤 공격
                playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 배열에서 삭제
                for (int i = 0; i < enemyAttackArray.Length; i++)
                {
                    if (enemyAttackArray[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit enemy unit (attackArray) : " + enemyAttackArray[i].name);
                        Debug.Log("Hit enemy unit (forwardUnit) : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                        enemyAttackArray[i] = null;
                        enemyBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("enemyAttackArray 탐색중"); }
                }

                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                // 1턴 종료에 따른 턴 변수 증가 
                playerTurnCount++;
            }

            // 적이 전멸한 경우
            for (int i = 0; i < enemyAttackArray.Length; i++)
            {
                if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
            }
            if (isEnemyAliveCount == enemyAttackArray.Length)
            {
                Debug.Log("2. isEnemyAliveCount : " + isEnemyAliveCount);
                isEnemyAliveCount = 0;
                PlayerBattleWin();
                break;
            }

            isEnemyAliveCount = 0;

            // [Enemy -> Player Attack] 플레이어의 전열이 살아있는 경우
            if (isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                // 피격 가능한 Player가 나올때까지 랜덤 수 를 받음
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isPlayerAliveCount = 0;

                    for (int i = 0; i < playerForwardUnits.Length; i++)
                    {
                        if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerForwardUnits.Length)
                    {
                        isPlayerAliveCount = 0;
                        isPlayerPreemptiveAlive = false;
                    }

                    if (isPlayerPreemptiveAlive == false) { break; }
                }

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    isEnemyAliveCount = 0;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                    for (int i = 0; i < enemyAttackArray.Length; i++)
                    {
                        if (enemyAttackArray[i] == null) { isEnemyAliveCount++; }
                    }

                    if (isEnemyAliveCount == enemyAttackArray.Length)
                    {
                        isEnemyAliveCount = 0;
                        Debug.Log("공격가능한 Enemy uint이 없음");
                        break;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player forward hit Unit : " + playerForwardUnits[exArray[randomArrayNum]].name);

                // 적 유닛이 플레이어 유닛 중 랜덤한 플레이어 공격
                enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerForwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerForwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerForwardUnits 탐색중"); }
                }

                for (int i = 0; i < playerForwardUnits.Length; i++)
                {
                    if (playerForwardUnits[i] == null)
                    {
                        isPlayerAliveCount++;
                    }
                }
                if (playerForwardUnits.Length == isPlayerAliveCount)
                {
                    isPlayerAliveCount = 0;
                    isPlayerPreemptiveAlive = false;
                }

                // 새로운 random num 부여
                // ex) 적이 플레이어의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;


                // 1턴 종료에 따른 턴 변수 증가
                enemyTurnCount++;
            }

            // 플레이어의 전열이 전멸한 경우
            // 후열을 공격 가능한 상태로 변경
            else if (!isPlayerPreemptiveAlive)
            {
                isPlayerAliveCount = 0;
                isEnemyAliveCount = 0;

                while (playerBackwardUnits[exArray[randomArrayNum]] == null) { randomArrayNum++; }

                if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }

                while (enemyAttackArray[enemyTurnCount] == null)
                {
                    enemyTurnCount++;

                    if (enemyAttackArray.Length <= enemyTurnCount) { enemyTurnCount = 0; }
                }

                Debug.Log("enemy attack unit : " + enemyAttackArray[enemyTurnCount].name);
                Debug.Log("player backward hit unit : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                // 적 유닛이 플레이어 후열 유닛 랜덤 공격
                enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 플레이어 유닛을 배열에서 삭제
                for (int i = 0; i < playerAttackArray.Length; i++)
                {
                    if (playerAttackArray[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        Debug.Log("Hit player unit (attackArray) : " + playerAttackArray[i].name);
                        Debug.Log("Hit player unit (forwardUnit) : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                        playerAttackArray[i] = null;
                        playerBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }
                    else { Debug.Log("playerAttackArray 탐색중"); }
                }


                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                isPlayerAliveCount = 0;

                // 1턴 종료에 따른 턴 변수 증가 
                enemyTurnCount++;
                // enemy 공격 순서 초기화
                enemyTurnCount = 0;
            }

            // 플레이어 유닛이 전멸한 경우
            for (int i = 0; i < playerAttackArray.Length; i++)
            {
                if (playerAttackArray[i] == null) { isPlayerAliveCount++; }
                if (isPlayerAliveCount == playerAttackArray.Length)
                {
                    isPlayerAliveCount = 0;
                    PlayerBattleLose();
                    break;
                }
            }
            isPlayerAliveCount = 0;
        }

    }
    #endregion

    #region Enemy 선제 공격
    /*
        // Enemy 선제 공격
        public void SubordinatedAttack()
        {
            // 전열 배치여부 확인
            AliveUnit();


            Debug.Log("Enemy 선공");

            while (playerAttackArray.Count != 0 || enemyAttackArray.Count != 0)
            {
                Debug.Log("공격 시작");

                // 랜덤 수를 가지고 있는 배열 1바퀴 돌았을 때 0번째로 초기화
                if (randomArrayNum == exArray.Length)
                {
                    randomArrayNum = 0;
                }

                // [Enemy -> Player Attack] 플레이어의 전열이 살아있는 경우
                if (isPlayerPreemptiveAlive)
                {
                    while (playerForwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    if (enemyAttackArray.Count == enemyTurnCount)
                    {
                        enemyTurnCount = 0;
                    }

                    else if (enemyAttackArray.Count != enemyTurnCount)
                    {
                        while (enemyAttackArray[enemyTurnCount] == null)
                        {
                            enemyTurnCount++;
                        }
                    }

                    else
                    {
                        Debug.Log("enemyTurnCount 확인 필요");
                    }

                    // 적 유닛이 플레이어 유닛 중 랜덤한 플레이어 공격
                    enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 유닛을 공격 리스트에서 삭제
                    for (int i = 0; i < playerAttackArray.Count; i++)
                    {
                        if (playerAttackArray[i] == playerForwardUnits[exArray[randomArrayNum]])
                        {
                            playerAttackArray[i] = null;
                            break;
                        }

                        else
                        {
                            Debug.Log("playerForwardUnits 탐색중");
                        }
                    }

                    playerForwardUnits[exArray[randomArrayNum]] = null;

                    // 새로운 random num 부여
                    // ex) 적이 플레이어의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 플레이어의 전열이 전멸한 경우
                    if (playerForwardUnits[0] == null && playerForwardUnits[1] == null && playerForwardUnits[2] == null)
                    {
                        isPlayerPreemptiveAlive = false;
                    }

                    // 1턴 종료에 따른 턴 변수 증가
                    enemyTurnCount++;
                }

                // 플레이어의 전열이 전멸한 경우
                else if (!isPlayerPreemptiveAlive)
                {
                    // 후열을 공격 가능한 상태로 변경

                    while (playerBackwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    if (enemyTurnCount > enemyAttackArray.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }

                    Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                    while (enemyAttackArray[enemyTurnCount] == null)
                    {
                        enemyTurnCount++;
                        if (enemyTurnCount > enemyAttackArray.Count - 1)
                        {
                            enemyTurnCount = 0;
                        }
                        Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                    }

                    // 적 유닛이 플레이어 후열 유닛 랜덤 공격
                    enemyAttackArray[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 플레이어 유닛을 공격 리스트에서 삭제
                    for (int i = 0; i < playerAttackArray.Count; i++)
                    {
                        if (playerAttackArray[i] == playerBackwardUnits[exArray[randomArrayNum]])
                        {
                            playerAttackArray[i] = null;
                            Debug.Log(playerBackwardUnits[exArray[randomArrayNum]] + " : null");
                            break;
                        }

                        else
                        {
                            Debug.Log("playerAttackArray 탐색중");
                        }
                    }

                    playerBackwardUnits[exArray[randomArrayNum]] = null;

                    // 새로운 random num 부여
                    // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 플레이어의 후열이 전멸한 경우
                    if (playerBackwardUnits[0] == null && playerBackwardUnits[1] == null && playerBackwardUnits[2] == null)
                    {
                        // 플레이어 패배
                        PlayerBattleLose();
                        break;
                    }

                    // 1턴 종료에 따른 턴 변수 증가 
                    enemyTurnCount++;
                    // player 공격 순서 초기화
                    enemyTurnCount = 0;
                }

                else
                {
                    Debug.Log("전열/후열 생존여부 확인 필요 rq_SSH");
                }

                // [Player -> Enemy Attack] 적의 전열이 살아있는 경우
                if (isEnemyPreemptiveAlive)
                {
                    // 피격 가능한 적이 나올때까지 randomArray 순회
                    while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    // 공격 가능한 플레이어가 나올때 까지 playerturnCount 증가
                    while (playerAttackArray[playerTurnCount] == null)
                    {
                        playerTurnCount++;
                    }

                    // 플레이어 유닛이 적 전열 유닛 랜덤 공격
                    playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 유닛을 공격 리스트에서 삭제
                    for (int i = 0; i < enemyAttackArray.Count; i++)
                    {
                        if (enemyAttackArray[i] == enemyForwardUnits[exArray[randomArrayNum]])
                        {
                            enemyAttackArray[i] = null;
                            break;
                        }
                        else
                        {
                            Debug.Log("enemyAttackArray 탐색중");
                        }
                    }

                    // 피격 받은 유닛을 전열 리스트에서 삭제
                    enemyForwardUnits[exArray[randomArrayNum]] = null;

                    // 새로운 random num 부여
                    // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 적의 전열이 전멸한 경우
                    if (enemyForwardUnits[0] == null && enemyForwardUnits[1] == null && enemyForwardUnits[2] == null)
                    {
                        isEnemyPreemptiveAlive = false;
                    }

                    // 1턴 종료에 따른 턴 변수 증가 
                    playerTurnCount++;
                }

                // 적의 전열이 전멸한 경우
                else if (!isEnemyPreemptiveAlive)
                {
                    // 후열을 공격 가능한 상태로 변경
                    while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    if (playerAttackArray.Count - 1 > playerTurnCount)
                    {
                        playerTurnCount = 0;
                    }

                    Debug.Log("***playerTurnCount : " + playerTurnCount);
                    // 공격 가능한 플레이어가 나올때 까지 playerTurnCount 증가
                    while (playerAttackArray[playerTurnCount] == null)
                    {
                        playerTurnCount++;
                        if (playerTurnCount > playerAttackArray.Count - 1)
                        {
                            playerTurnCount = 0;
                        }
                        Debug.Log("***playerTurnCount : " + playerTurnCount);
                    }

                    // 플레이어 유닛이 적 후열 유닛 랜덤 공격
                    playerAttackArray[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 유닛을 공격 리스트에서 삭제
                    for (int i = 0; i < enemyAttackArray.Count; i++)
                    {
                        if (enemyAttackArray[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                        {
                            enemyAttackArray[i] = null;
                            break;
                        }

                        else
                        {
                            Debug.Log("enemyAttackArray 탐색중");
                        }
                    }

                    //피격 받은 유닛을 후열 리스트에서 삭제
                    enemyBackwardUnits[exArray[randomArrayNum]] = null;

                    // 새로운 random num 부여
                    // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 적의 후열이 전멸한 경우
                    if (enemyBackwardUnits[0] == null && enemyBackwardUnits[1] == null && enemyBackwardUnits[2] == null)
                    {
                        // 플레이어 승리
                        PlayerBattleWin();
                        break;
                    }

                    // 1턴 종료에 따른 턴 변수 증가 
                    playerTurnCount++;
                }
            }
        }
    */
    #endregion

    // 승리 시
    private void PlayerBattleWin()
    {
        Debug.Log("Player Win");
        // 승리 로직 추가
    }

    // 패배 시
    private void PlayerBattleLose()
    {
        Debug.Log("Player Lose");
        // 패배 로직 추가
    }
}
