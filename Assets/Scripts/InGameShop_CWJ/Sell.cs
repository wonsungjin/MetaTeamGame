using MongoDB.Driver;
using Unity.VisualScripting;
using UnityEngine;

public class Sell : MonoBehaviour
{
   
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

    void Selld(Collider2D coll)
    {
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
        GameMGR.Instance.Event_Sell(coll.gameObject.GetComponent<Card>());
    }
}
