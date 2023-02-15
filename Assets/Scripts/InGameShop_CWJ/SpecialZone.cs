using UnityEngine;

public class SpecialZone : MonoBehaviour
{
    public bool isNotSpawn = false;

    public SpriteRenderer mySprite = null;

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
