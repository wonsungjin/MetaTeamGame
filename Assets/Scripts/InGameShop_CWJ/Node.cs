using MongoDB.Driver;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    // 이 타일에 몬스터가 있는지?
    public bool isNotSpawn = false;
    public SpriteRenderer mySprite = null;

    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
       
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        mySprite.sprite = Resources.Load<Sprite>("FrameOn");

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            this.isNotSpawn = true;

            gameObject.tag = "Rect";
        }

        if (collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "Rect";
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        mySprite.sprite = Resources.Load<Sprite>("FrameOff");

        if (collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "SelectRing";
        }

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            this.isNotSpawn = false;
        }
    }
}
