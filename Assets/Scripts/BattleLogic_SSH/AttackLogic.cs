using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AttackLogic : Skill
{
    Vector2 playerTrans = Vector2.zero;
    Vector2 enemyTrans = Vector2.zero;
    Vector2 returnPosition = Vector2.zero;

    public void UnitAttack(GameObject targetUnit)
    {
        /*playerTrans = gameObject.transform.position;
        enemyTrans = targetUnit.transform.position;
        returnPosition = playerTrans;

        // Attack
        gameObject.transform.position = Vector2.Lerp(playerTrans, enemyTrans, 0.5f);

        // return position
        if (gameObject.transform.position.x == enemyTrans.x)
        {
            gameObject.transform.position = Vector2.Lerp(enemyTrans, returnPosition, 0.5f);
        }*/
    }
}
