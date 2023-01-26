using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum 정보
public enum SkillTiming     // 스킬이 발동되는 때
{
    turnStart,
    startBattle,
    buy,
    sell,
    reroll,
    attackBefore,
    attackAfter,
    kill,
    hit,
    enemyHit,
    death,
    summonAlly,
    turnEnd
}

public enum TargetType // 스킬이 어떤 타입을 노리는지(ex - 공격력 높은 유닛)
{
    none,
    all,
    allExceptMe,
    random,
    randomExceptMe,
    mostATK,
    leastATK,
    leastHP,
    mostHP,
    front,
    back,
    near,
    otherSide,
}

public enum EffectTarget // 타겟 유형 - 적, 나, 아군 등
{
    none,
    self,
    unit,
    enemyUnit,
    allyUint,
    enemyFront,
    enemyBack,
    allyFront,
    allyBack
}

public enum TriggerCondition // 발동되기 위한 조건 - ex ) 빈 자리가 있어야한다
{
    allyEmpty
}

public enum EffectType // 스킬 효과
{
    damage,     //데미지
    getGold,    //골드 획득
    changeHP,   //체력 증감
    changeATK,  //공격력 증감
    changeATKandHP, //공체 증감
    summon,         //소환
    changeDamage,   //데미지 증감
    attackTargeting,//공격 타겟팅(대상어그로)
    ReduceHireCost, //고용비용 감소
    ReduceShopLevelUpCost,  //상점렙업비용 감소
    grantEXP,       //경험치 부여
    addHireUnit,    //고용유닛추가

}


#endregion
public partial class CardInfo : ScriptableObject
{
    [Header("Skill Data")]
    [SerializeField] internal SkillTiming skillTiming; 
    [SerializeField] internal TargetType targetType; 
    [SerializeField] internal EffectTarget target; // 타겟 유형 - 적군, 아군, 자신 등
    [SerializeField] internal int Min_target; // 최소 타겟 수
    [SerializeField] internal int Max_target; // 최대 타겟 수
    [SerializeField] internal TriggerCondition triggerCondition; //발동 조건 - 어떤 상황에서 발동되는가(ex - 빈 자리 있을 때)
    [SerializeField] internal EffectType EffectType;
    [SerializeField] internal int value1; //값1
    [SerializeField] internal int value2; //값2
    [SerializeField] internal int triggerCount; // 발동횟수
    [SerializeField] internal int groupIndex; // 그룹인덱스

    /*//858ed67e0d64b72429e8c773f1903334
    [SerializeField] internal int ID;
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int tribe;
    [SerializeField] internal int hp;
    [SerializeField] internal int attackValue;
    [SerializeField] internal string skilltype;
    [SerializeField] internal string skill;
    [SerializeField] internal bool appear;
    public string GetSkillExplantion(int num)
    {
        string [] skillExplantion =  skill.Split(".");
        if (num == 1)
        {
            return $"Level 1: {skillExplantion[0]}";
        }
        else if (num == 2)
        {
            if (skillExplantion.Length==2) return $"Level 2:{skillExplantion[0]}";
            return $"Level 2:{skillExplantion[1]}";
        }
        else if (num == 3)
        {
            if (skillExplantion.Length == 2) return $"Level 3:{skillExplantion[0]}";
            return $"Level 3:{skillExplantion[2]}";
        }
        return null;
    }*/

}