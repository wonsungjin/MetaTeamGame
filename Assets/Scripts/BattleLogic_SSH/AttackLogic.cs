using MongoDB.Driver.Builders;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class AttackLogic : Skill
{
    Vector2 playerTrans = Vector2.zero;
    Vector2 enemyTrans = Vector2.zero;
    Vector2 returnPosition = Vector2.zero;

    public bool isArrive = false;

    float curTime = 0f;
    float goalTime = 1f;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    Card card;

    public void Awake()
    {
        card = GetComponent<Card>();
    }

    private IEnumerator COR_Delay(GameObject targetUint)
    {
        curTime = 0;

        card.SetAnim("Walk");

        while (Vector2.Distance(gameObject.transform.parent.position, targetUint.transform.position) > 2)
        {
            curTime += Time.deltaTime;

            // Attack
            gameObject.transform.parent.position = Vector2.Lerp(playerTrans, enemyTrans, curTime / goalTime);

            yield return waitForFixedUpdate;
        }

        curTime = 0f;
        card.SetAnim("Attack");
        StartCoroutine(COR_AttackEFF(targetUint.transform.position));
        // 
        /////////////////////////////

        yield return new WaitForSeconds(1f);
        //is delvoewafafcajff
        card.Attack( card.curAttackValue, targetUint.GetComponentInChildren<Card>(), true, true );
        //GameMGR.Instance.objectPool.DestroyPrefab(targetUint);
        yield return new  WaitUntil(() => isArrive);
        isArrive = false;

        Debug.Log($"{card.curHP} 가 현재 나의 체력이다");
        if(card.curHP > 0 && this.gameObject != null)
        {
            card.SetAnim("Walk");

            while (Vector2.Distance(gameObject.transform.parent.position, returnPosition) > 0)
            {
                curTime += Time.deltaTime;

                // return position
                gameObject.transform.parent.position = Vector2.Lerp(playerTrans, returnPosition, curTime / goalTime);

                yield return waitForFixedUpdate;
            }
        }
        else
        {
            yield return new WaitForSecondsRealtime(1f);
            card.SetAnim("Idle");
        }


        GameMGR.Instance.battleLogic.isWaitAttack = true;
    }
    public void UnitAttack(GameObject targetUnit,bool player)
    {
        if (targetUnit == null)
        {
            Debug.Log("공격하려고 보니 대상이 없더라");
            GameMGR.Instance.battleLogic.isWaitAttack = true;
            return;
        }

        Debug.Log($"{gameObject.name}이 {targetUnit}를 때린다다다다다");

        playerTrans = gameObject.transform.parent.position;
        enemyTrans = targetUnit.transform.position;
        returnPosition = playerTrans;
        if (player == true)
        {
            if (GameMGR.Instance.battleLogic.playerAttackList.Count != 0) GameMGR.Instance.battleLogic.playerAttackList.RemoveAt(0);
        }
        if(player == false)
        {
            if (GameMGR.Instance.battleLogic.enemyAttackList.Count != 0) GameMGR.Instance.battleLogic.enemyAttackList.RemoveAt(0);
        }
        StartCoroutine(COR_Delay(targetUnit.transform.gameObject));
    }

    IEnumerator COR_AttackEFF(Vector3 targetUint)
    {
        yield return new WaitForSeconds(0.3f);
        GameObject eff = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>("CFXR Hit D 3D (Yellow)"), targetUint + new Vector3(0,0,-2f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        GameMGR.Instance.objectPool.DestroyPrefab(eff);


      //  GameObject summonCard = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{cardInfo.sumom_Unit}"), targetPos + new Vector2(0, -0.6f), Quaternion.identity);
    }

/*
    public void OnDisable()
    {
        GameMGR.Instance.battleLogic.isWaitAttack = true;
    }*/
}
