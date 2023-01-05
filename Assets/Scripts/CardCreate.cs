using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCreate : MonoBehaviour
{
    [SerializeField] CardInfo card; 
    void InstCard()
    {
        Instantiate(card);
    }
}
