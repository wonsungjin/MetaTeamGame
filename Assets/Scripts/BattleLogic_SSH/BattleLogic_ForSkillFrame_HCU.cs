using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{

    // 스킬 능력 함수들 모음집
    public void ThrowMissile(int value)
    {
        enemyForwardUnits[exArray[randomArrayNum]].GetComponent<Card>().curHP -= value;
    }

    

    #region Player 선제 공격
    // Player 선제 공격
    public void PreemptiveAttack_HCU()
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

                // 공격 시작
                // 투사체 공격 함수 실행

                // 플레이어 유닛이 적 전열 유닛 랜덤 공격
                playerAttackList[playerTurnCount].GetComponent<AttackLogic>().UnitAttack(enemyForwardUnits[exArray[randomArrayNum]]);

                // 공격 전(대상만 체크)


                // 피격 대상 선정
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

                // 피격 받은 유닛의 체력이 0이하로 내려가면
                if(enemyForwardUnits[exArray[randomArrayNum]])
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
        }
    }
    #endregion

    #region Enemy 선제 공격
    // Enemy 선제 공격
    public void SubordinatedAttack_HCU()
    {
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

}
