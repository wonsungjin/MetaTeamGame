using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Spine.Unity;
public enum CardStatus
{
    Hp,
    Attack,
    Exp,
    Level,
}
public partial class Card : MonoBehaviour
{
    [SerializeField] public CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;
    public int curAttackValue;
    public int curHP;
    public int curEXP;
    SkeletonAnimation skeletonAnimation;

    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/

    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name.Replace("(Clone)", "")}");
        transform.GetChild(0).localScale = Vector3.one;
        hpText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
        atkText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshPro>();
        levelText = transform.GetChild(0).GetChild(5).GetComponent<TextMeshPro>();
        curHP = cardInfo.hp;
        hpText.text = curHP.ToString();
        curAttackValue = cardInfo.attackValue;
        atkText.text = curAttackValue.ToString();
        level = 1;
        levelText.text = level.ToString();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    public void SetFlip(bool isSet)
    {
        skeletonAnimation.SetFlip(isSet);
    }
    public void PlayAnimation(string ani,bool isSet = false)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, ani, isSet);
    }
    public void ChangeValue(CardStatus key, int value = 0)
    {
        if (key == CardStatus.Hp)
        {
            curHP = value;
            hpText.text = curHP.ToString();

        }
        else if (key == CardStatus.Attack)
        {
            curAttackValue = value;
            atkText.text = curAttackValue.ToString();

        }
        else if (key == CardStatus.Exp)
        {
            if (level == 1)
            {
                curEXP++;
                if (curEXP >= 2)
                {
                    ChangeValue(CardStatus.Level);
                }

            }
            else if (level == 2)
            {
                curEXP++;
                if (curEXP >= 3)
                {
                    ChangeValue(CardStatus.Level);
                }
            }
            else if (key == CardStatus.Level)
            {
                level++;
                levelText.text = level.ToString();
                GameMGR.Instance.spawner.SpecialMonster();
            }
        }
    }
    private void Awake()
    {
        SetMyInfo(name);
    }
}

