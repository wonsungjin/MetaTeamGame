using MongoDB.Driver;
using System.Collections;
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public partial class Drag2D : MonoBehaviour
{
    WaitForSeconds wait = new WaitForSeconds(0.1f);

    Card card;
    BoxCollider2D pol;
    MeshRenderer spriteRenderer;
    Vector2 pos;
    Vector2 selectZonePos;
    Vector2 meltPos;
    Vector3 monsterPos = new Vector3(0, -0.6f, 0);

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

        this.pos = this.gameObject.transform.parent.position;
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

        // �巡�� �Ҷ� ���� ���̸� ���� �ؿ� ���� ��Ʋ���͸� �ٸ� ��ġ�� ����
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
                    }

                    else if (hit.collider.name == this.gameObject.name)
                    {
                        timer += Time.deltaTime;
                        if (timer > 1f)
                        {
                            GameObject vec = GameObject.FindGameObjectWithTag("BattleZone");
                            hit.collider.gameObject.transform.parent.position = vec.transform.position + Vector3.down;
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

        // �뺴�� ��� ������ �� ���� ��ġ�� ���ư���
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
            // ������ ī�带 ����� ��
            if (gameObject.CompareTag("FreezeCard"))
            {
                // ������ ī�带 ����� ������ ���� �� ���� ��ġ�� ���ư���
                // ��Ʈī��� �±� ������ �ǰ� ���� ��ġ�� ���ư��� �ٽ� ���� ���°� �ȴ�. 
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                }

                // ������� �� ��Ʋ���� ���� ���� �����ϰ� �ϴ� ����ó��
                if (GameMGR.Instance.uiManager.goldCount >= 3)
                {
                    if (collision.gameObject.CompareTag("BattleZone"))
                    {
                        meltPos = collision.gameObject.transform.position;
                        Vector2 monTras = gameObject.transform.parent.localScale;
                        gameObject.transform.parent.localScale = monTras * 2;
                        BackMeltBuy();
                    }
                }
            }

            // �������� ���� �Ҷ� ��Ʋ���� �뺴 ������ ������ �ȴ�.
            if (gameObject.CompareTag("Monster"))
            {
                // ����� ������ ������ī��� �±׸� �ٲ� �� ���� ��ġ�� ������.
                if (collision.gameObject.CompareTag("Freeze"))
                {
                    StartCoroutine(COR_BackAgain());
                }

                // ���Ͱ� ��Ʋ ���� ������ ��尡 ���� �ǰ� ��Ʋ���� �±׷� �ٲ��
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.audioMGR.SoundBuy();
                        spriteRenderer.sortingLayerName = "SellTXT";
                        gameObject.tag = "BattleMonster";
                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        pos = collision.gameObject.transform.position;
                        Vector2 monTras = gameObject.transform.parent.localScale;
                        gameObject.transform.parent.localScale = monTras * 2;

                        //GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>()); //������ ī�尡 ���Ž� ȿ���� �ִٸ� ��ų �ߵ�
                        if (card.cardInfo.skillTiming == SkillTiming.buy)
                            card.SkillActive2(card);
                    }
                }

                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster") || gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster2"))
                {
                    // �������� �ٷ� �������ϴ� ���
                    if (GameMGR.Instance.uiManager.goldCount >= 3)
                    {
                        GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>()); //������ ī�尡 ���Ž� ȿ���� �ִٸ� ��ų �ߵ�

                        GameMGR.Instance.uiManager.goldCount -= 3;
                        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
                        ShopCardLevelUp(collision.gameObject);

                        if (card.cardInfo.skillTiming == SkillTiming.buy)
                            card.SkillActive2(card);
                    }
                }
            }

            if (gameObject.CompareTag("BattleMonster") || gameObject.CompareTag("BattleMonster2") || gameObject.CompareTag("BattleMonster3"))
            {
                // ��� �ִ� ������Ʈ�� ��Ʋ���� ������ ������Ʈ ��ġ�� ����
                if (collision.gameObject.CompareTag("BattleZone"))
                {
                    pos = collision.gameObject.transform.position;
                }

                if (gameObject.name == collision.gameObject.name && collision.gameObject.CompareTag("BattleMonster") || collision.gameObject.CompareTag("BattleMonster2"))
                {
                    ShopCardLevelUp(collision.gameObject);
                }
            }
        }
    }

    void ShopCardLevelUp(GameObject collision)
    {
        int colAttack = collision.gameObject.GetComponent<Card>().curAttackValue;
        int colHP = collision.gameObject.GetComponent<Card>().curHP;
        int attack = card.curAttackValue;
        int hP = card.curHP;
        int plusAttack = 0;
        int plusHp = 0;
        int thisExp = card.curEXP;
        int thisLevel = card.level;

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

        collision.gameObject.GetComponent<Card>().ChangeValue(CardStatus.Attack, plusAttack + 1);
        collision.gameObject.GetComponent<Card>().ChangeValue(CardStatus.Hp, plusHp + 1);

        if (thisLevel == 1)
        {
            thisExp += 1;
            collision.gameObject.GetComponent<Card>().ChangeValue(CardStatus.Exp, thisExp);
        }

        else if (thisLevel == 2)
        {
            thisExp += 2;
            collision.gameObject.GetComponent<Card>().ChangeValue(CardStatus.Exp, thisExp);
        }
        
        GameMGR.Instance.objectPool.DestroyPrefab(this.gameObject.transform.parent.gameObject);
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
    }

    // �ǸŹ�ư ON OFF
    IEnumerator COR_SellButton()
    {
        yield return new WaitForSeconds(0.12f);
        GameMGR.Instance.uiManager.sell.gameObject.SetActive(false);
    }

    // ���� ��ġ�� ������ �Լ�
    private IEnumerator COR_BackAgain()
    {
        yield return wait;

        if (CompareTag("BattleMonster") || CompareTag("BattleMonster2") || CompareTag("BattleMonster3"))
            this.transform.parent.position = pos + Vector2.down;

        else if (CompareTag("Monster"))
        {
            this.transform.parent.position = selectZonePos;
        }

        else if (CompareTag("FreezeCard"))
        {
            this.transform.parent.position = selectZonePos;
        }
    }

    void BackMeltBuy()
    {
        gameObject.tag = "BattleMonster";
        pos = meltPos;
        this.gameObject.transform.parent.position = pos + Vector2.down;
        spriteRenderer.sortingOrder = 3;
        GameMGR.Instance.uiManager.goldCount -= 3;
        GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();

        //GameMGR.Instance.Event_Buy(gameObject.GetComponent<Card>());
        if (card.cardInfo.skillTiming == SkillTiming.buy)
            card.SkillActive2(card);
    }
}
