using MongoDB.Driver;
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
    int num;

    [SerializeField] GameObject[] stars = null;
    [SerializeField] GameObject[] iceStars = null;

    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isEnter == false)
        {
            if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
            {
                this.isNotSpawn = true;
                gameObject.tag = "FullZone";
                gameObject.transform.localScale = new Vector3(0.245f, 0.245f, 1);
                mySprite.sprite = Resources.Load<Sprite>("FrameFreeze");

                ColliceStars(collision);
            }

            if (collisionObj == null || collisionObj == collision.gameObject && collision.gameObject.CompareTag("Monster"))
            {
                collisionObj = collision.gameObject;
                gameObject.tag = "FullZone";
                mySprite.sprite = Resources.Load<Sprite>("FrameOn");
                isNotMonster = true;

                CollStarts(collision);
            }
        }
    }

    void CollStarts(Collider2D collision)
    {
        int colTire = collision.gameObject.GetComponentInChildren<Card>().cardInfo.tier;

        switch (colTire)
        {
            case 1:
                stars[0].SetActive(true);
                num = 0;
                break;
            case 2:
                stars[1].SetActive(true);
                num = 1;
                break;
            case 3:
                stars[2].SetActive(true);
                num = 2;
                break;
            case 4:
                stars[3].SetActive(true);
                num = 3;
                break;
            case 5:
                stars[4].SetActive(true);
                num = 4;
                break;
            case 6:
                stars[5].SetActive(true);
                num = 5;
                break;
        }
    }

    void ColliceStars(Collider2D collision)
    {
        int colTire = collision.gameObject.GetComponentInChildren<Card>().cardInfo.tier;

        switch (colTire)
        {
            case 1:
                iceStars[0].SetActive(true);
                num = 0;
                break;
            case 2:
                iceStars[1].SetActive(true);
                num = 1;
                break;
            case 3:
                iceStars[2].SetActive(true);
                num = 2;
                break;
            case 4:
                iceStars[3].SetActive(true);
                num = 3;
                break;
            case 5:
                iceStars[4].SetActive(true);
                num = 4;
                break;
            case 6:
                iceStars[5].SetActive(true);
                num = 5;
                break;
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
            isEnter = false;
            stars[num].SetActive(false);
        }

        if (collisionObj == collision.gameObject && collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SelectRing";
            this.isNotSpawn = false;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            isEnter = false;
            iceStars[num].SetActive(false);
        }
    }
}
