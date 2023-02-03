using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CardStatus
{
    Hp,
    Attack,
    Exp,
    Level,
}
public class Card : MonoBehaviour
{
    [SerializeField] public CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;
    public int curAttackValue;
    public int curHP;
    public int curEXP;
    public Slider expSlider;
    SkeletonAnimation skeletonAnimation;


    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name.Replace("(Clone)", "")}");
        hpText = transform.parent.GetChild(1).GetChild(1).GetComponent<TextMeshPro>();
        atkText = transform.parent.GetChild(1).GetChild(3).GetComponent<TextMeshPro>();
        levelText = transform.parent.GetChild(1).GetChild(5).GetComponent<TextMeshPro>();
        expSlider = transform.parent.GetChild(1).GetChild(8).GetChild(0).GetComponent<Slider>();
        curHP = cardInfo.hp;
        hpText.text = curHP.ToString();
        curAttackValue = cardInfo.atk;
        atkText.text = curAttackValue.ToString();
        level = 1;
        levelText.text = level.ToString();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    public void SetFlip(bool isSet)
    {
        skeletonAnimation.SetFlip(isSet);
    }
    public void PlayAnimation(string ani, bool isSet = false)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, ani, isSet);
    }
    public void ChangeValue(CardStatus key, int value = 0)
    {
        switch (key)
        {
            case CardStatus.Hp:
                curHP = value;
                hpText.text = curHP.ToString();
                break;

            case CardStatus.Attack:
                curAttackValue = value;
                atkText.text = curAttackValue.ToString();
                break;

            case CardStatus.Exp:
                if (level == 1)
                {
                    curEXP++;
                    if (curEXP >= 2)
                    {
                        ChangeValue(CardStatus.Level);
                    }
                    else expSlider.value = curEXP * 0.5f;

                }
                else if (level == 2)
                {
                    curEXP++;
                    if (curEXP >= 3)
                    {
                        ChangeValue(CardStatus.Level);
                    }
                    else expSlider.value = curEXP * 0.33f;
                }
                break;

            case CardStatus.Level:
                expSlider.value = 0;
                level++;
                levelText.text = level.ToString();
                GameMGR.Instance.spawner.SpecialMonster();

                break;
        }
    }
    private void Awake()
    {
        SetMyInfo(name);
    }
}

