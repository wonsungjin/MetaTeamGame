using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum 정보
public enum SkillTiming     // 스킬이 발동되는 때
{
    turnStart,
    battleStart,
    buy,
    sell,
    reroll,
    attackBefore,
    attackAfter,
    kill,
    hit,
    hitEnemy,
    death,
    summon,
    turnEnd
}
public enum EffectTarget // 타겟 유형 - 적, 나, 아군 등
{
    none,
    both,
    enemy,
    ally,
    enemyBackward,
    enemyForward,
    allyBackward,
    allyForward,

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
    forward,
    backward,
    self,
    empty,

}

public enum TriggerCondition // 발동되기 위한 조건 - ex ) 빈 자리가 있어야한다
{
    allyEmpty,
    damageEcess,
    losePlayerHP,
}


public enum EffectType // 스킬 효과
{
    damage,     //데미지
    getGold,    //골드 획득
    changeHP,   //체력 증감
    changeATK,  //공격력 증감
    changeATKandHP, //공체 증감
    changeDamage,     //데미지 증감 value1은 주는 데미지, value2는 받는 데미지
    summon,         //소환
    reduceShopLevelUpCost,  //상점렙업비용 감소
    addHireUnit,    //고용유닛추가
    grantEXP,       //경험치 부여
    crossChangeHPandATK, // 공격력과 체력 맞바꿈
    oneShotKill,    //즉사기
    attackTargeting,//공격 타겟팅(대상어그로)

}
#endregion


[CreateAssetMenu(fileName = "new CardData", menuName = "ScriptableObjects/CardData")]
public class CardInfo : ScriptableObject
{
    //858ed67e0d64b72429e8c773f1903334
    [SerializeField] internal int ID;
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int hp;
    [SerializeField] internal int atk;
    [SerializeField] internal string description;
    [SerializeField] internal SkillTiming skillTiming;
    [SerializeField] internal TargetType targetType;
    [SerializeField] internal EffectTarget effectTarget;
    [SerializeField] internal int min_Target;
    [SerializeField] internal int max_Target;
    [SerializeField] internal TriggerCondition triggerCondition;
    [SerializeField] internal EffectType effectType;
    [SerializeField] internal int value1;
    [SerializeField] internal int value2;
    [SerializeField] internal int value3;
    [SerializeField] internal string sumom_Unit;
    [SerializeField] internal int num_Triggers;
    [SerializeField] internal int duration;
    [SerializeField] internal string appear;

    public string GetSkillExplantion(int num)
    {
        string [] skillExplantion =  description.Split(".");
        if (num == 1)
        {
            return $"Lv 1: {skillExplantion[0]}";
        }
        else if (num == 2)
        {
            if (skillExplantion.Length==1) return $"";
            return $"Lv 2:{skillExplantion[1]}";
        }
        else if (num == 3)
        {
            if (skillExplantion.Length == 1) return $"";
            return $"Lv 3:{skillExplantion[2]}";
        }
        return null;
    }

}