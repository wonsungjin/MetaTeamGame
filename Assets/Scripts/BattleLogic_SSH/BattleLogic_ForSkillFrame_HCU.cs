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
    public bool CheckAllDead(GameObject[] array, bool forwardAlive)
    {
        for(int i = 0; i < 6; i++)
        {
            if (i >= 3) forwardAlive = false; // 3번까지 검사했는데 리턴이 안됬다면 전열 전멸이다.
            if (array[i] != null)
            {
                if (i < 3) forwardAlive = true; // 뭔가 있는게 걸렸는데 인덱스가 3보다 작다면 전열 존재
                return true;
            }
        }
        return false;
    }

    public GameObject FindTarget(bool isForwardAlive, GameObject[] other)
    {
        if(isForwardAlive) // 상대 전열이 살아있는 경우
        {
            while(other[exArray[countNum]] == null) // 대상을 찾을 때 까지
            {
                if (exArray[countNum] >= 3) // 전열 살아있는데 뒷열 지정이면 앞열로 변경
                    exArray[countNum] -= 3;
                if (other[exArray[countNum]] != null) break; // 빼고보니 있다면 찾음
                countNum++;
            }  
        }

        else // 상대 전열이 전멸했다면
        {
            while (other[exArray[countNum]] == null) // 대상을 찾을 때 까지
            {
                if (exArray[countNum] < 3) // 전열 살아있는데 뒷열 지정이면 앞열로 변경
                    exArray[countNum] += 3;
                if (other[exArray[countNum]] != null) break; // 더하고보니 있다면 찾음
                countNum++;
            }
        }

        return other[exArray[countNum]];
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

            firstForwardAlive = isPlayerPreemptiveAlive;
            secondForwardAlive = isEnemyPreemptiveAlive;
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

            firstForwardAlive = isEnemyPreemptiveAlive;
            secondForwardAlive = isPlayerPreemptiveAlive;
        }

        bool eventExist = GameMGR.Instance.Event_BattleStart();   // 전투 시작시 스킬 발동 지점
        if (eventExist) yield return new WaitForSeconds(1f); // 스킬이 있다면 스킬 발동 시간 대기 (수정 필요)

        int myNum = 0;  // 내 인덱스 저장값
        int enemyNum = 0;   // 상대 인덱스 저장값

        // 내 차례 지정
        while (true)
        {
            if (firstArray[myNum] == null)  // 차례인 유닛이 없으면 다음 찾는다
            {
                if (myNum > 5) myNum = 0;
                for (int i = myNum; i < 6; i++)
                {
                    if (firstArray[myNum] != null) break;
                    myNum++;
                }
            }

            if (!CheckAllDead(secondArray, secondForwardAlive)) // 전열죽었는데 다죽었다면 내가 이긴다. 이기고 지는 함수도 나인지 적인지 구분할 인자 필요
            {
                PlayerBattleWin();
                yield break;
            }

            else // 적이 남아있다
            {
                isWaitAttack = false;
                // firstArray[i] 가 FindTarget(secondForwardAlive, secondArray) 로 찾은 적을 공격
                firstArray[myNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(secondForwardAlive, secondArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

            }

            // ========== 다른 쪽 턴 =============

            if (secondArray[enemyNum] == null)  // 차례인 유닛이 없으면 다음 찾는다
            {
                if (enemyNum > 5) enemyNum = 0;
                for (int i = enemyNum; i < 6; i++)
                {
                    if (firstArray[enemyNum] != null) break;
                    enemyNum++;
                }
            }

            if (!CheckAllDead(firstArray, firstForwardAlive)) // 전열죽었는데 다죽었다면 내가 이긴다. 이기고 지는 함수도 나인지 적인지 구분할 인자 필요
            {
                PlayerBattleLose();
                yield break;
            }

            else // 적이 남아있다
            {
                isWaitAttack = false;
                // firstArray[i] 가 FindTarget(secondForwardAlive, secondArray) 로 찾은 적을 공격
                secondArray[enemyNum].GetComponentInChildren<AttackLogic>().UnitAttack(FindTarget(firstForwardAlive, firstArray));
                yield return new WaitUntil(() => isWaitAttack);
                isWaitAttack = false;

                if (playerAttackArray.All(x => x == null) && enemyAttackArray.All(x => x == null)) { PlayerBattleDraw(); yield break; }

            }
        }
    }
       
 
}
