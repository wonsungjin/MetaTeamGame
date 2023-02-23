using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellInCollider : MonoBehaviour
{
    [SerializeField] GameObject sell = null;
    [SerializeField] GameObject[] specialZone = null;
    Collider2D[] specialColl = null;

    [SerializeField] GameObject[] nomalZone = null;
    Collider2D[] nomalColl = null;


    private void Start()
    {
        specialColl = new Collider2D[specialZone.Length];
        nomalColl = new Collider2D[nomalZone.Length];

        for (int i = 0; i < 2; i++)
        {
            specialColl[i] = specialZone[i].GetComponent<Collider2D>();
        }

        for (int i = 0; i < nomalZone.Length; i++)
        {
            nomalColl[i] = nomalZone[i].GetComponent<Collider2D>(); 
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
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = false;
            }

            for (int i = 0; i < nomalZone.Length; i++)
            {
                nomalColl[i].enabled = false;
            }
        }

        if (sell.activeSelf == false)
        {
            for (int i = 0; i < 2; i++)
            {
                specialColl[i].enabled = true;
            }

            for (int i = 0; i < nomalZone.Length; i++)
            {
                nomalColl[i].enabled = true;
            }
        }
    }

    public void CollOn()
    {
        for (int i = 0; i < 2; i++)
        {
            specialColl[i].enabled = true;
        }
    }

    public void NomalCollOn()
    {
        for (int i = 0; i < nomalZone.Length; i++)
        {
            nomalColl[i].enabled = true;
        }
    }
}
