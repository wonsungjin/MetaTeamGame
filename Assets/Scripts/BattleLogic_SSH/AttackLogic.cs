using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AttackLogic : Skill
{
    Vector2 playerTrans = Vector2.zero;
    Vector2 enemyTrans = Vector2.zero;
    Vector2 returnPosition = Vector2.zero;

    bool isArrive = false;

    float curTime = 0f;

    float goalTime = 3f;

    public void UnitAttack(GameObject targetUnit)
    {
        playerTrans = gameObject.transform.position;
        enemyTrans = targetUnit.transform.position;
        returnPosition = playerTrans;

        while (curTime < goalTime)
        {
            curTime += Time.deltaTime;

            // Attack
            gameObject.transform.position = Vector2.Lerp(playerTrans, enemyTrans, curTime / goalTime);
        }

        curTime = 0f;

        Destroy(targetUnit);

        while (curTime < goalTime)
        {
            curTime += Time.deltaTime;

            // return position
            gameObject.transform.position = Vector2.Lerp(enemyTrans, returnPosition, curTime / goalTime);
        }
    }
}
