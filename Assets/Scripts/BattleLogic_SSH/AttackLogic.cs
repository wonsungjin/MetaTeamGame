using MongoDB.Driver.Builders;
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

    float goalTime = 10f;


    private void Update()
    {

    }

    WaitForSeconds delayTime = new WaitForSeconds(0.15f);

    IEnumerator COR_Delay(GameObject targetUint)
    {

        while (Vector2.Distance(gameObject.transform.position, targetUint.transform.position) > 1)
        {
            yield return delayTime;

            // Attack
            gameObject.transform.position = Vector2.Lerp(playerTrans, enemyTrans, curTime / goalTime);
        }

        curTime = 0f;

        Destroy(targetUint);

        while (Vector2.Distance(gameObject.transform.position, targetUint.transform.position) > 1)
        {
            yield return delayTime;


            // return position
            gameObject.transform.position = Vector2.Lerp(enemyTrans, returnPosition, curTime / goalTime);
        }
        GameMGR.Instance.battleLogic.isWaitAttack = true;
        yield return delayTime;
    }
    public void UnitAttack(GameObject targetUnit)
    {
        playerTrans = gameObject.transform.position;
        enemyTrans = targetUnit.transform.position;
        returnPosition = playerTrans;

        StartCoroutine(COR_Delay(targetUnit));
    }
}
