using Photon.Pun;
using Spine.Unity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public enum CardStatus
{
    Hp,
    Attack,
    Exp,
    Level,
}
public partial class Card : MonoBehaviourPun
{
    [SerializeField] public CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;
    public int curAttackValue;
    public int curHP;
    public int curEXP = 0;
    bool isLoop = false;
    public Slider expSlider;
    SkeletonAnimation skeletonAnimation;
    AudioSource audioSource;
    MeshRenderer spriteRenderer;

    Vector3 vec = new Vector3(0, 0.6f, 0);

    bool isSkillTiming = false;

    private void Awake()
    {
        SetMyInfo(name);
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname, bool flip = true)
    {
        if (transform.parent == null) return;
        name = myname;
        Debug.Log(name);
        curEXP = 0;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name.Replace("(Clone)", "")}");
        hpText = transform.parent.GetChild(1).GetChild(1).GetComponent<TextMeshPro>();
        atkText = transform.parent.GetChild(1).GetChild(3).GetComponent<TextMeshPro>();
        levelText = transform.parent.GetChild(1).GetChild(5).GetComponent<TextMeshPro>();
        expSlider = transform.parent.GetChild(1).GetChild(8).GetChild(0).GetComponent<Slider>();
        spriteRenderer = gameObject.GetComponent<MeshRenderer>();
        expSlider.value = 0;
        gameObject.tag = "Monster";
        curHP = cardInfo.hp;
        hpText.text = curHP.ToString();
        curAttackValue = cardInfo.atk;
        atkText.text = curAttackValue.ToString();
        level = 1;
        levelText.text = level.ToString();
        spriteRenderer.sortingLayerName = "Default";
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if(!isSkillTiming)
        SetSkillTiming();
        if (flip == true) SetFlip(false);
        transform.parent.gameObject.transform.localScale = Vector3.one;
    }
    public void ChangeCard(Card card)
    {
        ChangeValue(CardStatus.Hp, card.curHP);
        ChangeValue(CardStatus.Attack, card.curAttackValue);
        if (level == 2) expSlider.value = card.curEXP * 0.5f;
        else expSlider.value = card.curEXP * 0.33f;
        levelText.text = card.level.ToString();
    }

    public void SetAnim(string state)
    {
        // skeletonAnimation.AnimationState
        
        switch(state)
        {
            case "Idle":
                isLoop = true;
                break;
            case "Walk":
                isLoop = true;
                break;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, state, isLoop);
        isLoop = false;
    }

    public void SetFlip(bool isSet)
    {
        skeletonAnimation.SetFlip(isSet);
    }
    public void PlayAnimation(string ani, bool isSet = false)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, ani, isSet);
    }
    public void ChangeValue(CardStatus key, int value = 0, bool plus = false)
    {
        switch (key)
        {
            case CardStatus.Hp:
                if (plus == false) curHP = value;
                else
                {
                    curHP += value;
                }

                hpText.text = curHP.ToString();
                break;

            case CardStatus.Attack:
                if (plus == false) curAttackValue = value;
                else
                {
                    curAttackValue += value;
                }

                atkText.text = curAttackValue.ToString();
                break;

            case CardStatus.Exp:
                if (level == 1)
                {
                    audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Unit, "Merge_sound");
                    audioSource.Play();
                    StartCoroutine(COR_ComBineMonsterEF());

                    curEXP += value;
                    if (curEXP >= 2)
                    {
                        StartCoroutine(COR_LevelUpMonsterEF());
                        ChangeValue(CardStatus.Level);
                        gameObject.tag = "BattleMonster2";
                        audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Unit, "UnitLevelUP_sound");
                        audioSource.Play();
                    }
                    else expSlider.value = curEXP * 0.5f;

                }
                else if (level == 2)
                {
                    Debug.Log("2레벨에서 렙업");
                    curEXP += value;
                    audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Unit, "Merge_sound");
                    audioSource.Play();
                    StartCoroutine(COR_ComBineMonsterEF());

                    if (curEXP >= 3)
                    {
                        StartCoroutine(COR_LevelUpMonsterEF());
                        ChangeValue(CardStatus.Level);
                        gameObject.tag = "BattleMonster3";
                        audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Unit, "UnitLevelUP_sound");
                        audioSource.Play();
                       
                    }
                    else expSlider.value = curEXP * 0.33f;
                }
                break;

            case CardStatus.Level:
                curEXP = 0;
                expSlider.value = 0;
                level++;
                levelText.text = level.ToString();
                GameMGR.Instance.spawner.SpecialMonster();

                break;
        }
    }

    IEnumerator COR_ComBineMonsterEF()
    {
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>("skillAttack2"), gameObject.transform.position + vec, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        GameMGR.Instance.objectPool.DestroyPrefab(mon.transform.gameObject);
    }

    IEnumerator COR_LevelUpMonsterEF()
    {
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>("skillAttack"), gameObject.transform.position + vec, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        GameMGR.Instance.objectPool.DestroyPrefab(mon.transform.gameObject);
    }

}

