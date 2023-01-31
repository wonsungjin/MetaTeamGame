using MongoDB.Driver;
using System.Collections;
//using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.1f);

    Card card;
    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 battleZonePos;
    Vector2 selectZonePos;
    Vector2 meltPos;
    Vector3 monsterPos = new Vector3(0, -0.6f, 0);
    Vector2 monsterBackPos = new Vector2(0, -0.6f);

    float timer = 0f;
    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;
    bool isClickBattleMonster = false;
    bool isSelectZone = false;

    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void Start()
    {
        spriteRenderer = GetComponent<MeshRenderer>();
        pol = GetComponent<BoxCollider2D>();
        pos = this.gameObject.transform.position;
        card = GetComponent<Card>();
    }

    Camera mainCam = null;
    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);
        
        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        if (CompareTag("BattleMonster")) transform.position = objPosition + Vector3.down;
        else transform.position = objPosition + monsterPos;

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
        UpdateOutline(true);
        isClickBool = false;
        pol.enabled = false;
        battleZonePos = this.gameObject.transform.position;
        GameMGR.Instance.uiManager.OnEnter_Set_SkillExplantion(false, Vector3.zero);
        GameMGR.Instance.uiManager.SetisExplantionActive(true);

        if (gameObject.CompareTag("BattleMonster"))
        {
            GameMGR.Instance.uiManager.sell.gameObject.SetActive(true);

            isClickBattleMonster = true;
        }
    }

    private void OnMouseUp()
    {
        UpdateOutline(false);
        isClickBool = true;
        pol.enabled = true;
        isClickBattleMonster = false;
        GameMGR.Instance.uiManager.SetisExplantionActive(false);

        if (this.gameObject.CompareTag("Monster") || this.gameObject.CompareTag("BattleMonster") || this.gameObject.CompareTag("FreezeCard"))
        {
            StartCoroutine(COR_BackAgain());
        }
        if (GameMGR.Instance.uiManager.sell.activeSelf == true)
        {
            StartCoroutine(COR_SellButton());
        }

        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
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
                if (GameMGR.Instance.uiManager.goldCount >= 3)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        meltPos = collision.gameObject.transform.position;
                        StartCoroutine(COR_BackMelt());
                    }
                }
            }

            if (gameObject.CompareTag("Monster"))
            {
                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster"))
                {
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        ShopCardLevelUp(collision);
                    }
                }
            }

            if (gameObject.CompareTag("BattleMonster"))
            {
                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster"))
                {
                    CardLevelUp(collision);
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
                    GameMGR.Instance.uiManager.goldCount += 1;
                    GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
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
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        spriteRenderer.sortingOrder = 3;
                        gameObject.tag = "BattleMonster";
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        pos = collision.gameObject.transform.position;
                        Vector2 monTras = gameObject.transform.localScale;
                        gameObject.transform.localScale = monTras * 2;
                    }
                }
            }
        }
        if(isSelectZone == false)
        {
            if (gameObject.CompareTag("Monster"))
            {
                selectZonePos = collision.gameObject.transform.position;
                isSelectZone = true;
            }
        }
    }

    // 카드 레벨업 
    void CardLevelUp(Collider2D collision)
    {
        int colAttack = collision.GetComponent<Card>().curAttackValue;
        int colHP = collision.GetComponent<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;

        if (colAttack > attack)
        {
            plusAttack = colAttack;
        }
        else if (colAttack <= attack)
        {
            plusAttack = attack;
        }

        if (colHP > hP)
        {
            plusHp = colHP;
        }
        else if (colHP <= hP)
        {
            plusHp = hP;
        }

        collision.GetComponent<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Exp, 1);

        if (collision.gameObject.transform.position.y > gameObject.transform.position.y)
        {
            CombineCard(collision);
        }
    }

    void ShopCardLevelUp(Collider2D collision)
    {
        int colAttack = collision.GetComponent<Card>().curAttackValue;
        int colHP = collision.GetComponent<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;

        if (colAttack > attack)
        {
            plusAttack = colAttack;
        }
        else if (colAttack <= attack)
        {
            plusAttack = attack;
        }

        if (colHP > hP)
        {
            plusHp = colHP;
        }
        else if (colHP <= hP)
        {
            plusHp = hP;
        }

        collision.GetComponent<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);
        collision.GetComponent<Card>().ChangeValue(CardStatus.Exp, 1);

        Destroy(this.gameObject);
    }

    // 용병 조합
    void CombineCard(Collider2D collision)
    {
        transform.position = collision.gameObject.transform.position;
        Destroy(collision.gameObject);
    }

    // 판매버튼 ON OFF
    IEnumerator COR_SellButton()
    {
        yield return wait;
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
    }

    // 원래 위치로 돌리는 함수
    private IEnumerator COR_BackAgain()
    {
        yield return wait;

        if (CompareTag("BattleMonster"))
            transform.position = pos + Vector2.down;

        else if (CompareTag("Monster"))
            transform.position = selectZonePos + monsterBackPos;

        else if (CompareTag("FreezeCard"))
        {
            transform.position = selectZonePos + monsterBackPos;
            gameObject.tag = "Monster";
        }
    }
    // 프리즈카드로 바꾸는 함수
    IEnumerator COR_BackCard()
    {
        yield return wait;
        gameObject.tag = "FreezeCard";
    }

    // 얼린카드를 구매시
    IEnumerator COR_BackMelt()
    {
        yield return wait;
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.position = pos + Vector2.down;
        spriteRenderer.sortingOrder = 3;
        GameMGR.Instance.uiManager.goldCount -= 3;
        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
    }

    void SellButton()
    {
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
