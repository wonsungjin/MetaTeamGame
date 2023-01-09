using System.Collections;
using UnityEngine;

public class Drag2D : MonoBehaviour
{
    PolygonCollider2D pol;
    SpriteRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;

    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;

    private void Start()
    {
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
        // spriteRenderer.color = Color.red;
        pol.enabled = false;
        battleZonePos = pos;

        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.sell.gameObject.SetActive(true);
        }
    }

    private void OnMouseUp()
    {
        isClickBool = true;
        pol.enabled = true;

        if (this.gameObject.CompareTag("Monster") || this.gameObject.CompareTag("BattleMonster") || this.gameObject.CompareTag("FreezeCard"))
        {
            StartCoroutine(COR_BackAgain());
        }
        if (UIManager.Instance.sell.activeSelf == true)
        {
            StartCoroutine(COR_SellButton());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isClickBool == true)
        {
            // 잡고 있는 오브젝트가 배틀몬스터에 닿으면 위치값 서로 바뀜
            if (collision.gameObject.CompareTag("BattleMonster"))
            {
                collision.gameObject.transform.position = pos;
            }

            // 프리즈 카드를 잡았을 때
            if (gameObject.CompareTag("FreezeCard"))
            {
                // 프리즈 카드를 프리즈에 넣으면 녹은 후 원래 위치로 돌아가고
                // 멜트카드로 태그 변경이 되고 원래 위치로 돌아가면 다시 몬스터 상태가 된다. 
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    gameObject.tag = "MeltCard";
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isClickBool == true)
        {
            // 배틀 몬스터를 잡았을 때
            if (gameObject.CompareTag("BattleMonster"))
            {
                // 판매
                if (collision.gameObject.CompareTag("Sell"))
                {
                    UIManager.Instance.goldCount += 1;
                    SellButton();
                }

                // 잡고 있는 오브젝트가 배틀존에 닿으면 오브젝트 위치값 저장
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    pos = collision.gameObject.transform.position;
                }
            }

            // 몬스터를 잡았을 때
            if (gameObject.CompareTag("Monster"))
            {
                // 프리즈에 닿으면 프리즈카드로 태그를 바꾼 후 원래 위치로 돌린다.
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                    StartCoroutine(COR_BackCard());
                }
                // 몬스터가 배틀 존에 닿으면 골드가 차감 되고 배틀몬스터 태그로 바뀐다
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (UIManager.Instance.goldCount > 0)
                    {
                        spriteRenderer.sortingOrder = 3;
                        gameObject.tag = "BattleMonster";
                        UIManager.Instance.goldCount -= 3;
                        pos = collision.gameObject.transform.position;
                    }
                }
            }
        }
    }

    // 판매버튼 ON OFF
    IEnumerator COR_SellButton()
    {
        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.sell.gameObject.SetActive(false);
    }
    // 원래 위치로 돌리는 함수
    private IEnumerator COR_BackAgain()
    {
        yield return new WaitForSeconds(0.11f);
        transform.position = pos;
    }
    // 프리즈카드로 바꾸는 함수
    IEnumerator COR_BackCard()
    {
        yield return new WaitForSeconds(0.11f);
        gameObject.tag = "FreezeCard";
    }

    void SellButton()
    {
        Debug.Log("????????????");
        UIManager.Instance.sell.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
