using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(CompareTag("Monster") || CompareTag("FreezeCard"))
        {
            gameObject.tag = "FullZone";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.tag = "SpecialZone";
    }
}
