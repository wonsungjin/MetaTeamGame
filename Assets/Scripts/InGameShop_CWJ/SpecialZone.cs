using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialZone : MonoBehaviour
{
    public SpriteRenderer mySprite = null;


    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Monster") ||collision.CompareTag("FreezeCard"))
        {
            mySprite.sprite = Resources.Load<Sprite>("FrameSp");
            gameObject.tag = "FullZone";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.tag = "SpecialZone";
        mySprite.sprite = Resources.Load<Sprite>("FrameOff");
    }
}
