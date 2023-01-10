using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class BattleLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] myForwardUnits = null;
    [SerializeField] GameObject[] myBackwardUnits = null;
    [SerializeField] GameObject[] enemyForwardUnits = null;
    [SerializeField] GameObject[] enemyBackwardUnits = null;

    int UnitNum = 3; // 전열, 후열의 유닛 수    
    bool isFirstAttack = false; // 선공 후공에 따른 bool 변수
    bool isResurrection = true; // 소환 특성에 따른 bool 변수
    int turnCount = 0; // Turn Count

    // 추후 master client가 gamemananger에서 생성한 랜덤 배열로 대체 예정
    int[] exArray = new int[100];
    int randomNum = 0;

    int playerCurRound = 6;
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

        // myPlayer Unit 전열, 후열 순서 지정 (0 ~ 2)
        for (int i = 0; i < UnitNum; i++)
        {
            myForwardUnits[i].GetComponent<AttackLogic>().Init(i);

            myBackwardUnits[i].GetComponent<AttackLogic>().Init(i);
        }

        // enemyPlayer Unit 전열, 후열 순서 지정 (3 ~ 5)
        for (int i = 0; i < UnitNum; i++)
        {
            enemyForwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
            enemyBackwardUnits[i].GetComponent<AttackLogic>().Init(i + 3);
        }
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
        GameObject EnemyUnit = new GameObject();

        while (true)
        {
            // 전열이 살아있는 경우
            if (myForwardUnits.Length != 0)
            {
                // 피격 Enemy Unit
                randomNum = exArray[turnCount];
                EnemyUnit = enemyForwardUnits[randomNum];
                myForwardUnits[turnCount].GetComponent<AttackLogic>().UnitAttack(turnCount, EnemyUnit);

                turnCount++;
            }

            // 전열이 전멸한 경우
            else if(myForwardUnits.Length == 0)
            {

            }
        }
    }

    // Enemy 선제 공격
    public void SubordinatedAttack()
    {

    }

    // 피격당한 유닛 제거
    public void RemoveUnit(int hitUnitNum)
    {

    }

}
