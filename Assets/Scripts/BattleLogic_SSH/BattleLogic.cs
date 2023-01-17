using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> playerForwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> playerBackwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> enemyForwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> enemyBackwardUnits = new List<GameObject>();
    [SerializeField] List<GameObject> playerAttackList = new List<GameObject>();
    [SerializeField] List<GameObject> enemyAttackList = new List<GameObject>();

    bool isPlayerPreemptiveAlive = true; // player 전열 생존 여부
    bool isEnemyPreemptiveAlive = true; // enemy 전열 생존 여부
    bool isFirstAttack = true; // 선공 후공에 따른 bool 변수 => true : Player 선공
    bool isResurrection = true; // 소환 특성에 따른 bool 변수

    [SerializeField] int playerTurnCount = 0; // Player Turn Count
    int enemyTurnCount = 0; // Enemy Turn Count
    int randomArrayNum = 0;

    // 추후 master client가 gamemananger에서 생성한 랜덤 배열로 대체 예정 (매 라운드 생성 및 배포)
    int[] exArray = new int[100];

    int playerCurRound = 0;
    int enemyCurRound = 0;

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        AttackLogic();
    }

    #region Player, EnemyList 초기화
    public void Init()
    {
        // Master Clinet가 매 라운드마다 생성하는 Random Array
        for (int i = 0; i < exArray.Length; i++)
        {
            exArray[i] = Random.Range(0, 3);
        }

        // player 공격리스트 추가
        for (int i = 0; i < playerForwardUnits.Count; i++)
        {
            playerAttackList.Add(playerForwardUnits[i]);
            playerAttackList.Add(playerBackwardUnits[i]);
        }

        // enemy 공격리스트 추가
        for (int i = 0; i < enemyForwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyForwardUnits[i]);
            enemyAttackList.Add(enemyBackwardUnits[i]);
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

    #region Player 선제 공격
    // Player 선제 공격
    public void PreemptiveAttack()
    {
        Debug.Log("player 선공");

        while (playerAttackList.Count != 0 || enemyAttackList.Count != 0)
        {
            Debug.Log("공격 시작");

            // 랜덤 수를 가지고 있는 배열 1바퀴 돌았을 때 0번째로 초기화
            if (randomArrayNum == exArray.Length)
            {
                randomArrayNum = 0;
            }

            // [Player -> Enemy Attack] 적의 전열이 살아있는 경우
            if (isEnemyPreemptiveAlive)
            {
                if (playerAttackList.Count >= playerTurnCount)
                {
                    while (enemyForwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

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

                else
                {
                    // player 공격 순서 초기화
                    playerTurnCount = 0;
                }
            }

            // 적의 전열이 전멸한 경우
            else if (!isEnemyPreemptiveAlive)
            {
                // 후열을 공격 가능한 상태로 변경
                if (playerAttackList.Count >= playerTurnCount)
                {
                    while (enemyBackwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    if (playerAttackList.Count == playerTurnCount)
                    {
                        playerTurnCount = 0;
                    }

                    while (playerAttackList[playerTurnCount] == null)
                    {
                        if (playerTurnCount >= playerAttackList.Count)
                        {
                            playerTurnCount = 0;
                        }

                        else
                        {
                            playerTurnCount++;
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

                else
                {
                    // player 공격 순서 초기화
                    playerTurnCount = 0;
                }
            }

            // [Enemy -> Player Attack] 플레이어의 전열이 살아있는 경우
            if (isPlayerPreemptiveAlive)
            {
                if (enemyAttackList.Count >= enemyTurnCount)
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

                else
                {
                    // Enemy 공격 순서 초기화
                    enemyTurnCount = 0;
                }
            }

            // 플레이어의 전열이 전멸한 경우
            else if (!isPlayerPreemptiveAlive)
            {
                // 후열을 공격 가능한 상태로 변경
                if (enemyAttackList.Count >= enemyTurnCount)
                {
                    while (playerBackwardUnits[exArray[randomArrayNum]] == null)
                    {
                        randomArrayNum++;
                    }

                    if (enemyAttackList.Count == enemyTurnCount)
                    {
                        enemyTurnCount = 0;
                    }

                    while (enemyAttackList[enemyTurnCount] == null)
                    {
                        if (enemyTurnCount >= enemyAttackList.Count)
                        {
                            enemyTurnCount = 0;
                        }
                        else
                        {
                            enemyTurnCount++;
                        }
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
                }

                else
                {
                    // player 공격 순서 초기화
                    enemyTurnCount = 0;
                }
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
