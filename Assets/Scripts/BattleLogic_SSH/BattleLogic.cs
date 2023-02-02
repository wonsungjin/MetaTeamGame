using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Unity.VisualScripting;
using System.Data;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] public List<GameObject> playerForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> playerBackwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyForwardUnits = new List<GameObject>();
    [SerializeField] public List<GameObject> enemyBackwardUnits = new List<GameObject>();

    [SerializeField] public GameObject[] _playerForwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _playerBackwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _enemyForwardUnits = new GameObject[3];
    [SerializeField] public GameObject[] _enemyBackwardUnits = new GameObject[3];

    [SerializeField] public List<GameObject> playerAttackList = new List<GameObject>(); //외부에서 접근하기 위해 attackList 2개 퍼블릭으로 변경.
    [SerializeField] public List<GameObject> enemyAttackList = new List<GameObject>();

    private bool isPlayerPreemptiveAlive = true; // player 전열 생존 여부
    private bool isEnemyPreemptiveAlive = true; // enemy 전열 생존 여부
    private bool isFirstAttack = true; // 선공 후공에 따른 bool 변수 => true : Player 선공
    private bool isResurrection = true; // 소환 특성에 따른 bool 변수

    private int playerTurnCount = 0; // Player Turn Count
    private int enemyTurnCount = 0; // Enemy Turn Count

    private int randomArrayNum = 0;
    private int isPlayerAliveCount = 0;
    private int isEnemyAliveCount = 0;


    // 추후 master client가 gamemananger에서 생성한 랜덤 배열로 대체 예정 (매 라운드 생성 및 배포)
    private int[] exArray = new int[100];

    private int playerCurRound = 0;
    private int enemyCurRound = 0;

    #region PlayerList 초기화
    public void InitPlayerList()
    {
        Debug.Log("PlayerList 초기화");
        // Master Clinet가 매 라운드마다 생성하는 Random Array

        // player 공격리스트 추가
        if (playerForwardUnits.Count != 0)
        {
            for (int i = 0; i < playerForwardUnits.Count; i++)
            {
                playerAttackList.Add(playerForwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("공격 가능한 Player 전열이 없음");
        }

        if (playerBackwardUnits.Count != 0)
        {
            for (int i = 0; i < playerBackwardUnits.Count; i++)
            {
                playerAttackList.Add(playerBackwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("공격 가능한 Player 후열이 없음");
        }
    }
    #endregion

    #region EnemyList 초기화
    public void InitEnemyList()
    {
        // enemy 공격리스트 추가
        if (enemyForwardUnits.Count != 0)
        {
            for (int i = 0; i < enemyForwardUnits.Count; i++)
            {
                enemyAttackList.Add(enemyForwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("공격가능한 Enemy 전열이 없음");
        }

        if (enemyBackwardUnits.Count != 0)
        {
            for (int i = 0; i < enemyBackwardUnits.Count; i++)
            {
                enemyAttackList.Add(enemyBackwardUnits[i]);
            }
        }

        else
        {
            Debug.Log("공격 가능한 Enemy 후열이 없음");
        }
    }
    #endregion

    #region 전투 로직 
    // 인게임 전투 로직
    public void AttackLogic()
    {
        // 내가 선공일 경우
        if (isFirstAttack) { PreemptiveAttack(); }

        // 상대방이 선공일 경우
        else if (!isFirstAttack) { SubordinatedAttack(); }

        else { Debug.Log("선공 후공이 정해지지 않음"); }
    }
    #endregion

    public void AliveUnit()
    {
        if (playerForwardUnits.Count == 0)
        {
            isPlayerPreemptiveAlive = false;
        }

        if (enemyForwardUnits.Count == 0)
        {
            isEnemyPreemptiveAlive = false;
        }
    }

    #region Player 선제 공격
    // Player 선제 공격
    public void PreemptiveAttack()
    {
        // 전열 배치여부 확인
        AliveUnit();

        for (int i = 0; i < GameMGR.Instance.randomValue.Length; i++) exArray[i] = GameMGR.Instance.randomValue[i];
        Debug.Log("player 선공");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        {
            Debug.Log("Player가 공격 시작");

            // 랜덤 수를 가지고 있는 배열 1바퀴 돌았을 때 0번째로 초기화
            if (randomArrayNum == exArray.Length) { randomArrayNum = 0; }

            // [Player -> Enemy Attack] 적의 전열이 살아있는 경우
            if (isEnemyPreemptiveAlive)
            {
                // 피격 가능한 적이 나올때까지 randomArray 순회
                while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                    isEnemyAliveCount = 0;

                    for (int i = 0; i < enemyForwardUnits.Count; i++)
                    {
                        if (enemyForwardUnits[i] == null) { isEnemyAliveCount++; }

                        if (isEnemyAliveCount == enemyForwardUnits.Count)
                        {
                            isEnemyPreemptiveAlive = false;
                            break;
                        }
                    }

                    if (isEnemyPreemptiveAlive == false)
                    {
                        break;
                    }
                }

                if (playerAttackList.Count - 1 < playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                // 공격 가능한 플레이어가 나올때 까지 playerturnCount 증가
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    isPlayerAliveCount = 0;

                    if (playerTurnCount > playerAttackList.Count - 1) { playerTurnCount = 0; }

                    for (int i = 0; i < playerAttackList.Count; i++)
                    {
                        if (playerAttackList[i] == null) { isPlayerAliveCount++; }
                    }

                    if (isPlayerAliveCount == playerAttackList.Count) { break; }
                }

                Debug.Log("Player Attack Unit name : " + playerAttackList[playerTurnCount].name);
                Debug.Log("Enemy forward hit unit : " + enemyForwardUnits[exArray[randomArrayNum]].name);
                // 플레이어 유닛이 적 전열 유닛 랜덤 공격
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 리스트에서 삭제
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        enemyForwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet Enemy list");
                        break;
                    }
                    else { Debug.Log("enemyAttackList 탐색중"); }
                }

                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                // 적이 전멸한 경우
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == enemyAttackList.Count)
                        {
                            isEnemyAliveCount = 0;
                            BattleWin();
                            break;
                        }
                    }
                }
                isEnemyAliveCount = 0;

                // 1턴 종료에 따른 턴 변수 증가 
                playerTurnCount++;
            }

            // 적의 전열이 전멸한 경우
            // 후열을 공격 가능한 상태로 변경
            else if (!isEnemyPreemptiveAlive)
            {
                while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;
                }

                if (playerAttackList.Count - 1 > playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                // 공격 가능한 플레이어가 나올때 까지 playerTurnCount 증가
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;

                    if (playerTurnCount > playerAttackList.Count - 1)
                    {
                        playerTurnCount = 0;
                    }

                    for (int i = 0; i < playerAttackList.Count; i++)
                    {
                        if (playerAttackList[i] == null)
                        {
                            isPlayerAliveCount++;
                        }
                    }

                    if (isPlayerAliveCount == playerAttackList.Count)
                    {
                        break;
                    }
                }

                Debug.Log("player attack unit : " + playerAttackList[playerTurnCount].name);
                Debug.Log("enemy backward hit unit : " + enemyBackwardUnits[exArray[randomArrayNum]].name);

                // 플레이어 유닛이 적 후열 유닛 랜덤 공격
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 리스트에서 삭제
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        enemyBackwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet enemy unit");
                        break;
                    }

                    else
                    {
                        Debug.Log("enemyAttackList 탐색중");
                    }
                }

                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                // 적이 전멸한 경우
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == enemyAttackList.Count)
                        {
                            isEnemyAliveCount = 0;
                            BattleWin();
                            break;
                        }
                    }
                }

                isEnemyAliveCount = 0;

                // 1턴 종료에 따른 턴 변수 증가 
                playerTurnCount++;
            }

            // [Enemy -> Player Attack] 플레이어의 전열이 살아있는 경우
            if (isPlayerPreemptiveAlive)
            {
                // 피격 가능한 Player가 나올때까지 랜덤 수 를 받음
                while (playerForwardUnits[exArray[randomArrayNum]] == null)
                {
                    randomArrayNum++;

                    isPlayerAliveCount = 0;

                    for (int i = 0; i < playerForwardUnits.Count; i++)
                    {
                        if (playerForwardUnits[i] == null) { isPlayerAliveCount++; }

                        if (isPlayerAliveCount == playerForwardUnits.Count)
                        {
                            isPlayerPreemptiveAlive = false;
                            break;
                        }
                    }

                    if (isEnemyPreemptiveAlive == false)
                    {
                        break;
                    }
                }

                if (enemyAttackList.Count - 1 < enemyTurnCount)
                {
                    enemyTurnCount = 0;
                }

                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    isEnemyAliveCount = 0;

                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }

                    for (int i = 0; i < enemyAttackList.Count; i++)
                    {
                        if (enemyAttackList[i] == null)
                        {
                            isEnemyAliveCount++;
                        }
                    }

                    if (isEnemyAliveCount == enemyAttackList.Count)
                    {
                        break;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackList[enemyTurnCount].name);
                Debug.Log("player forward hit Unit : " + playerForwardUnits[exArray[randomArrayNum]].name);
                // 적 유닛이 플레이어 유닛 중 랜덤한 플레이어 공격
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        playerForwardUnits[exArray[randomArrayNum]] = null;
                        Debug.Log("Delet player unit");
                        break;
                    }

                    else
                    {
                        Debug.Log("playerForwardUnits 탐색중");
                    }
                }

                // 새로운 random num 부여
                // ex) 적이 플레이어의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;

                // 플레이어 유닛이 전멸한 경우
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == playerAttackList.Count)
                        {
                            isPlayerAliveCount = 0;
                            BattleLose();
                            break;
                        }
                    }
                }
                isPlayerAliveCount = 0;

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

                if (enemyTurnCount > enemyAttackList.Count - 1)
                {
                    enemyTurnCount = 0;
                }

                Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }
                }

                Debug.Log("enemy attack unit : " + enemyAttackList[enemyTurnCount].name);
                Debug.Log("player backward hit unit : " + playerBackwardUnits[exArray[randomArrayNum]].name);

                // 적 유닛이 플레이어 후열 유닛 랜덤 공격
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 플레이어 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        playerBackwardUnits[exArray[randomArrayNum]] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("playerAttackList 탐색중");
                    }
                }


                // 새로운 random num 부여
                // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                randomArrayNum++;


                // 플레이어 유닛이 전멸한 경우
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == null)
                    {
                        isPlayerAliveCount++;
                        if (isPlayerAliveCount == playerAttackList.Count)
                        {
                            isPlayerAliveCount = 0;
                            BattleLose();
                            break;
                        }
                    }
                }
                isPlayerAliveCount = 0;

                // 1턴 종료에 따른 턴 변수 증가 
                enemyTurnCount++;
                // player 공격 순서 초기화
                enemyTurnCount = 0;
            }

            else
            {
                Debug.Log("전열/후열 생존여부 확인 필요 rq_SSH");
            }
        }
    }
    #endregion

    #region Enemy 선제 공격
    // Enemy 선제 공격
    public void SubordinatedAttack()
    {
        // 전열 배치여부 확인
        AliveUnit();

        Debug.Log("Enemy 선공");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
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

                if (enemyAttackList.Count == enemyTurnCount)
                {
                    enemyTurnCount = 0;
                }

                else if (enemyAttackList.Count != enemyTurnCount)
                {
                    while (enemyAttackList[enemyTurnCount] == null)
                    {
                        enemyTurnCount++;
                    }
                }

                else
                {
                    Debug.Log("enemyTurnCount 확인 필요");
                }

                // 적 유닛이 플레이어 유닛 중 랜덤한 플레이어 공격
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerForwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
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

                if (enemyTurnCount > enemyAttackList.Count - 1)
                {
                    enemyTurnCount = 0;
                }

                Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                while (enemyAttackList[enemyTurnCount] == null)
                {
                    enemyTurnCount++;
                    if (enemyTurnCount > enemyAttackList.Count - 1)
                    {
                        enemyTurnCount = 0;
                    }
                    Debug.Log("***enemyTurnCount : " + enemyTurnCount);
                }

                // 적 유닛이 플레이어 후열 유닛 랜덤 공격
                enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 플레이어 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < playerAttackList.Count; i++)
                {
                    if (playerAttackList[i] == playerBackwardUnits[exArray[randomArrayNum]])
                    {
                        playerAttackList[i] = null;
                        Debug.Log(playerBackwardUnits[exArray[randomArrayNum]] + " : null");
                        break;
                    }

                    else
                    {
                        Debug.Log("playerAttackList 탐색중");
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
                    BattleLose();
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
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                }

                // 플레이어 유닛이 적 전열 유닛 랜덤 공격
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyForwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }
                    else
                    {
                        Debug.Log("enemyAttackList 탐색중");
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

                if (playerAttackList.Count - 1 > playerTurnCount)
                {
                    playerTurnCount = 0;
                }

                Debug.Log("***playerTurnCount : " + playerTurnCount);
                // 공격 가능한 플레이어가 나올때 까지 playerTurnCount 증가
                while (playerAttackList[playerTurnCount] == null)
                {
                    playerTurnCount++;
                    if (playerTurnCount > playerAttackList.Count - 1)
                    {
                        playerTurnCount = 0;
                    }
                    Debug.Log("***playerTurnCount : " + playerTurnCount);
                }

                // 플레이어 유닛이 적 후열 유닛 랜덤 공격
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyBackwardUnits[exArray[randomArrayNum]]);

                // 피격 받은 유닛을 공격 리스트에서 삭제
                for (int i = 0; i < enemyAttackList.Count; i++)
                {
                    if (enemyAttackList[i] == enemyBackwardUnits[exArray[randomArrayNum]])
                    {
                        enemyAttackList[i] = null;
                        break;
                    }

                    else
                    {
                        Debug.Log("enemyAttackList 탐색중");
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
                    BattleWin();
                    break;
                }

                // 1턴 종료에 따른 턴 변수 증가 
                playerTurnCount++;
            }
        }
    }
    #endregion

    // 승리 시
    private void BattleWin()
    {
        Debug.Log("Player Win");
        // 승리 로직 추가
    }

    // 패배 시
    private void BattleLose()
    {
        Debug.Log("Player Lose");
        // 패배 로직 추가
    }
}
