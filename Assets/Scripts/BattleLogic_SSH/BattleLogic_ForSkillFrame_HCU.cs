using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public partial class BattleLogic : MonoBehaviourPunCallbacks
{
    public GameObject[] firstForward = null;
    public GameObject[] firstBackward = null;
    public GameObject[] secondForward = null;
    public GameObject[] secondBackward = null;

    public GameObject[] firstArray = new GameObject[6];
    public GameObject[] secondArray = new GameObject[6];

    public bool firstForwardAlive = true;
    public bool secondForwardAlive = true;

    int countNum = 0; // 랜덤 배열의 인덱스 카운팅 용도

    public bool firstAttackTime = false;
    public bool secondAttackTime = false;

    [SerializeField] int firstNum = 0;  // 내 인덱스 저장값
    [SerializeField] int secondNum = 0;   // 상대 인덱스 저장값

    private void Start2()
    {
        Init2();
        PhotonNetwork.FetchServerTimestamp();
    }

    private void Init2()
    {
        exArray = new int[200];
        playerForwardUnits = new GameObject[3];
        playerBackwardUnits = new GameObject[3];
        enemyForwardUnits = new GameObject[3];
        enemyBackwardUnits = new GameObject[3];
    }

    // 상대방 전멸 + 전열 유무 체크
    public bool CheckAllDead(GameObject[] array, bool isArray = true, int forwardAliveNum = 0)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i >= 3) 
            {
                if (isArray) // array 전열 기준이라면
                {
                    if (forwardAliveNum == 1)
                        firstForwardAlive = false;// 3번까지 검사했는데 리턴이 안됬다면 전열 전멸이다.
                    else if (forwardAliveNum == 2)
                        secondForwardAlive = false;
                }
            }
            
            if (array[i] != null)
            {
                if (i < 3)
                {
                    if (isArray) // array 전열 기준이라면
                    {
                        if (forwardAliveNum == 1)
                            firstForwardAlive = true;  // 뭔가 있는게 걸렸는데 인덱스가 3보다 작다면 전열 존재
                        else if (forwardAliveNum == 2)
                            secondForwardAlive = true;
                    }
                }
                return true;    // 살아있다.
            }
        }
        return false;
    }

    public GameObject FindTarget(ref bool isForwardAlive, GameObject[] other)
    {
        if(isForwardAlive) // 상대 전열이 살아있는 경우
        {
            Debug.Log("상대 전열은 살아있다고 보이는 부분이다 : " + isForwardAlive);
            while (other[exArray[countNum]] == null) // 대상을 찾을 때 까지
            {
                Debug.Log(countNum);
                Debug.Log(exArray[countNum]);
                if (exArray[countNum] >= 3) // 전열 살아있는데 뒷열 지정이면 앞열로 변경
                    exArray[countNum] -= 3;

                Debug.Log(exArray[countNum] + " : 3 넘어서 3 뺀 값");
                if (other[exArray[countNum]] != null)   return other[exArray[countNum]];
                else
                    countNum++;
                if (countNum >= 200) Debug.Log("200");
            }  
        }

        else // 상대 전열이 전멸했다면
        {
            Debug.Log("상대 전열은 살아있다고 보이는 부분이다 : " + isForwardAlive);
            while (other[exArray[countNum]] == null) // 대상을 찾을 때 까지
            {
                Debug.Log(countNum);
                Debug.Log(exArray[countNum]);
                if (exArray[countNum] < 3) // 전열 살아있는데 뒷열 지정이면 앞열로 변경
                    exArray[countNum] += 3;
                if (other[exArray[countNum]] != null) return other[exArray[countNum]];  // 더하고보니 있다면 찾음
                else
                    countNum++;
                if (countNum >= 200) Debug.Log("200");
            }
        }

        return other[exArray[countNum]];
    }

    void FindAttacker(GameObject[] array, ref int num, ref bool attackTime, bool myforwardAlive, GameObject[] me = null)
    {
        if (num > 5) num = 0;
        if (array[num] == null)  // 차례인 유닛이 없으면 다음 찾는다
        {
            for (int i = num; i < 6; i++)
            {
                if (array[i] != null)
                {
                    attackTime = true;
                    num = i;
                    return;
                }
            }

            //if (array.All(x => x == null)) { PlayerBattleWin(); }
            //else
            if (array[num] == null)
            {
                if (num > 5) num = 0;
                if (array[num] == null)
                {
                    for (int i = num; i < 6; i++)
                    {
                        if (array[i] != null)
                        {
                            attackTime = true;
                            num = i;
                            return;
                        }
                    }
                }
                else
                    attackTime = true;
            }
        }
        else
            attackTime = true;
    }

    IEnumerator InBattleLogic(bool playerFirst)
    {
        AliveUnit();

        // 플레이어가 선공권이면 player first
        if (playerFirst)
        {
            firstArray = playerAttackArray;
            firstForward = playerForwardUnits;
            firstBackward = playerBackwardUnits;

            secondArray = enemyAttackArray;
            secondForward = enemyForwardUnits;
            secondBackward = enemyBackwardUnits;

        }
        // 상대가 선공이면 enemy first
        else
        {
            firstArray = enemyAttackArray;
            firstForward = enemyForwardUnits;
            firstBackward = enemyBackwardUnits;

            secondArray = playerAttackArray;
            secondForward = playerForwardUnits;
            secondBackward = playerBackwardUnits;

        }

        // 공격 순서 초기화
        firstNum = 0;
        secondNum = 0;

        bool eventExist = GameMGR.Instance.Event_BattleStart();   // 전투 시작시 스킬 발동 지점
        if (eventExist) yield return new WaitForSeconds(1f); // 스킬이 있다면 스킬 발동 시간 대기 (수정 필요)

        while(true)
        {
            FindAttacker(firstArray, ref firstNum, ref firstAttackTime, firstForwardAlive, firstArray);

            if (!CheckAllDead(firstArray, true, 1)) // 대상 찾고 나서 내 덱을 보니 비어있다면 내가 졌다.
            {
                PlayerBattleLose();
                yield break;
            }

            if (!CheckAllDead(secondArray, true, 2)) // 전열죽었는데 다죽었다면 내가 이긴다. 이기고 지는 함수도 나인지 적인지 구분할 인자 필요
            {
                PlayerBattleWin();
                yield break;
            }

            else // 적이 남아있다
            {
                isWaitAttack = false;

                FindAttacker(firstArray, ref firstNum, ref firstAttackTime, firstForwardAlive, firstArray);

                yield return new WaitUntil(() => firstAttackTime);

                Debug.Log(firstArray[firstNum].name);
                // firstArray[i] 가 FindTarget(secondForwardAlive, secondArray) 로 찾은 적을 공격
                firstArray[firstNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(ref secondForwardAlive, secondArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;
                firstAttackTime = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

                firstNum++;

            }

            // ========== 다른 쪽 턴 =============

            FindAttacker(secondArray, ref secondNum, ref secondAttackTime, secondForwardAlive, secondArray);

            if (!CheckAllDead(secondArray, true, 2))
            {
                PlayerBattleWin(); // 선공이 이긴다.
                yield break; 
            }


            if (!CheckAllDead(firstArray, true, 1)) // 전열죽었는데 다죽었다면 내가 이긴다. 이기고 지는 함수도 나인지 적인지 구분할 인자 필요
            {
                PlayerBattleLose();
                yield break;
            }


            else // 적이 남아있다
            {
                isWaitAttack = false;

                FindAttacker(secondArray, ref secondNum, ref secondAttackTime, secondForwardAlive, secondArray);

                yield return new WaitUntil(() => secondAttackTime);

                Debug.Log(secondArray[secondNum].name);  // 갑자기 여기서 널이 떠버린다.
                // firstArray[i] 가 FindTarget(secondForwardAlive, secondArray) 로 찾은 적을 공격
                secondArray[secondNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(ref firstForwardAlive, firstArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;
                secondAttackTime = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

                secondNum++;
            }
            

        }

    }

    
       
 
}
