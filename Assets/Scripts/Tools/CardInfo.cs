using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new CardData", menuName = "ScriptableObjects/CardData")]
public class CardInfo : ScriptableObject
{
    [SerializeField] internal string objName;
    [SerializeField] internal int level;
    [SerializeField] internal int tier;
    [SerializeField] internal int attackValue;
    [SerializeField] internal int hpValue;
    [SerializeField] internal string explanation;

}