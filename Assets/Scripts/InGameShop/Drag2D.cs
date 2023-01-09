using System.Collections;
using UnityEngine;

public class Drag2D : MonoBehaviour
{
    BattleZone battleZone;
    PolygonCollider2D pol;
    SpriteRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZoneTempPos;
    Vector2 battleZonePos;
    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;

    private void Start()
    {
        battleZone = FindObjectOfType<BattleZone>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pol = GetComponent<PolygonCollider2D>();
        pos = this.gameObject.transform.position;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
    }

    private void OnMouseDown()
    {
        isClickBool = false;
        spriteRenderer.color = Color.red;
        pol.enabled = false;
    }

    private void OnMouseUp()
    {
        isClickBool = true;
        pol.enabled = true;

        spriteRenderer.color = Color.white;

        if (this.gameObject.CompareTag("Monster"))
        {
            StartCoroutine(COR_BackAgain());
        }
        if (this.gameObject.CompareTag("BattleMonster"))
        {
            StartCoroutine(COR_BackAgain());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isClickBool == true)
        {
            if (gameObject.CompareTag("BattleMonster"))
            {
                if (collision.gameObject.CompareTag("Sell"))
                {
                    UIManager.Instance.goldCount += 1;
                    Destroy(gameObject);
                }

                if (collision.gameObject.CompareTag("BattleMonster"))
                {
                    battleZoneTempPos = collision.gameObject.transform.position;
                    this.gameObject.transform.position = battleZoneTempPos;
                    collision.gameObject.transform.position = battleZonePos;
                }

                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (battleZone.isHere == false)
                    {
                        gameObject.tag = "BattleMonster";
                        pos = collision.gameObject.transform.position;
                        battleZonePos = pos;
                    }
                }
            }

            if (gameObject.CompareTag("Monster"))
            {
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    StartCoroutine(COR_BackCard());
                    spriteRenderer.color = Color.blue;
                }

                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (UIManager.Instance.goldCount > 0)
                    {
                        gameObject.tag = "BattleMonster";
                        UIManager.Instance.goldCount -= 3;
                        pos = collision.gameObject.transform.position;
                    }
                }
            }

            if (gameObject.CompareTag("FreezeCard"))
            {
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    gameObject.tag = "MeltCard";
                    spriteRenderer.color = Color.white;
                }
            }
        }
    }

    private IEnumerator COR_BackAgain()
    {
        yield return new WaitForSeconds(0.11f);
        transform.position = pos;
    }

    IEnumerator COR_BackCard()
    {
        yield return new WaitForSeconds(0.11f);
        gameObject.tag = "FreezeCard";
    }


    //void InstantiateSelectRing()
    //{
    //    if (!onSelected && !GameObject.FindGameObjectWithTag("SelectedRing"))
    //    {
    //        onSelected = true;
    //        nameBox.SetActive(true);
    //        Instantiate(seletedRingPrefabs, targetPosition, transform.rotation);
    //    }
    //    else if (onSelected && GameObject.FindGameObjectWithTag("SelectedRing"))
    //    {
    //        GameObject.FindGameObjectWithTag("SelectedRing").transform.position = targetPosition;
    //    }
    //}
}
