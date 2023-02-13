using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    // 이 타일에 몬스터가 있는지?
    public bool isNotSpawn = false;
    public SpriteRenderer mySprite = null;

    public bool isNotMonster = false;

    public GameObject collisionObj;

    bool isEnter = false;
    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
            {
                this.isNotSpawn = true;
                gameObject.tag = "FullZone";
                gameObject.transform.localScale = new Vector3(0.245f, 0.245f, 1);
                mySprite.sprite = Resources.Load<Sprite>("FrameFreeze");
            }

            if (collisionObj == null || collisionObj == collision.gameObject && collision.gameObject.CompareTag("Monster"))
            {
                collisionObj = collision.gameObject;
                gameObject.tag = "FullZone";
                mySprite.sprite = Resources.Load<Sprite>("FrameOn");
                isNotMonster = true;
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
            gameObject.tag = "SelectRing";
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isNotMonster = false;
            //collisionObj = null;
            isEnter = false;
        }

        if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            this.isNotSpawn = false;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isEnter = false;
        }
    }
}
