using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new CardData", menuName = "ScriptableObjects/CardData")]
public class CardInfo : ScriptableObject
{
    //858ed67e0d64b72429e8c773f1903334
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
    }

}