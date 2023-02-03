using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum When
{
    BattleStart,
    AttackBefore,
    AttackAfter,
    Kill,
    Hit,
    EnemyHit,
    Death,
    Summon,
    SummonAlly,
    SummonEnemy,
    TurnStart,
    Buy,
    Sell,
    Reroll,
    TurnEnd,
}

public class Skill : MonoBehaviour
{
    public string skillType;

    public delegate void BattleStart();
    public event BattleStart battleStart;

    public void TriggerTiming(When when)
    {

    }

    // ================  스킬 발동 타이밍  ==================
    // 전투 씬에서 발동되는 경우
    public virtual void When_BattleStart() //전투 시작시 
    {
        //BattleStart callbackreciever_battleStart = When_BattleStart;

    }
    public virtual void When_AttackBefore() // 공격전
    {
        
    }
    public virtual void When_AttackAfter() // 공격후
    {

    }
    public virtual void When_Kill() // 처치시
    {

    }
    public virtual void When_Hit() // 피격시
    {     
          
    }
    public virtual void When_EnemyHit() // 적 피격시
    {

    }
    public virtual void When_Death() // 사망시
    {     
          
    }     
    public virtual void When_Summon(bool isAlly) // 아군 소환시 // 상점에서도 발동 가능. 발동 순서는 턴종료 전 리롤 이후
    {
        
    }

    // 상점 씬에서 발동되는 경우 
    public virtual void When_TurnStart() // 턴 시작시
    {

    }
    public virtual void When_Buy() // 구매시
    {      
        
    }      
    public virtual void When_Sell() // 판매시
    {      
        
    }           
    public virtual void When_ReRoll() // 리롤시 
    {

    }
    public virtual void When_TurnEnd() // 턴 종료시
    {

    }

    //================== 세부 스킬 내용 (스킬 함수 내부에서 호출되는 함수들)=================================================

    public void GetGold(int value) // 돈 획득
    {
        // 상점 씬으로 넘어가서 
        GameMGR.Instance.uiManager.goldCount += value;
    }
    public void ReduceHire(int value) // 고용 비용 감소
    {
        // 용병 고용 비용 value 만큼 감소
    }
    public void ReduceHireShop(int value) // 고용 비용 감소
    {
        // 용병 고용소 레벨업 비용 value 만큼 감소
    }
    public void Summon(Card summoned, int num) // 소환, 갯수
    {
        // 자신의 필드에 빈 칸이 있는지 확인

        // 빈 칸이 있다면 해당 위치에 소환(빈칸 중 인덱스 순서대로 우선순위)

        // 빈 칸이 없다면 소환 실패 처리
    }

    public void Buff(Card card,string key, bool isTemp) // 지정 버프 - 버프 대상, 버프 키워드(공격력/체력/경험치), 일시적 여부
    {

    }

    public void Buff(int count, string key, bool isTemp) // 랜덤 버프 - 버프 대상 갯수, 버프 키워드, 일시적 여부
    {

    }

    public void Target(Card target, string key, int value)  // 타겟팅 - 타겟 대상, 타겟팅능력 키워드, 값
    {

    }


}