using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new CardData", menuName = "ScriptableObjects/CardData")]
public partial class CardInfo : ScriptableObject
{
    //858ed67e0d64b72429e8c773f1903334
    [SerializeField] internal int ID;
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int hp;
    [SerializeField] internal int atk;
    [SerializeField] internal string description;
    [SerializeField] internal int triggerTiming;
    [SerializeField] internal int targetType;
    [SerializeField] internal int target;
    [SerializeField] internal int min_Target;
    [SerializeField] internal int max_Target;
    [SerializeField] internal int triggerCondition;
    [SerializeField] internal int effectType;
    [SerializeField] internal int value_1;
    [SerializeField] internal int value_2;
    [SerializeField] internal int value_3;
    [SerializeField] internal string sumom_Unit;
    [SerializeField] internal int num_Triggers;
    [SerializeField] internal int duration;

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