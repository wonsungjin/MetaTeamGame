using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public string skillType;


    // ================  스킬 발동 타이밍  ==================
    // 전투 씬에서 발동되는 경우
    public virtual void Skill_BattleStart() //전투 시작시 
    {

    }
    public virtual void Skill_AttackBefore() // 공격전
    {
        
    }
    public virtual void Skill_AttackAfter() // 공격후
    {

    }
    public virtual void Skill_Kill() // 처치시
    {

    }
    public virtual void Skill_Hit() // 피격시
    {     
          
    }
    public virtual void Skill_EnemyHit() // 적 피격시
    {

    }
    public virtual void Skill_Death() // 사망시
    {     
          
    }     
    public virtual void Skill_SummonAlly() // 아군 소환시 // 상점에서도 발동 가능. 발동 순서는 턴종료 전 리롤 이후
    {
        
    }

    // 상점 씬에서 발동되는 경우 
    public virtual void Skill_TurnStart() // 턴 시작시
    {

    }
    public virtual void Skill_Buy() // 구매시
    {      
        
    }      
    public virtual void Skill_Sell() // 판매시
    {      
        
    }           
    public virtual void Skill_ReRoll() // 리롤시 
    {

    }
    public virtual void Skill_TrunEnd() // 턴 종료시
    {
        int gold = 0;
        int buffHp = 0;
    }

    //================== 세부 스킬 내용 (스킬 함수 내부에서 호출되는 함수들)=================================================

    public void GetGold(int value) // 돈 획득
    {

    }
    public void Target(int who, int whom)  // 타겟팅 - 누가, 누구에게
    {

    }
    public void Summon(GameObject summon) // 소환
    {

    }
    public void Buff(Card card,string key) // 버프
    {

    }
}