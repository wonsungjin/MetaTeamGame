using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using Unity.VisualScripting;

using Photon.Pun;
using System.Collections;

public partial class Card : MonoBehaviourPun
{
    //[SerializeField] public CardInfo cardInfo;
    [SerializeField] List<Card> skillTarget;
    [SerializeField] Vector2 batchPos;
    [SerializeField] Vector2 targetPos;

    [SerializeField] GameObject curPos;

    //스킬 효과 관련 변수
    public int giveDamage = 0;
    public int takeDamage = 0;


    public int shopBatchEmptyIndex = 0;  // 상점 배치 인덱스값을 저장하는 변수

    public bool isMine = false; // 이 카드가 나의 것인지 적의 것인지

    public int triggerOnCount;

    //스킬 범위를 설정하는 배열 ( 위 isMine 값에 따라 들어가는 기준이 다르다 )
    GameObject[] myArea;
    GameObject[] myAreaFront;
    GameObject[] myAreaBack;
    GameObject[] enemyArea;
    GameObject[] enemyAreaFront;
    GameObject[] enemyAreaBack;

    public void OnEnable()
    {
        // SetSkillTiming(); // 나의 스킬타이밍에 따라 이벤트에 추가해야한다면 추가한다.
        triggerOnCount = cardInfo.GetNumTrigger(level);
    }

    #region 스킬 효과 적용 관련 변수 모음

    public void Attack(int damage, Card Target, bool isDirect, bool isFirst) // 자신이 공격시 호출하는 함수 // 주는 데미지, 때릴 대상 // 직접 공격이냐 아니냐 (공격 차례때 때리는 것 / 스킬데미지로 때리는 것) // 첫타 구분(무한루프 방지)
    {
        Debug.Log($"{gameObject.name}이 {Target.name}에게 {damage} 데미지를 주었다");
        if (cardInfo.skillTiming == SkillTiming.attackBefore) SkillActive(); // 공격 전 효과 발동
        //GameMGR.Instance.audioMGR.BattleAttackSound(damage);
        Target.Hit(damage, this, isDirect, isFirst); // 지금부터 내가 너를 때리겠다는 말이야
        if (cardInfo.skillTiming == SkillTiming.attackAfter) SkillActive(); // 공격 후 효과 발동
    }

    public void Hit(int damage, Card Attacker, bool isDirect, bool isFirst) // 자신이 피격시 호출되는 함수 // 받은 데미지, 날 때린 사람
    {
        if(cardInfo.effectType == EffectType.changeDamage)
        {
            if(damage + cardInfo.GetValue(1, level) > 1)
            {
                damage = damage + cardInfo.GetValue(1, level);
            }
            else
            {
                damage = 1;
            }
        }
        if (!isDirect)
            Debug.Log($"{gameObject.name}이 {Attacker.name}에게 {damage}만큼 맞았다. 스킬피해");
        else
            Debug.Log($"{gameObject.name}이 {Attacker.name}에게 {damage}만큼 맞았다. 직접공격 : {isDirect}, 첫공격 : {isFirst}");
        if (isDirect && isFirst == true) // 처음 직접 공격을 받았을 때만 응수를 하는 것이 응당 정당 타당 합당 마땅하다.
            Attacker.Hit(curAttackValue, this, true, false); // 니가 날 직접 때렸다면 나도 너를 때릴 것이다.
        curHP -= damage;
        hpText.text = curHP.ToString();


        if (this.curHP <= 0)
        {
            StartCoroutine(COR_Dead(Attacker));
        }

        GameMGR.Instance.Event_HitEnemy(this);

        if (cardInfo.skillTiming == SkillTiming.hit) // 피격시 효과 발동. 죽으면 피격시 효과가 발동하지 않는다.
        {
            SkillActive();
        }
        GetComponent<AttackLogic>().isArrive = true;
    }

    #endregion
    IEnumerator COR_Dead(Card Attacker)
    {
        SetAnim("Dead");

        if (isMine)
        {
            for (int i = 0; i < 6; i++)
            {
                if (GameMGR.Instance.battleLogic.playerAttackArray[i] == null) continue;
                if (GameMGR.Instance.battleLogic.playerAttackArray[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.playerAttackArray[i] = null;
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (GameMGR.Instance.battleLogic.playerForwardUnits[i] == null) continue;
                if (GameMGR.Instance.battleLogic.playerForwardUnits[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.playerForwardUnits[i] = null;
                    break;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (GameMGR.Instance.battleLogic.playerBackwardUnits[i] == null) continue;
                if (GameMGR.Instance.battleLogic.playerBackwardUnits[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.playerBackwardUnits[i] = null;
                    break;
                }
            }
        }
        else // 적인 경우
        {
            for (int i = 0; i < 6; i++)
            {
                if (GameMGR.Instance.battleLogic.enemyAttackArray[i] == null) continue;
                if (GameMGR.Instance.battleLogic.enemyAttackArray[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.enemyAttackArray[i] = null;
                    break;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (GameMGR.Instance.battleLogic.enemyForwardUnits[i] == null) continue;
                if (GameMGR.Instance.battleLogic.enemyForwardUnits[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.enemyForwardUnits[i] = null;
                    break;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (GameMGR.Instance.battleLogic.enemyBackwardUnits[i] == null) continue;
                if (GameMGR.Instance.battleLogic.enemyBackwardUnits[i].GetComponentInChildren<Card>() == this)
                {
                    GameMGR.Instance.battleLogic.enemyBackwardUnits[i] = null;
                    break;
                }
            }
        }

        if (Attacker.cardInfo.skillTiming == SkillTiming.kill) Attacker.SkillActive(); // 내가 죽었는데 적이 처치시 효과가 있다면 적 효과 먼저 발동시켜준다.
        if (cardInfo.skillTiming == SkillTiming.death) SkillActive(); // 사망시 효과 발동
                                                                      //GameMGR.Instance.battleLogic.isWaitAttack = true;
        yield return new WaitForSeconds(2f);

        SetAnim("Idle");

        GameMGR.Instance.objectPool.DestroyPrefab(gameObject.transform.parent.gameObject);
    }

    public void SetSkillTiming() // 스킬을 언제 발동시키느냐에 따라서 각 델리게이트 이벤트에 추가시켜준다. 이벤트는 보따리의 개념으로써 이벤트를 실행하면 안에 추가한 모든 함수들이 실행되기 때문에 공통적으로 사용되는 부분에서만 사용하는 것이 응당 정당 타당 합당 마땅하다고 보는 부분적인 부분이라고 할 수 있는 부분이다.
    {
         
        Debug.Log("SetSkillTiming 설정하는 함수로 들어왔다");
        switch (cardInfo.skillTiming)
        {
            case SkillTiming.turnStart:
                GameMGR.Instance.callbackEvent_TurnStart += SkillActive;
                Debug.Log("턴시작시 효과니까 이벤트에 추가");
                break;
            case SkillTiming.turnEnd:
                GameMGR.Instance.callbackEvent_TurnEnd += SkillActive;
                Debug.Log("턴종료시효과니까 이벤트에 추가");
                break;
            case SkillTiming.reroll:
                GameMGR.Instance.callbackEvent_Reroll += SkillActive;
                Debug.Log("리롤시효과니까 이벤트에 추가");
                break;
            case SkillTiming.hitEnemy:
                GameMGR.Instance.callbackEvent_HitEnemy += SkillActive2;
                break;
            case SkillTiming.battleStart:
                GameMGR.Instance.callbackEvent_BattleStart += SkillActive;
                Debug.Log("전투시작시효과니까 이벤트에 추가" + gameObject.name);
                break;
                /*case SkillTiming.summon:
                    GameMGR.Instance.callbackEvent_Summon += SkillActive;
                    break;*/
        }
        isSkillTiming = true; //
    }

    public void SkillActive() // 스킬 효과 발동 // FindTargetType 함수를 통해 구체적인 스킬 적용 대상이 정해지고 난 이후에 발동하는 게 맞다고 볼 수 있는 부분적인 부분
    {
        if (triggerOnCount < 1) return;
        
        if (cardInfo.skillTiming == SkillTiming.hitEnemy || cardInfo.skillTiming == SkillTiming.battleStart)
            if (isBattle == false) return;

        Debug.Log("Skill Active");

        if (cardInfo.effectType == EffectType.damage && cardInfo.GetValue(3, level) != 0) // 데미지는 연속으로 때리는 경우가 있기 때문에 특수 처리를 해준다
        {
            for (int i = 0; i < cardInfo.GetValue(3, level); i++)
            {
                Debug.Log("연속 타격");
                FindTargetType();
                SkillEffect();
                triggerOnCount--;
            }
        }
        else
        {
            Debug.Log("단발 타격");
            FindTargetType();
            SkillEffect();
            triggerOnCount--;
        }

    }

    public void SkillActive2(Card card)
    {
        if (triggerOnCount < 1) return;
        if (cardInfo.skillTiming == SkillTiming.hitEnemy || cardInfo.skillTiming == SkillTiming.battleStart)
            if (isBattle == false) return;

        if (cardInfo.skillTiming == SkillTiming.hitEnemy)
        {
            if ((isMine && !card.isMine) || (!isMine && card.isMine))
            {
                FindTargetType();
                SkillEffect();
                triggerOnCount--;
            }
            else return;
        }
        else if (card != this) return;

        else if (cardInfo.effectType == EffectType.summon)
        {
            for (int i = 0; i < cardInfo.GetNumTrigger(level); i++)
            {
                FindTargetType();
                SkillEffect();
                triggerOnCount--;
            }
        }
        else
        {
            Debug.Log("Skill Active 2");
            FindTargetType();
            SkillEffect();
            triggerOnCount--;
        }
    }


    public void SkillEffect() // 스킬 발동시 적용되는 효과
    {
        Debug.Log(skillTarget.Count + "스킬타겟 길이");
        Debug.Log("스킬효과 발동");
        switch (cardInfo.effectType)
        {
            case EffectType.getGold:
                Debug.Log("골드 획득 효과 발동");
                GameMGR.Instance.uiManager.goldCount += cardInfo.GetValue(0, level);
                break;
            case EffectType.damage:
                Debug.Log("데미지 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].Hit(cardInfo.GetValue(1, level), this, false, false);
                }
                break;
            case EffectType.changeDamage:
                Debug.Log("데미지 증감 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    if (skillTarget[i].curAttackValue > 0 && skillTarget[i].curAttackValue > cardInfo.GetValue(1, level))
                    {
                        skillTarget[i].giveDamage += cardInfo.GetValue(1, level);
                        skillTarget[i].takeDamage += cardInfo.GetValue(2, level);
                    }

                }
                break;
            case EffectType.changeATK:
                Debug.Log("공격력 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    if (cardInfo.GetValue(1, level) < 0) // && skillTarget[i].curAttackValue > cardInfo.GetValue(1, level))
                    {
                        Debug.Log("공격력 감소 효과");
                        if (skillTarget[i].curAttackValue + cardInfo.GetValue(1, level) < 1)
                        {
                            Debug.Log("감소한 값이 음수");
                            skillTarget[i].ChangeValue(CardStatus.Attack, 1);
                        }
                        else
                        {
                            Debug.Log("감소한 값이 양수");
                            skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level), true);
                        }
                        atkText.text = curAttackValue.ToString();
                    }
                    else
                    {
                        Debug.Log("공격력 증가 효과");
                        skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level), true);
                        atkText.text = curAttackValue.ToString();
                    }

                    Debug.Log(cardInfo.GetValue(1, level));
                    //skillTarget[i].curAttackValue += cardInfo.value1;
                }
                break;
            case EffectType.changeHP:
                Debug.Log("체력 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].ChangeValue(CardStatus.Hp, cardInfo.GetValue(1, level), true);
                }
                break;
            case EffectType.changeATKandHP:
                Debug.Log("공격력 체력 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].ChangeValue(CardStatus.Attack, cardInfo.GetValue(1, level), true);
                    skillTarget[i].ChangeValue(CardStatus.Hp, cardInfo.GetValue(2, level), true);
                }
                break;
            case EffectType.grantEXP:
                Debug.Log("경험치 부여 효과 발동");
                for (int i = 0; i < skillTarget.Count; i++)
                {
                    skillTarget[i].curEXP += cardInfo.GetValue(1, level);
                }
                break;
            case EffectType.summon:
                Debug.Log(cardInfo.sumom_Unit + "소환 효과 발동");
                if (targetPos == Vector2.zero) break; //만약에 빈칸이 없다면 소환을 하지말라
                GameObject summonCard = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>($"Prefabs/{cardInfo.sumom_Unit}"), targetPos + new Vector2(0, -0.6f), Quaternion.identity);
                summonCard.transform.GetChild(0).tag = "BattleMonster";
                summonCard.transform.localScale = summonCard.transform.localScale * 2;
                summonCard.GetComponentInChildren<Card>().spriteRenderer.sortingLayerName = "SellTXT";

                GameMGR.Instance.spawner.cardBatch[shopBatchEmptyIndex] = summonCard;
                //summoncard 이름 디버그 띄울것
                Debug.Log(summonCard.name);

                //GameMGR.Instance.battleLogic.playerForwardUnits.Add(summonCard.gameObject);
                Debug.Log(targetPos + ": 소환할 때 지정되어있던 타겟포즈");
                //summonCard.transform.position = targetPos;
                break;
            case EffectType.reduceShopLevelUpCost:
                Debug.Log("상점 렙업 비용 감소 효과 발동");
                // 상점 레벨업 비용 감소
                if (GameMGR.Instance.uiManager.shopMoney > 0)
                    GameMGR.Instance.uiManager.shopMoney -= cardInfo.GetValue(1, level);
                GameMGR.Instance.uiManager.ChangeShopLevelUpCost(GameMGR.Instance.uiManager.shopMoney); //  비용값 변경 출력
                break;
            case EffectType.addHireUnit:
                Debug.Log("고용가능 유닛 추가 효과 발동");
                // 고용가능 유닛 추가
                GameMGR.Instance.spawner.SpecialMonster();
                break;
        }
    }

    //==============================================================================================================================================================
    //==========================================               ★ 특 수 발 동 조 건 ★                ================================================================
    //==============================================================================================================================================================

    public void CheckTriggerCondition(Card target = null)
    {
        if (cardInfo.triggerCondition != 0) // 특수 발동조건이 있는 경우
        {
            switch (cardInfo.triggerCondition)
            {
                case TriggerCondition.allyEmpty:
                    for (int i = 0; i < 6; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] == null)
                        {
                            curAttackValue = cardInfo.GetValue(1, level);
                            curHP = cardInfo.GetValue(2, level);
                            break;
                        }
                    }
                    break;

                case TriggerCondition.damageEcess:
                    if (curAttackValue > target.curHP)
                    {
                        int excessDamage = curAttackValue - target.curHP;
                        FindTargetType(false);
                        List<GameObject> curExistBatch = new List<GameObject>();
                        for (int i = 0; i < enemyArea.Length; i++)
                        {
                            if (enemyArea[i] != null)
                            {
                                curExistBatch.Add(enemyArea[i]);
                            }
                        }
                        if (curExistBatch.Count == 0) break;

                        int curTargetNum = GameMGR.Instance.GetRandomValue(0, curExistBatch.Count);

                        Attack(excessDamage, curExistBatch[curTargetNum].GetComponentInChildren<Card>(), false, false);
                    }
                    break;

                case TriggerCondition.losePlayerHP:
                    int curLoseLife = 20 - (int)PhotonNetwork.LocalPlayer.CustomProperties["Life"];
                    curHP += curLoseLife * cardInfo.GetValue(1, level);
                    break;
            }
        }
    }

    //==============================================================================================================================================================

    public List<GameObject> searchArea = new List<GameObject>(); // 대상 범위가 아군인지 적군인지에 따라 구분하여 담는 게임오브젝트 변수
    public void FindTargetType(bool isBaseOnDB = true) // 어떤 유형의 대상을 찾는지에 따라 실행하는 경우가 다르다는 말이란 말이란 말이란 말이란 말이란 말
    {
        searchArea.Clear();
        skillTarget.Clear();
        Debug.Log("타겟을 찾는다");
        // 유닛이 스킬 사용시 나의 유닛 기준인지 상대 유닛 기준인지에 따라서 담아주는 경우가 다른 경우를 말하는 경우라고 할 수 있는 경우


        if (isMine)
        {
            myArea = GameMGR.Instance.battleLogic.playerAttackArray;
            myAreaFront = GameMGR.Instance.battleLogic.playerForwardUnits;
            myAreaBack = GameMGR.Instance.battleLogic.playerBackwardUnits;

            enemyArea = GameMGR.Instance.battleLogic.enemyAttackArray;
            enemyAreaFront = GameMGR.Instance.battleLogic.enemyForwardUnits;
            enemyAreaBack = GameMGR.Instance.battleLogic.enemyBackwardUnits;
        }
        else
        {
            enemyArea = GameMGR.Instance.battleLogic.playerAttackArray;
            enemyAreaFront = GameMGR.Instance.battleLogic.playerForwardUnits;
            enemyAreaBack = GameMGR.Instance.battleLogic.playerBackwardUnits;

            myArea = GameMGR.Instance.battleLogic.enemyAttackArray;
            myAreaFront = GameMGR.Instance.battleLogic.enemyForwardUnits;
            myAreaBack = GameMGR.Instance.battleLogic.enemyBackwardUnits;
        }

        if (isBaseOnDB) SetTargetType();    // 디테일한 대상 찾기
    }

    //===============================================================================================================================================================
    //======================================     대상을 찾는 범위 설정 및 구체적인 대상의 특징을 설정하는 부분         =====================================================
    //===============================================================================================================================================================
    private void SetTargetType()
    {
        if (GameMGR.Instance.isBattleNow)
        {
            switch (cardInfo.effectTarget) // 스킬 효과 적용 대상에 따른 탐색 범위 지정
            {
                case EffectTarget.ally:
                    Debug.Log($"{gameObject.name} 이 사용 : 아군");
                    for (int i = 0; i < myArea.Length; i++)
                    {
                        if (myArea[i] != null)
                        {
                            searchArea.Add(myArea[i]);
                        }
                    }
                    break;
                case EffectTarget.allyForward:
                    Debug.Log("아군전열");
                    for (int i = 0; i < myAreaFront.Length; i++)
                    {
                        if (myAreaFront[i] != null)
                        {
                            searchArea.Add(myAreaFront[i]);
                        }
                    }
                    break;
                case EffectTarget.allyBackward:
                    Debug.Log("아군후열");
                    for (int i = 0; i < myAreaBack.Length; i++)
                    {
                        if (myAreaBack[i] != null)
                        {
                            searchArea.Add(myAreaBack[i]);
                        }
                    }
                    break;
                case EffectTarget.enemy:
                    Debug.Log("적군");
                    for (int i = 0; i < enemyArea.Length; i++)
                    {
                        if (enemyArea[i] != null)
                        {
                            searchArea.Add(enemyArea[i]);
                        }
                    }
                    break;
                case EffectTarget.enemyForward:
                    Debug.Log("적전열");
                    for (int i = 0; i < enemyAreaFront.Length; i++)
                    {
                        if (enemyAreaFront[i] != null)
                        {
                            searchArea.Add(enemyAreaFront[i]);
                        }
                    }
                    break;
                case EffectTarget.enemyBackward:
                    Debug.Log("적후열");
                    for (int i = 0; i < enemyAreaBack.Length; i++)
                    {
                        if (enemyAreaBack[i] != null)
                        {
                            searchArea.Add(enemyAreaBack[i]);
                        }
                    }
                    break;
                case EffectTarget.both:
                    Debug.Log("전체");
                    for (int i = 0; i < myArea.Length; i++)
                    {
                        searchArea.Add(myArea[i]);
                    }
                    for (int i = 0; i < enemyArea.Length; i++)
                    {
                        searchArea.Add(enemyArea[i]);
                    }
                    break;
                case EffectTarget.none:
                    break;
            }

            Debug.Log("현재는 전투씬 기준");

        }
        else // 전투씬이 아니라면 상점씬 기준으로  // 상점씬으로 들어오면 적군 경우는 해당하지 않게 된다. 
        {
            Debug.Log("현재는 상점씬 기준");
            switch (cardInfo.effectTarget) // 스킬 효과 적용 대상에 따른 탐색 범위 지정
            {
                case EffectTarget.ally:
                case EffectTarget.allyForward:
                case EffectTarget.allyBackward:
                case EffectTarget.both:
                    Debug.Log("상점씬 기준으로 searchArea에 추가");
                    for (int i = 0; i < GameMGR.Instance.spawner.cardBatch.Length; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] != null)
                        {
                            Debug.Log(i + "번째 카드배치에 녀석을 searchArea에 추가");
                            searchArea.Add(GameMGR.Instance.spawner.cardBatch[i]);
                        }
                    }
                    Debug.Log("카드배치 몹들 전부 추가 후 현재 searchArea 개수 :" + searchArea.Count);
                    break;
                case EffectTarget.none:
                    Debug.Log("아무 대상도 없다");
                    break;
            }
        }



        //=============================================================================================================================================================

        switch (cardInfo.targetType) // 구체적인 공격 대상 지정 ( 체력이 낮은, 공격력이 높은, 전열 등등)
        {

            case TargetType.self:
                skillTarget.Add(this);
                Debug.Log("대상은 나자신");
                break;

            case TargetType.empty: // 빈 공간을 찾는다 = 소환시
                bool isFind = false;
                targetPos = Vector2.zero; //위치값 초기화
                Debug.Log("대상은 빈칸");
                if (GameMGR.Instance.isBattleNow)    // 현재 상태가 전투씬인 경우
                {
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
                                isFind = true;
                                break;
                            }
                        }
                        if (!isFind)
                        {
                            // 튕겨나가는 애니메이션 출력하면서 어딘가에 소환했다가 삭제
                        }
                    }
                }
                else    // 현재 상태가 상점씬인 경우
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (GameMGR.Instance.spawner.cardBatch[i] == null)
                        {
                            targetPos = GameMGR.Instance.spawner.shopBatchPos[i].transform.position;
                            Debug.Log(targetPos + ": 타겟포즈");
                            shopBatchEmptyIndex = i; // 찾은 빈칸 인덱스 기록
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        // 튕겨나가는 애니메이션 출력하면서 어딘가에 소환했다가 삭제
                    }
                }
                break;

            case TargetType.random:
                Debug.Log("대상은 랜덤");
                int random = GameMGR.Instance.GetRandomValue(0, searchArea.Count);

                List<Card> targetArray1 = new List<Card>();
                for (int i = 0; i < searchArea.Count; i++)
                {
                    if (searchArea[i] != null)
                    {
                        targetArray1.Add(searchArea[i].GetComponentInChildren<Card>());
                    }
                }

                for (int i = 0; i < cardInfo.GetMaxTarget(cardInfo.level); i++)
                {
                    random = GameMGR.Instance.GetRandomValue(0, targetArray1.Count);

                    if (targetArray1.Count != 0)
                    {
                        if (skillTarget.Contains(targetArray1[random])) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                        {
                            i--;
                            continue;
                        }
                        skillTarget.Add(targetArray1[random]);
                        if (targetArray1.Count < cardInfo.GetMaxTarget(cardInfo.level)) break;
                        Debug.Log("skillTarget에 Add함");
                    }
                }

                //skillTarget.Add(GameMGR.Instance.battleLogic.)
                break;

            case TargetType.randomExceptMe:
                Debug.Log("대상은 날 제외한 랜덤");
                //if (searchArea.Count == 1) break;
                List<Card> targetArray = new List<Card>();
                Debug.Log(searchArea.Count);
                for (int i = 0; i < searchArea.Count; i++)
                {
                    if (searchArea[i] != null && searchArea[i] != this.transform.parent.gameObject)
                    {
                        targetArray.Add(searchArea[i].GetComponentInChildren<Card>());
                    }
                }
                Debug.Log(targetArray.Count);

                for (int i = 0; i < cardInfo.GetMaxTarget(cardInfo.level); i++)
                {
                    random = GameMGR.Instance.GetRandomValue(0, targetArray.Count);
                    if (targetArray.Count == 0) break;
                    if (skillTarget.Contains(targetArray[random])) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        i--;
                        continue;
                    }
                    skillTarget.Add(targetArray[random]);
                    if (targetArray.Count < cardInfo.GetMaxTarget(cardInfo.level)) break;
                    Debug.Log("skillTarget에 Add함");
                }
                break;

            case TargetType.forward:      // 전열ㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇㅈㅇ
                Debug.Log("대상은 전열");
                
                random = GameMGR.Instance.GetRandomValue(0, 3);
                bool isAllDead = true;
                for (int i = 0; i < 3; i++)
                {
                    if (searchArea[i].GetComponentInChildren<Card>().curHP >= 0)
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
                if (searchArea.Count == 0)
                {
                    skillTarget.Clear();
                }
                else // 한명이라도 살아있다면
                {
                    random = GameMGR.Instance.GetRandomValue(0, 3);
                    while (searchArea[random].GetComponentInChildren<Card>().curHP <= 0 && searchArea[random] == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = GameMGR.Instance.GetRandomValue(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponentInChildren<Card>());
                }
                break;

            case TargetType.backward:       // 후열 ㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇㅎㅇ
                Debug.Log("대상은 후열");
                random = GameMGR.Instance.GetRandomValue(0, 3);
                isAllDead = true;
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea[i].GetComponentInChildren<Card>().curHP >= 0)
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
                    random = GameMGR.Instance.GetRandomValue(0, 3);
                    while (searchArea[random].GetComponentInChildren<Card>().curHP <= 0 && searchArea[random] == this) // 죽은 아군이 아닐 때까지 랜덤값을 돌려
                    {
                        random = GameMGR.Instance.GetRandomValue(0, 3);
                    }
                    skillTarget.Add(searchArea[random].GetComponentInChildren<Card>());
                }
                break;

            case TargetType.front:  // 내 앞
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea[i].GetComponentInChildren<Card>() == this)
                    {
                        if (searchArea[i - 3].GetComponentInChildren<Card>() != null)
                        {
                            skillTarget.Add(searchArea[i - 3].GetComponentInChildren<Card>());
                        }
                    }
                }
                break;

            case TargetType.back:   // 내 뒤
                for (int i = 3; i < 6; i++)
                {
                    if (searchArea[i].GetComponentInChildren<Card>() == this)
                    {
                        if (searchArea[i + 3].GetComponentInChildren<Card>() != null)
                        {
                            skillTarget.Add(searchArea[i + 3].GetComponentInChildren<Card>());
                        }
                    }
                }
                break;

            case TargetType.near:   //인접
                for (int i = 0; i < 6; i++)
                {
                    if (searchArea[i].GetComponentInChildren<Card>() == this)
                    {
                        switch (i)
                        {
                            case 0:
                                skillTarget.Add(searchArea[1].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[3].GetComponentInChildren<Card>());
                                break;
                            case 1:
                                skillTarget.Add(searchArea[0].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[2].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[4].GetComponentInChildren<Card>());
                                break;
                            case 2:
                                skillTarget.Add(searchArea[1].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[5].GetComponentInChildren<Card>());
                                break;
                            case 3:
                                skillTarget.Add(searchArea[0].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[4].GetComponentInChildren<Card>());
                                break;
                            case 4:
                                skillTarget.Add(searchArea[1].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[3].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[5].GetComponentInChildren<Card>());
                                break;
                            case 5:
                                skillTarget.Add(searchArea[2].GetComponentInChildren<Card>());
                                skillTarget.Add(searchArea[4].GetComponentInChildren<Card>());
                                break;
                        }
                    }
                }
                break;

            case TargetType.otherSide:
                Debug.Log("대상은 내 반대편");
                int myPosNum = System.Array.IndexOf(GameMGR.Instance.battleLogic.playerAttackArray, this);
                if (GameMGR.Instance.battleLogic.enemyAttackArray[myPosNum] != null)
                {
                    skillTarget.Add(searchArea[myPosNum].GetComponentInChildren<Card>());
                }
                break;

            case TargetType.leastATK:
                Debug.Log("대상은 최소공격");
                // 가장 공격력이 낮은 대상을 찾아라아아아ㅏ아아아아아아아아아아아아ㅏ아아즈벡!야아아아ㅏ 발바리이 치와아아아아아아ㅏ
                int[] atkArray = new int[6];
                int leastAtk = -1;
                int validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponentInChildren<Card>().curHP <= 0) continue; // 죽은 녀석은 대상에서 제외한다.
                    if (leastAtk == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        leastAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponentInChildren<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponentInChildren<Card>().curAttackValue;
                        validIndex++;
                    }
                    if (validIndex != 0) if (atkArray[validIndex] < atkArray[0]) leastAtk = i;
                }
                skillTarget.Add(searchArea[leastAtk].gameObject.GetComponentInChildren<Card>());
                break;

            case TargetType.mostATK:
                Debug.Log("대상은 최대공격");
                atkArray = new int[6];
                int mostAtk = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponentInChildren<Card>().curHP <= 0) continue;
                    if (mostAtk == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        mostAtk = i;
                        atkArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponentInChildren<Card>().curAttackValue;
                        validIndex++;
                    }
                    else
                    {
                        atkArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponentInChildren<Card>().curAttackValue;
                        validIndex++;
                    }
                    if (validIndex != 0) if (atkArray[validIndex] > atkArray[0]) mostAtk = i;
                }
                skillTarget.Add(searchArea[mostAtk].GetComponentInChildren<Card>());
                break;

            case TargetType.leastHP:
                Debug.Log("대상은 최소체력");
                int[] hpArray = new int[6];
                int leastHp = -1;
                validIndex = 0; // 유효값이 있을 때마다 올라가는 인덱스 카운트 변수
                for (int i = 0; i < 6; i++) // 가장 공격력이 낮은 유닛을 찾는 과정
                {
                    if (searchArea[i].GetComponentInChildren<Card>().curHP <= 0) continue;
                    if (leastHp == -1) //아무것도 없을 때에는 최초로 들어온 녀석이 값을 받는다. 
                    {
                        leastHp = i;
                        hpArray[validIndex] = transform.parent.transform.GetChild(i).gameObject.GetComponentInChildren<Card>().curHP;
                        validIndex++;
                    }
                    else
                    {
                        hpArray[validIndex] = transform.parent.transform.GetChild(validIndex).gameObject.GetComponentInChildren<Card>().curHP;
                        validIndex++;
                    }
                    if (validIndex != 0) if (hpArray[validIndex] < hpArray[0]) leastHp = i;
                }
                skillTarget.Add(searchArea[leastHp].gameObject. GetComponentInChildren<Card>());
                break;

            case TargetType.mostHP:
                Debug.Log("대상은 최대체력");
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
                skillTarget.Add(searchArea[mostHp].GetComponentInChildren<Card>());
                break;


            default:
                break;

        }
    }
}

