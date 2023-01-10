using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLogic : MonoBehaviour
{
    [SerializeField] public int attackOrder = 0;

    Vector2 playerPosition = Vector2.zero;
    Vector2 enemyPosition = Vector2.zero;

    public void Init(int order)
    {
        // 플레이어 순서 지정
        attackOrder = order;
    }

    public void UnitAttack(int playerUnitNum, GameObject targetUnit)
    {
        // BattleLogic Script에서 호출한 player unit과 일치하는 경우 공격
        if (attackOrder == playerUnitNum)
        {
            playerPosition = gameObject.transform.position;
            enemyPosition = targetUnit.transform.position;

            // enemy 피격 후 돌아옴
            gameObject.transform.position = Vector2.Lerp(playerPosition, enemyPosition, 0.5f);
            gameObject.transform.position = Vector2.Lerp(enemyPosition, playerPosition, 0.5f);
        }
    }
}
