using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField] public CardInfo cardInfo;
    public TextMeshPro hpText;
    public TextMeshPro atkText;
    public TextMeshPro levelText;
    public int level;
    public int curAttackValue;
    public int curHP;


    /*자신의 오브젝트 이름과 같은 스크립터블 데이터를 읽어와서 설정한다
    스프라이트 랜더러도 같은 원리로 설정*/
    public void SetMyInfo(string myname)
    {
        name = myname;
        cardInfo = Resources.Load<CardInfo>($"ScriptableDBs/{name.Replace("(Clone)","")}");
        hpText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
        atkText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshPro>();
        levelText = transform.GetChild(0).GetChild(5).GetComponent<TextMeshPro>();
        hpText.text = cardInfo.hp.ToString();
        atkText.text = cardInfo.attackValue.ToString();
        level = 1;
        levelText.text = level.ToString();
    }
    private void Awake()
    {
        SetMyInfo(name);
    }

}
