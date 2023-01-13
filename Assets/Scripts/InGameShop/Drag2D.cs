using System.Collections;
using UnityEngine;

public class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.11f);

    PolygonCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;
    Vector2 meltPos;

    float distance = 10;
    private bool isClickBool = false;
    private Vector3 mousePosition;
    private Vector3 objPosition;
    public bool isFreezen = false;



    private void Start()
    {
        spriteRenderer = GetComponent<MeshRenderer>();
        pol = GetComponent<PolygonCollider2D>();
        pos = this.gameObject.transform.position;
    }
    private void OnMouseEnter()
    {
        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.OnEnter_Set_SkillExplantion(true,mousePosition);

        }
    }
    private void OnMouseExit()
    {
        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.OnEnter_Set_SkillExplantion(false, mousePosition);

        }

    }
    private void OnMouseDrag()
    {
        mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnMouseDown()
    {
        isClickBool = false;
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
        if (gameObject.CompareTag("BattleMonster"))
        {
            // 잡고 있는 오브젝트가 배틀몬스터에 닿으면 위치값 서로 바뀜
            if (collision.gameObject.CompareTag("BattleMonster"))
            {
                collision.gameObject.transform.position = pos;
            }
        }

        if (isClickBool == true)
        {
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

                // 얼려있을 때 배틀존에 가면 구매 가능하게 하는 예외처리
                if (UIManager.Instance.goldCount > 0)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        meltPos = collision.gameObject.transform.position;
                        StartCoroutine(COR_BackMelt());
                    }
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

                if (gameObject.CompareTag("BattleMonster"))
                {
                    // 잡고 있는 오브젝트가 배틀몬스터에 닿으면 위치값 서로 바뀜
                    if (collision.gameObject.CompareTag("BattleMonster"))
                    {
                        collision.gameObject.transform.position = pos;
                    }
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
                    if (UIManager.Instance.goldCount >= 3)
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
        yield return wait;
        UIManager.Instance.sell.gameObject.SetActive(false);
    }
    // 원래 위치로 돌리는 함수
    private IEnumerator COR_BackAgain()
    {
        yield return wait;
        transform.position = pos;
    }
    // 프리즈카드로 바꾸는 함수
    IEnumerator COR_BackCard()
    {
        yield return wait;
        gameObject.tag = "FreezeCard";
    }
    // 얼린카드를 구매시 들어갈 함수 원래 자리로 돌아가 자리의 bool값을 바꾼후 구매된다.
    IEnumerator COR_BackMelt()
    {
        gameObject.tag = "MeltCard";
        pos = battleZonePos;
        yield return wait;
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.position = pos;
        spriteRenderer.sortingOrder = 3;
        UIManager.Instance.goldCount -= 3;
    }

    void SellButton()
    {
        UIManager.Instance.sell.gameObject.SetActive(false);
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
