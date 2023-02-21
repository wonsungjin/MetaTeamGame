using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

public class SpecialZone : MonoBehaviour
{
    public bool isNotSpawn = false;
    int num;
    public SpriteRenderer mySprite = null;
    [SerializeField] GameObject[] stars = null;
    private void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            mySprite.sprite = Resources.Load<Sprite>("FrameSp");
            gameObject.tag = "FullZone";

            int colTire = collision.gameObject.GetComponentInChildren<Card>().cardInfo.tier;

            switch (colTire)
            {
                case 1:
                    stars[0].SetActive(true);
                    stars[0].gameObject.layer = 2;
                    num = 0;
                    break;
                case 2:
                    stars[1].SetActive(true);
                    stars[1].gameObject.layer = 2;
                    num = 1;
                    break;
                case 3:
                    stars[2].SetActive(true);
                    stars[2].gameObject.layer = 2;
                    num = 2;
                    break;
                case 4:
                    stars[3].SetActive(true);
                    stars[3].gameObject.layer = 2;
                    num = 3;
                    break;
                case 5:
                    stars[4].SetActive(true);
                    stars[4].gameObject.layer = 2;
                    num = 4;
                    break;
                case 6:
                    stars[5].SetActive(true);
                    stars[5].gameObject.layer = 2;
                    num = 5;
                    break;
            }
        }

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "FullZone";
            gameObject.transform.localScale = new Vector3(0.245f, 0.245f, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameFreeze");
            isNotSpawn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            gameObject.tag = "SpecialZone";
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
            stars[num].SetActive(false);
        }

        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            gameObject.tag = "SpecialZone";
            isNotSpawn = false;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            mySprite.sprite = Resources.Load<Sprite>("FrameOff");
        }
    }
}
