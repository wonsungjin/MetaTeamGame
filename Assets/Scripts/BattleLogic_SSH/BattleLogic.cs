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

    List<GameObject> playerAttackList = new List<GameObject>();
    List<GameObject> enemyAttackList = new List<GameObject>();

    bool isPlayerPreemptiveAlive = true; // player 전열 생존 여부
    bool isEnemyPreemptiveAlive = true; // enemy 전열 생존 여부

    bool isFirstAttack = true; // 선공 후공에 따른 bool 변수
    bool isResurrection = true; // 소환 특성에 따른 bool 변수
    int playerTurnCount = 0; // Player Turn Count
    int enemyTurnCount = 0; // Enemy Turn Count
    int randomArrayNum = 0;

    // 추후 master client가 gamemananger에서 생성한 랜덤 배열로 대체 예정 (매 라운드 생성 및 배포)
    int[] exArray = new int[100];
    int randomNum = 0;

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
        }
        for (int i = 0; i < playerBackwardUnits.Count; i++)
        {
            playerAttackList.Add(playerBackwardUnits[i]);
        }

        // enemy 공격리스트 추가
        for (int i = 0; i < enemyForwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyForwardUnits[i]);
        }
        for (int i = 0; i < enemyBackwardUnits.Count; i++)
        {
            enemyAttackList.Add(enemyBackwardUnits[i]);
        }


        /*
                // myPlayer Unit 전열, 후열 순서 지정 (0 ~ 2)
                for (int i = 0; i < UnitNum; i++)
                {
                    playerForwardUnits[i].GetComponent<AttackLogic>().Init(i);
                    playerBackwardUnits[i].GetComponent<AttackLogic>().Init(i);
                }

                // enemyPlayer Unit 전열, 후열 순서 지정 (3 ~ 5)
                for (int i = 0; i < UnitNum; i++)
                {
                    enemyForwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
                    enemyBackwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
                }
        */
    }

    // 인게임 전투 로직
    public void AttackLogic()
    {
        // 내가 선공일 경우
        if (isFirstAttack)
        {
            PreemptiveAttack();
        }

        // 상대방이 선공일 경우
        else if (!isFirstAttack)
        {
            SubordinatedAttack();
        }

        else
        {
            Debug.Log("선공 후공이 정해지지 않음");
        }
        // 전열 후열 전멸 판단 로직
    }

    // Player 선제 공격
    public void PreemptiveAttack()
    {
        while (playerAttackList.Count == 0 || enemyAttackList.Count == 0)
        {
            // 플레이어의 전열이 살아있는 경우
            if (isPlayerPreemptiveAlive)
            {
                if (playerAttackList.Count <= playerTurnCount)
                {
                    // 플레이어 유닛이 적 전열 유닛 중 랜덤한 적 공격
                    playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 유닛을 공격, 전열 리스트에서 삭제
                    enemyForwardUnits.Remove(enemyForwardUnits[exArray[randomArrayNum]]);
                    enemyAttackList.Remove(enemyForwardUnits[exArray[randomArrayNum]]);

                    // 새로운 random num 부여
                    // ex) 플레이어가 적의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 적의 전열이 전멸한 경우
                    if (enemyForwardUnits.Count == 0)
                    {
                        isEnemyPreemptiveAlive = false;
                    }
                }

                else
                {
                    // player 공격 순서 초기화
                    playerTurnCount = 0;
                }

                // 1턴 종료에 따른 턴 변수 증가
                playerTurnCount++;
            }

            // 플레이어의 전열이 전멸한 경우
            else if (!isPlayerPreemptiveAlive)
            {
                // 후열을 공격 가능한 상태로 변경
            }

            // 적의 전열이 살아있는 경우
            if (isEnemyPreemptiveAlive)
            {
                if (enemyAttackList.Count <= enemyTurnCount)
                {
                    // 적 유닛이 플레이어 유닛 중 랜덤한 적 공격
                    enemyAttackList[enemyTurnCount].GetComponent<AttackLogic>().UnitAttack(playerForwardUnits[exArray[randomArrayNum]]);

                    // 피격 받은 유닛을 공격, 전열리스트에서 삭제
                    playerAttackList.Remove(playerForwardUnits[exArray[randomArrayNum]]);
                    playerForwardUnits.Remove(playerForwardUnits[exArray[randomArrayNum]]);

                    // 새로운 random num 부여
                    // ex) 적이 플레이어의 2번째를 공격했을 때 적도 플레이어의 2번째를 공격하기 때문에 다른 random num 부여
                    randomArrayNum++;

                    // 플레이어의 전열이 전멸한 경우
                    if (playerForwardUnits.Count == 0)
                    {
                        isPlayerPreemptiveAlive = false;
                    }
                }

                else
                {
                    // Enemy 공격 순서 초기화
                    enemyTurnCount = 0;
                }

                // 1턴 종료에 따른 턴 변수 증가
                enemyTurnCount++;
            }

            // 적의 전열이 전멸한 경우
            else if(!isEnemyPreemptiveAlive)
            {
                // 후열을 공격가능한 상태로 변경
            }

            else
            {
                Debug.Log("전열/후열 생존여부 확인 필요 rq_SSH");
            }

        }

    }

    // Enemy 선제 공격
    public void SubordinatedAttack()
    {

    }
}
