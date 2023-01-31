using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 TargetPos;

    [SerializeField] GameObject curPos;

    //스킬 관련 변수
    [SerializeField] GameObject AllyCamp;   // 아군 진형
    [SerializeField] GameObject EnemyCamp;   // 적 진형

    //스킬 효과 관련 변수
    public int giveDamage = 0;
    public int takeDamage = 0;

    public void Start()
    {
        AllyCamp = GameObject.Find("PlayerUnit");   // 아군 진형
        EnemyCamp = GameObject.Find("EnemyUnit");   // 적 진형
    }

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

    public void Hit(int damage) // 자신이 피격시 호출되는 함수
    {
        if(cardInfo.skillTiming == SkillTiming.hit)
        {
            SkillActive();
        }
        this.curHP -= damage;
        if (this.curHP <= 0) Destroy(this.GameObject());
    }

    public void SkillActive() // 스킬 효과 발동
    {
       switch(cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.value1);
                }
                break;
            case EffectType.changeDamage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].giveDamage += cardInfo.value1;
                    skillTarget[i].takeDamage += cardInfo.value2;
                }
                break;
            case EffectType.changeATK:
                for(int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curAttackValue += cardInfo.value1;
                }
                break;
            case EffectType.changeHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curHP += cardInfo.value1;
                }
                break;
            case EffectType.changeATKandHP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curAttackValue += cardInfo.value1;
                    skillTarget[i].curHP += cardInfo.value2;
                }
                break;
            case EffectType.grantEXP:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curEXP += cardInfo.value1;
                }
                break;
            case EffectType.summon:
                
                break;
            case EffectType.reduceHireCost:
                // 유닛 고용비용 감소
                break;
            case EffectType.reduceShopLevelUpCost:
                // 상점 레벨업 비용 감소
                break;
            case EffectType.addHireUnit:
                // 고용가능 유닛 추가
                break;
        }
    }
    public void FindTargetType() // 어떤 유형의 대상을 찾는지에 따라 실행하는 경우가 다르다는 말이란 말이란 말이란 말이란 말이란 말
    {
        GameObject searchArea; // 대상 범위가 아군인지 적군인지에 따라 구분하여 담는 게임오브젝트 변수
        if (cardInfo.effectTarget == EffectTarget.ally)
            searchArea = AllyCamp; // 아군 범위
        else if (cardInfo.effectTarget == EffectTarget.none) //대상이 없다면 타겟형스킬발동도 없는 것이다.
            return;
        else
            searchArea = EnemyCamp; // 적군 범위


        switch (cardInfo.targetType)
        {
            case TargetType.self:
                skillTarget.Add(this);
                break;

            case TargetType.empty: // 빈 공간을 찾는다 = 소환시
                for(int i = 0; i < 3; i++)
                {
                    if (GameMGR.Instance.battleLogic.playerForwardUnits[i] == null)
                    {
                        TargetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                        Card summonCard = Resources.Load<Card>($"Prefabs/{cardInfo.summonName}");
                        GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                    }
                }
                break;

            case TargetType.random:
                int random = Random.Range(0, 6);
                while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.randomExceptMe:
                random = Random.Range(0, 6);
                while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.front:
                random = Random.Range(0, 3);
                bool isAllDead = true;
                for(int i = 0; i < 3; i++)
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP >= 0)
                    {
                        isAllDead = false;
                        break;
                    }
                }
               if(isAllDead)
                {
                    //대상이 없으므로 스킬 무효 
                    skillTarget.Clear();
                }
                else // 한명이라도 살아있다면
                {
                    random = Random.Range(0, 3);
                    while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea.transform.GetChild(random).gameObject.GetComponent<Card>());
                }
                break;

            case TargetType.back:
                random = Random.Range(3, 6);
                isAllDead = true;
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP >= 0)
                    {
                        isAllDead = false;
                        break;
                    }
                }
                if (isAllDead)
                {
                    //대상이 없으므로 스킬 무효 
                    skillTarget.Clear();
                }
                else // 한명이라도 살아있다면
                {
                    random = Random.Range(3, 6);
                    while (searchArea.transform.GetChild(random).gameObject.GetComponent<Card>().curHP <= 0 && searchArea.transform.GetChild(random).gameObject == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = Random.Range(3, 6);
                    }
                    skillTarget.Add(searchArea.transform.GetChild(random).gameObject.GetComponent<Card>());
                }
                break;
            case TargetType.otherSide:
                if(searchArea.transform.GetChild(transform.parent.GetSiblingIndex()).gameObject.GetComponent<Card>() != null)
                {
                    skillTarget.Add(searchArea.transform.GetChild(transform.parent.GetSiblingIndex()).gameObject.GetComponent<Card>());
                }
                break;

            case TargetType.leastATK:
                // 가장 공격력이 낮은 대상을 찾아라아아아ㅏ아아아아아아아아아아아아ㅏ아아즈벡!야아아아ㅏ 발바리이 치와아아아아아아ㅏ
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue; // 죽은 녀석은 대상에서 제외한다.
                    if (leastAtk == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    { 
                        leastAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    if(validIndex != 0)  if(atkArray[validIndex] < atkArray[0]) leastAtk = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(leastAtk).gameObject.GetComponent<Card>());
                break;

            case TargetType.mostATK:
                atkArray = new int[6];
                int mostAtk = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (mostAtk == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        mostAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curAttackValue;
                        validIndex++;
                    }
                    if (validIndex != 0) if (atkArray[validIndex] > atkArray[0]) mostAtk = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(mostAtk).gameObject.GetComponent<Card>());
                break;
            case TargetType.leastHP:
                int[] hpArray = new int[6];
                int leastHp = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (leastHp == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        leastHp = i;
                        hpArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    else
                    {
                        hpArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    if (validIndex != 0) if (hpArray[validIndex] < hpArray[0]) leastHp = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(leastHp).gameObject.GetComponent<Card>());
                break;
            case TargetType.mostHP:
                hpArray = new int[6];
                int mostHp = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea.transform.GetChild(i).gameObject.GetComponent<Card>().curHP <= 0) continue;
                    if (mostHp == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        mostHp = i;
                        hpArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    else
                    {
                        hpArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponent<Card>().curHP;
                        validIndex++;
                    }
                    if (validIndex != 0) if (hpArray[validIndex] > hpArray[0]) mostHp = i;
                }
                skillTarget.Add(searchArea.transform.GetChild(mostHp).gameObject.GetComponent<Card>());
                break;

            default:
                break;

        }
    }
}

