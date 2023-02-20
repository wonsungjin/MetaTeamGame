using MongoDB.Driver;
using System.Collections;
using System.Data.Common;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.1f);

    Card card;
    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    public BattleZone pos;
    Vector2 selectZonePos;
    Vector3 monsterPos = new Vector3(0, -0.6f, 0);
    Vector3 monsterPos1 = new Vector3(0, 0.6f, 0);

    float timer = 0f;
    float distance = 10;
    private bool isClickBool = false;
    public bool isFreezen = false;
    bool isClickBattleMonster = false;

    private void OnEnable()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<MeshRenderer>();
        pol = GetComponent<BoxCollider2D>();
        card = GetComponent<Card>();
        if(this.transform.parent!=null)
        this.selectZonePos = this.transform.parent.position;
    }

    Camera mainCam = null;

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = mainCam.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(objPosition, Vector2.zero);

        if (CompareTag("BattleMonster") || CompareTag("BattleMonster2") || CompareTag("BattleMonster3")) transform.parent.position = objPosition + Vector3.down;
        else transform.parent.position = objPosition + monsterPos;

        // 드래그 할때 마다 레이를 쏴서 밑에 닿은 배틀몬스터를 다른 위치로 보냄
        if (isClickBattleMonster == true)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("BattleMonster") || hit.collider.CompareTag("BattleMonster2") || hit.collider.CompareTag("BattleMonster3"))
                {
                    if (hit.collider.name != this.gameObject.name)
                    {
                        GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                        hit.collider.gameObject.transform.parent.position = vec.transform.position + Vector3.down;
                        vec.GetComponent<BattleZone>().myObj = hit.collider.gameObject.transform.parent.gameObject;
                    }

                    else if (hit.collider.name == this.gameObject.name)
                    {
                        timer += Time.deltaTime;
                        if (timer > 1f)
                        {
                            GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                            hit.collider.gameObject.transform.parent.position = vec.transform.position + Vector3.down;
                            vec.GetComponent<BattleZone>().myObj = hit.collider.gameObject.transform.parent.gameObject;
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

        GameMGR.Instance.uiManager.OnEnter_Set_SkillExplantion(false, Vector3.zero);
        GameMGR.Instance.uiManager.SetisExplantionActive(true);

        if (this.gameObject.CompareTag("BattleMonster") || this.gameObject.CompareTag("BattleMonster2") || this.gameObject.CompareTag("BattleMonster3"))
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

        // 용병들 잡고 놓았을 때 원래 위치로 돌아간다
        if (this.gameObject.CompareTag("Monster"))
        {
            StartCoroutine(COR_BackAgain());
        }

        else if (this.gameObject.CompareTag("FreezeCard"))
        {
            StartCoroutine(COR_BackAgain());
        }

        else if (this.gameObject.CompareTag("BattleMonster") || this.gameObject.CompareTag("BattleMonster2") || this.gameObject.CompareTag("BattleMonster3"))
        {
            StartCoroutine(COR_SellButton());
            StartCoroutine(COR_BackAgain());
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
                if (GameMGR.Instance.uiManager.goldCount >= 3)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        GameMGR.Instance.audioMGR.SoundBuy();
                        Vector2 monTras = gameObject.transform.parent.localScale;
                        gameObject.transform.parent.localScale = monTras * 2;
                        BackMeltBuy(collision);
                        StartCoroutine(COR_BuyMonsterEF());
                    }
                }
            }

            // 상점에서 구매 할때 배틀존에 용병 넣으면 레벨업 된다.
            if (gameObject.CompareTag("Monster"))
            {
                // 프리즈에 닿으면 프리즈카드로 태그를 바꾼 후 원래 위치로 돌린다.
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                }

                // 몬스터가 배틀 존에 닿으면 골드가 차감 되고 배틀몬스터 태그로 바뀐다
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.audioMGR.SoundBuy();
                        spriteRenderer.sortingLayerName = "SellTXT";
                        gameObject.tag = "BattleMonster";
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        Vector2 monTras = gameObject.transform.parent.localScale;
                        gameObject.transform.parent.localScale = monTras * 2;
                        pos = collision.GetComponent<BattleZone>();
                        StartCoroutine(COR_BuyMonsterEF());

                        if (card.cardInfo.skillTiming == SkillTiming.buy)
                        {
                            card.SkillActive2(card);
                        }

                        //GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>()); //구매한 카드가 구매시 효과가 있다면 스킬 발동

                    }
                }

                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster") || gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster2"))
                {
                    // 상점에서 바로 레벨업하는 경우
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>()); //구매한 카드가 구매시 효과가 있다면 스킬 발동

                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        ShopCardLevelUp(collision.gameObject);

                        if (card.cardInfo.skillTiming == SkillTiming.buy)
                        {
                            card.SkillActive2(card);
                        }
                    }
                }
            }


            if (gameObject.name == collision.gameObject.name)
            {
                if (gameObject.CompareTag("BattleMonster2") && collision.gameObject.CompareTag("BattleMonster2"))
                {
                    if (collision.transform.position.y > transform.position.y)
                    {
                        ShopCardLevelUp(collision.gameObject);
                    }
                }

                if (gameObject.CompareTag("BattleMonster") || gameObject.CompareTag("BattleMonster2"))
                {
                    if (collision.gameObject.CompareTag("BattleMonster") || collision.gameObject.CompareTag("BattleMonster2"))
                    {
                        ShopCardLevelUp(collision.gameObject);
                    }
                }
            }

            if (collision.gameObject.CompareTag("BattleMonster3") || gameObject.CompareTag("BattleMonster3"))
            {
                return;
            }
        }
    }

    IEnumerator COR_BuyMonsterEF()
    {
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>("Heart"), gameObject.transform.position + monsterPos1, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        GameMGR.Instance.objectPool.DestroyPrefab(mon.transform.gameObject);
    }
 
    void ShopCardLevelUp(GameObject collision)
    {
        int colAttack = collision.gameObject.GetComponentInChildren<Card>().curAttackValue;
        int colHP = collision.gameObject.GetComponentInChildren<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;
        int thisExp = gameObject.GetComponent<Card>().curEXP;
        int thisLevel = gameObject.GetComponent<Card>().level;

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

        collision.gameObject.GetComponentInChildren<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.gameObject.GetComponentInChildren<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);

        if (thisLevel == 1)
        {
            thisExp += 1;
            collision.gameObject.GetComponentInChildren<Card>().ChangeValue(CardStatus.Exp, thisExp);
        }

        else if (thisLevel == 2)
        {
            thisExp += 3;
            collision.gameObject.GetComponentInChildren<Card>().ChangeValue(CardStatus.Exp, thisExp);
        }

        GameMGR.Instance.objectPool.DestroyPrefab(this.gameObject.transform.parent.gameObject);
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
    }

    // 판매버튼 ON OFF
    IEnumerator COR_SellButton()
    {
        yield return new WaitForSeconds(0.12f);
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
    }

    // 원래 위치로 돌리는 함수
    private IEnumerator COR_BackAgain()
    {
        yield return wait;

        if (CompareTag("BattleMonster") || CompareTag("BattleMonster2") || CompareTag("BattleMonster3"))
        {
            if (pos.myObj != null)
            {
                GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                if (vec != null)
                    gameObject.transform.parent.position = vec.transform.position + Vector3.down;
            }
            else
            {
                this.transform.parent.position = pos.gameObject.transform.position + Vector3.down;
                pos.myObj = gameObject.transform.parent.gameObject;
            }
        }

        else if (CompareTag("Monster"))
        {
            this.transform.parent.position = selectZonePos;
        }

        else if (CompareTag("FreezeCard"))
        {
            this.transform.parent.position = selectZonePos;
        }
    }

    void BackMeltBuy(Collider2D collision)
    {
        gameObject.tag = "BattleMonster";
        GameMGR.Instance.audioMGR.SoundSell();
        //Vector2 monTras = gameObject.transform.parent.localScale;
        //gameObject.transform.parent.localScale = monTras;
        pos = collision.GetComponent<BattleZone>();
        spriteRenderer.sortingOrder = 3;
        GameMGR.Instance.uiManager.goldCount -= 3;
        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();

        if (card.cardInfo.skillTiming == SkillTiming.buy)
        {
            card.SkillActive2(card);
        }
        //GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>());
    }
}
