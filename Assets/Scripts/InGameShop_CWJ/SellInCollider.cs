using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellInCollider : MonoBehaviour
{
    [SerializeField] GameObject sell = null;
    [SerializeField] GameObject[] specialZone = null;
    Collider2D[] specialColl = null;


    private void Start()
    {
        specialColl = new Collider2D[specialZone.Length];

        for (int i = 0; i < 2; i++)
        {
            specialColl[i] = specialZone[i].GetComponent<Collider2D>();
        }

        for (int i = 0; i < 2; i++)
        {
            specialColl[i].enabled = false;
        }
    }

    public void SellOn()
    {
        if (sell.activeSelf == true)
        {
            Debug.Log("현재 셀버튼이 트루다");
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = false;
            }
        }

        if (sell.activeSelf == false)
        {
            Debug.Log("현재 셀버튼이 폴스다");
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = true;
            }
        }
    }
}
