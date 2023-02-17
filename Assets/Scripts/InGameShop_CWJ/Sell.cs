using MongoDB.Driver;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Sell : MonoBehaviour
{
    Card card;  // Sell 콜라이더에 닿은 유닛의 데이터를 담기 위한 멤버 변수
    private void Start()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "SellTXT";
        transform.GetChild(1).GetComponent<MeshRenderer>().sortingLayerName = "SellTXT";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BattleMonster") || collision.CompareTag("BattleMonster2") || collision.CompareTag("BattleMonster3"))
        {
            Selld(collision);
            GameMGR.Instance.objectPool.DestroyPrefab(collision.gameObject.transform.parent.gameObject);
            GameMGR.Instance.uiManager.OnEnter_Set_SkillExplantion(false, Vector3.zero);
            gameObject.SetActive(false);
        }
    }

    IEnumerator COR_ComBineMonsterEF()
    {
        GameObject mon = GameMGR.Instance.objectPool.CreatePrefab(Resources.Load<GameObject>("CFX3_Hit_SmokePuff"), gameObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        GameMGR.Instance.objectPool.DestroyPrefab(mon.transform.gameObject);
    }


    void Selld(Collider2D coll)
    {
        StartCoroutine(COR_ComBineMonsterEF());

        if (coll.CompareTag("BattleMonster"))
        {
            GameMGR.Instance.uiManager.goldCount += 1;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }

        if (coll.CompareTag("BattleMonster2"))
        {
            GameMGR.Instance.uiManager.goldCount += 2;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }

        if (coll.CompareTag("BattleMonster3"))
        {
            GameMGR.Instance.uiManager.goldCount += 3;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }
        // 오디오 매니저에서 실행한다 (여기서 하면 사라지기에)
        GameMGR.Instance.audioMGR.SoundSell();
        Debug.Log("sell Something");
        if (coll.GetComponentInChildren<Card>().cardInfo.skillTiming == SkillTiming.sell)
        {
            Debug.Log("my skillTiming as same as Sell");
            card = coll.GetComponentInChildren<Card>();
            card.SkillActive2(card);
            Debug.Log("Sell Skill Trigger On");
        }
        //GameMGR.Instance.Event_Sell(coll.gameObject.GetComponent<Card>());
    }
}
