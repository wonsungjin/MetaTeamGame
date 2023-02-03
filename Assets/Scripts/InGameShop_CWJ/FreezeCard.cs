using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FreezeCard"))
        {
            collision.gameObject.tag = "Monster";
        }

        else if (collision.gameObject.CompareTag("Monster"))
        {
            collision.gameObject.tag = "FreezeCard";
        }
    }
}
