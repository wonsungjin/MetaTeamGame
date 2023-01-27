using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] Card skillTarget;

    [SerializeField] GameObject curPos;

    public void SetSkillTiming() // 스킬을 언제 발동시키느냐에 따라서 각 이벤트에 추가시켜준다.
    {
        switch(cardInfo.skillTiming)
        {
            case SkillTiming.turnStart:
                GameMGR.Instance.callbackEvent_TurnStart += SkillActive;
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                break;
            case SkillTiming.buy:
                GameMGR.Instance.callbackEvent_Buy += SkillActive;
                break;
            case SkillTiming.sell:
                GameMGR.Instance.callbackEvent_Sell += SkillActive;
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                break;
            case SkillTiming.attackBefore:
                GameMGR.Instance.callbackEvent_BeforeAttack += SkillActive;
                break;
            case SkillTiming.attackAfter:
                GameMGR.Instance.callbackEvent_AfterAttack += SkillActive;
                break;
            case SkillTiming.kill:
                GameMGR.Instance.callbackEvent_Kill += SkillActive;
                break;
            case SkillTiming.hit:
                GameMGR.Instance.callbackEvent_Hit += SkillActive;
                break;
            case SkillTiming.hitEnemy:
                GameMGR.Instance.callbackEvent_HitEnemy += SkillActive;
                break;
            case SkillTiming.death:
                GameMGR.Instance.callbackEvent_Death += SkillActive;
                break;
            case SkillTiming.battleStart:
                GameMGR.Instance.callbackEvent_BattleStart += SkillActive;
                break;
            case SkillTiming.summon:
                GameMGR.Instance.callbackEvent_Summon += SkillActive;
                break;

        }
    }

    public void SkillActive()
    {
       switch(cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                //cardInfo.value1;
                break;
            case EffectType.changeATK:
                
                break;

        }
    }

    public void CheckEffectTarget() // 스킬 적용 대상
    {
        switch(cardInfo.effectTarget)
        {
            
            case EffectTarget.self:
                skillTarget = this;
                break;
            case EffectTarget.allyUnit:
                int random = Random.Range(0, 6);
                if (transform.parent.gameObject.name == "Store") // 현재 나의 배치가 상점인 경우
                {
                    skillTarget = transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>();
                }
                else if (transform.parent.gameObject.name == "Battle") // 현재 나의 배치가 전투지인 경우
                {
                    skillTarget = transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>();
                }
                //skillTarget = 
                break;
            case EffectTarget.allyFront:
                break;
            case EffectTarget.allyBack:
                break;
            case EffectTarget.enemyUnit:
                break;
            case EffectTarget.enemyFront:
                break;
            case EffectTarget.enemyBack:
                break;
            case EffectTarget.unitAll: // 피아식별 안하고 싹 다 공격대상으로 삼는 극악무도한 경우
                break;
        }
    }

    public void FindTargetType() // 어떤 유형의 대상을 찾는지에 따라 실행하는 경우가 다르다는 말이란 말이란 말이란 말이란 말이란 말이-야아~ 아-베! 말이야~
    {
        switch(cardInfo.targetType)
        {
            case TargetType.leastATK:
                // 가장 공격력이 낮은 대상을 찾아라아아아ㅏ아아아아아아아아아아아아ㅏ아아즈벡!야아아아ㅏ 발바리이 치와아아아아아아ㅏ
                break;
        }
    }
}

