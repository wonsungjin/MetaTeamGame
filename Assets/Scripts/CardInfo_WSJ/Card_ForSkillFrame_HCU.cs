//using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class Card : MonoBehaviour
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 batchPos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] GameObject curPos;

    //스킬 효과 관련 변수
    public int giveDamage = 0;
    public int takeDamage = 0;



    public void Start()
    {
       // SetSkillTiming(); // 나의 스킬타이밍에 따라 이벤트에 추가해야한다면 추가한다.
    }
    

    #region 스킬 효과 적용 관련 변수 모음

    public void Attack(int damage, Card Target, bool isDirect, bool isFirst) // 자신이 공격시 호출하는 함수 // 주는 데미지, 때릴 대상 // 직접 공격이냐 아니냐 (공격 차례때 때리는 것 / 스킬데미지로 때리는 것) // 첫타 구분(무한루프 방지)
    {
        if (cardInfo.skillTiming == SkillTiming.attackBefore) SkillActive(); // 공격 전 효과 발동
        Target.Hit(damage, this, isDirect, isFirst); // 지금부터 내가 너를 때리겠다는 말이야
        if (cardInfo.skillTiming == SkillTiming.attackAfter) SkillActive(); // 공격 후 효과 발동
    }

    public void Hit(int damage, Card Attacker, bool isDirect, bool isFirst) // 자신이 피격시 호출되는 함수 // 받은 데미지, 날 때린 사람
    {
        if (isDirect && isFirst == true) // 처음 직접 공격을 받았을 때만 응수를 하는 것이 응당 정당 타당 합당 마땅하다.
            Attacker.Hit(damage, this, true, false); // 니가 날 직접 때렸다면 나도 너를 때릴 것이다.

        this.curHP -= damage;
        if (this.curHP <= 0)
        {
            if(Attacker.cardInfo.skillTiming == SkillTiming.kill)   Attacker.SkillActive(); // 내가 죽었는데 적이 처치시 효과가 있다면 적 효과 먼저 발동시켜준다.
            if (cardInfo.skillTiming == SkillTiming.death) SkillActive(); // 사망시 효과 발동
            Destroy(this.gameObject);
        }

        if (cardInfo.skillTiming == SkillTiming.hit) // 피격시 효과 발동. 죽으면 피격시 효과가 발동하지 않는다.
        {
            GameMGR.Instance.Event_HitEnemy();
            SkillActive();
        } 
    }

    #endregion

    public void SetSkillTiming() // 스킬을 언제 발동시키느냐에 따라서 각 델리게이트 이벤트에 추가시켜준다. 이벤트는 보따리의 개념으로써 이벤트를 실행하면 안에 추가한 모든 함수들이 실행되기 때문에 공통적으로 사용되는 부분에서만 사용하는 것이 응당 정당 타당 합당 마땅하다고 보는 부분적인 부분이라고 할 수 있는 부분이다.
    {
        switch (cardInfo.skillTiming)
        {
            case SkillTiming.turnStart:
                GameMGR.Instance.callbackEvent_TurnStart += SkillActive;
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                break;
            case SkillTiming.buy:
                GameMGR.Instance.callbackEvent_Buy += SkillActive2;
                break;
            case SkillTiming.sell:
                GameMGR.Instance.callbackEvent_Sell += SkillActive2;
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                break;
            /*case SkillTiming.attackBefore:
                GameMGR.Instance.callbackEvent_BeforeAttack += SkillActive;
                break;
            case SkillTiming.attackAfter:
                GameMGR.Instance.callbackEvent_AfterAttack += SkillActive;
                break;*/
            case SkillTiming.kill:
                GameMGR.Instance.callbackEvent_Kill += SkillActive;
                break;
            /*case SkillTiming.hit:
                GameMGR.Instance.callbackEvent_Hit += SkillActive;
                break;*/
            case SkillTiming.hitEnemy:
                GameMGR.Instance.callbackEvent_HitEnemy += SkillActive;
                break;
            case SkillTiming.death:
                GameMGR.Instance.callbackEvent_Death += SkillActive;
                break;
            case SkillTiming.battleStart:
                GameMGR.Instance.callbackEvent_BattleStart += SkillActive;
                break;
            /*case SkillTiming.summon:
                GameMGR.Instance.callbackEvent_Summon += SkillActive;
                break;*/

        }
    }

    public void SkillActive() // 스킬 효과 발동 // FindTargetType 함수를 통해 구체적인 스킬 적용 대상이 정해지고 난 이후에 발동하는 게 맞다고 볼 수 있는 부분적인 부분
    {
        FindTargetType();
        SkillEffect();
    }

    public void SkillActive2(Card card)
    {
        //if (card != this) return;
        FindTargetType();
        SkillEffect();
    }


    public void SkillEffect() // 스킬 발동시 적용되는 효과
    {
        Debug.Log("스킬효과 발동");
        switch (cardInfo.effectType)
        {
            case EffectType.getGold:
                GameMGR.Instance.uiManager.goldCount += cardInfo.value1;
                break;
            case EffectType.damage:
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.value1, this, false, false);
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
                for (int i = 0; i < skillTarget.Count; i++)
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
                Card summonCard = Resources.Load<Card>($"Prefabs/{cardInfo.sumom_Unit}");
                //GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                summonCard.transform.position = targetPos;
                break;
            case EffectType.reduceShopLevelUpCost:
                // 상점 레벨업 비용 감소
                if (GameMGR.Instance.uiManager.shopMoney > 0)
                    GameMGR.Instance.uiManager.shopMoney -= cardInfo.value1;
                break;
            case EffectType.addHireUnit:
                // 고용가능 유닛 추가
                GameMGR.Instance.spawner.SpecialMonster();
                break;
        }
    }

    public void FindTargetType() // 어떤 유형의 대상을 찾는지에 따라 실행하는 경우가 다르다는 말이란 말이란 말이란 말이란 말이란 말
    {
        Debug.Log("타겟을 찾는다");
        GameObject[] searchArea = new GameObject[6]; // 대상 범위가 아군인지 적군인지에 따라 구분하여 담는 게임오브젝트 변수

        if(GameMGR.Instance.isBattleNow)
        {
            switch (cardInfo.effectTarget) // 스킬 효과 적용 대상에 따른 탐색 범위 지정
            {
                case EffectTarget.ally:
                    searchArea = GameMGR.Instance.battleLogic.playerAttackArray;
                    break;
                case EffectTarget.allyForward:
                    searchArea = GameMGR.Instance.battleLogic.playerForwardUnits;
                    break;
                case EffectTarget.allyBackward:
                    searchArea = GameMGR.Instance.battleLogic.playerBackwardUnits;
                    break;
                case EffectTarget.enemy:
                    searchArea = GameMGR.Instance.battleLogic.enemyAttackArray;
                    break;
                case EffectTarget.enemyForward:
                    searchArea = GameMGR.Instance.battleLogic.enemyForwardUnits;
                    break;
                case EffectTarget.enemyBackward:
                    searchArea = GameMGR.Instance.battleLogic.enemyBackwardUnits;
                    break;
                case EffectTarget.both:
                    //Array.Resize<GameObject>(ref searchArea, 12);
                    searchArea = new GameObject[12];
                    for (int i = 0; i < GameMGR.Instance.battleLogic.playerAttackArray.Length; i++)
                    {
                        searchArea[i] = (GameMGR.Instance.battleLogic.playerAttackArray[i]);
                    }
                    for (int i = 0; i < GameMGR.Instance.battleLogic.enemyAttackArray.Length; i++)
                    {
                        searchArea[i + 6] = (GameMGR.Instance.battleLogic.enemyAttackArray[i + 6]);
                    }
                    break;
                case EffectTarget.none:
                    break;
            }
        }
        else // 전투씬이 아니라면 상점씬 기준으로
        {
            switch (cardInfo.effectTarget) // 스킬 효과 적용 대상에 따른 탐색 범위 지정
            {
                case EffectTarget.ally:
                case EffectTarget.allyForward:
                case EffectTarget.allyBackward:
                    searchArea = GameMGR.Instance.spawner.cardBatch;
                    break;
                case EffectTarget.both:
                    //Array.Resize<GameObject>(ref searchArea, 12);
                    searchArea = new GameObject[6];
                    for (int i = 0; i < GameMGR.Instance.spawner.cardBatch.Length; i++)
                    {
                        searchArea[i] = (GameMGR.Instance.spawner.cardBatch[i]);
                    }
                    break;
                case EffectTarget.none:
                    break;
            }
        }

        switch (cardInfo.targetType) // 구체적인 공격 대상 지정 ( 체력이 낮은, 공격력이 높은, 전열 등등)
        {
            case TargetType.self:
                skillTarget.Add(this);
                break;

            case TargetType.empty: // 빈 공간을 찾는다 = 소환시
                bool isFind = false;
                for (int i = 0; i < 3; i++) // 앞열 검사
                {
                    if (GameMGR.Instance.battleLogic.playerForwardUnits[i] == null)
                    {
                        targetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    for (int i = 0; i < 3; i++) // 뒷열 검사
                    {
                        if (GameMGR.Instance.battleLogic.playerBackwardUnits[i] == null)
                        {
                            targetPos = GameMGR.Instance.battleLogic.playerForwardUnits[i].transform.position;
                            break;
                        }
                    }
                }
                break;

            case TargetType.random:
                int random = Random.Range(0, 6);
                while (searchArea[random].GetComponent<Card>().curHP <= 0) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                {
                    random = Random.Range(0, 6);
                }
                //skillTarget.Add(GameMGR.Instance.battleLogic.)
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;
            case TargetType.randomExceptMe:
                random = Random.Range(0, 6);
                while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                {
                    random = Random.Range(0, 6);
                }
                skillTarget.Add(transform.parent.transform.GetChild(random).gameObject.GetComponent<Card>());
                break;

            case TargetType.front:      // 전열ㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇ
                random = Random.Range(0, 3);
                bool isAllDead = true;
                for (int i = 0; i < 3; i++)
                {
                    if (searchArea[i].GetComponent<Card>().curHP >= 0)
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
                if (searchArea.Length == 0)
                {
                    skillTarget.Clear();
                }
                else // 한명이라도 살아있다면
                {
                    random = Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.back:       // 후열 ㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇ
                random = Random.Range(0, 3);
                isAllDead = true;
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea[i].GetComponent<Card>().curHP >= 0)
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
                    random = Random.Range(0, 3);
                    while (searchArea[random].GetComponent<Card>().curHP <= 0 && searchArea[random] == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = Random.Range(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponent<Card>());
                }
                break;

            case TargetType.otherSide:
                searchArea = GameMGR.Instance.battleLogic.playerAttackArray;
                int myIndex = System.Array.IndexOf(searchArea, gameObject); // 나를 찾는 과정 나에게로 떠나는 여행 모놀로그 에필로그 프롤로그 사이보그 아이언호그
                searchArea = GameMGR.Instance.battleLogic.enemyAttackArray;
                skillTarget.Add(searchArea[myIndex].GetComponent<Card>());
                break;

            case TargetType.leastATK:
                // 가장 공격력이 낮은 대상을 찾아라아아아ㅏ아아아아아아아아아아아아ㅏ아아즈벡!야아아아ㅏ 발바리이 치와아아아아아아ㅏ
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue; // 죽은 녀석은 대상에서 제외한다.
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
                    if (validIndex != 0) if (atkArray[validIndex] < atkArray[0]) leastAtk = i;
                }
                skillTarget.Add(searchArea[leastAtk].gameObject.GetComponent<Card>());
                break;

            case TargetType.mostATK:
                atkArray = new int[6];
                int mostAtk = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[mostAtk].GetComponent<Card>());
                break;
            case TargetType.leastHP:
                int[] hpArray = new int[6];
                int leastHp = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[leastHp].gameObject.GetComponent<Card>());
                break;
            case TargetType.mostHP:
                hpArray = new int[6];
                int mostHp = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponent<Card>().curHP <= 0) continue;
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
                skillTarget.Add(searchArea[mostHp].GetComponent<Card>());
                break;

            default:
                break;

        }
    }
}

