using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AttackLogic : Skill
{
    Vector2 playerPosition = Vector2.zero;
    Vector2 enemyPosition = Vector2.zero;

    public void UnitAttack(GameObject targetUnit)
    {
/*
        playerPosition = gameObject.transform.position;
        enemyPosition = targetUnit.transform.position;
*/
        Debug.Log(gameObject.name + " 공격");
        Debug.Log(targetUnit.name + " 피격");

        // enemy 피격 후 돌아옴
    }
}
