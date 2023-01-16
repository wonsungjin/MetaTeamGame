using MongoDB.Driver;
using System.Collections;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.11f);
    WaitForSeconds meltWait = new WaitForSeconds(0.1f);

    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;
    Vector2 meltPos;

    float timer = 0f;
    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;
    bool isClickBattleMonster = false;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void Start()
    {
        spriteRenderer = GetComponent<MeshRenderer>();
        pol = GetComponent<BoxCollider2D>();
        pos = this.gameObject.transform.position;
    }

    Camera mainCam = null;
    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition + Vector3.down;

        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        // 드래그 할때 마다 레이를 쏴서 밑에 닿은 배틀몬스터를 다른 위치로 보냄
        if (isClickBattleMonster == true)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("BattleMonster"))
                {
                    if (hit.collider.name != this.gameObject.name)
                    {
                        GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                        hit.collider.gameObject.transform.position = vec.transform.position + Vector3.down;
                    }

                    else if (hit.collider.name == this.gameObject.name)
                    {
                        timer += Time.deltaTime;
                        Debug.Log(timer);
                        if (timer > 1f)
                        {
                            GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                            hit.collider.gameObject.transform.position = vec.transform.position + Vector3.down;
                        }
                    }
                }
                else
                    timer = 0f;
            }
        }
    }

    private void OnMouseDown()
    {
        isClickBool = false;
        pol.enabled = false;
        battleZonePos = pos;

        if (gameObject.CompareTag("BattleMonster"))
        {
            UIManager.Instance.sell.gameObject.SetActive(true);

            isClickBattleMonster = true;
        }
    }

    private void OnMouseUp()
    {
        isClickBool = true;
        pol.enabled = true;
        isClickBattleMonster = false;

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
            // 프리즈 카드를 잡았을 때
            if (gameObject.CompareTag("FreezeCard"))
            {
                // 프리즈 카드를 프리즈에 넣으면 녹은 후 원래 위치로 돌아가고
                // 멜트카드로 태그 변경이 되고 원래 위치로 돌아가면 다시 몬스터 상태가 된다. 
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                }

                // 얼려있을 때 배틀존에 가면 구매 가능하게 하는 예외처리
                if (UIManager.Instance.goldCount >= 3)
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

        if (CompareTag("BattleMonster"))
            transform.position = pos + Vector2.down;

        else if (CompareTag("Monster"))
            transform.position = pos;

        if (CompareTag("FreezeCard"))
        {
            transform.position = pos;
            gameObject.tag = "MeltCard";
        }
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
        this.transform.position = battleZonePos;
        yield return meltWait;
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.position = pos + Vector2.down;
        spriteRenderer.sortingOrder = 3;
        UIManager.Instance.goldCount -= 3;
    }

    void SellButton()
    {
        UIManager.Instance.sell.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
