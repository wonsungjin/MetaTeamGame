using UnityEngine;

public class SpecialZone : MonoBehaviour
{
    public bool isNotSpawn = false;

    public SpriteRenderer mySprite = null;
    public GameObject collisionObj;

    public bool isNotMonster = false;

    bool isEnter = false;

    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isEnter == false)
        {
            if (collisionObj == null || collisionObj == collision.gameObject && collision.gameObject.CompareTag("Monster"))
            {
                mySprite.sprite = Resources.Load<Sprite>("FrameSp");
                gameObject.tag = "FullZone";
                isNotMonster = true;
                collisionObj = collision.gameObject;
            }

            if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
            {
                isNotMonster = true;
                gameObject.tag = "FullZone";
                gameObject.transform.localScale = new Vector3(0.245f, 0.245f, 1);
                mySprite.sprite = Resources.Load<Sprite>("FrameFreeze");
            }
        }
    }

    public void NullObj()
    {
        collisionObj = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "SpecialZone";
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isNotMonster = false;
            isEnter = false;
        }

        if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            isNotSpawn = false;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isEnter = false;
        }
    }
}
