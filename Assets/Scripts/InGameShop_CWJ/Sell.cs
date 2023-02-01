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
        if (collision.CompareTag("BattleMonster")) 
        {
            Selld();
            Destroy(collision.gameObject);
            gameObject.SetActive(false);
        }
    }

    void Selld()
    {
        if(CompareTag("BattleMonster"))
        {
            GameMGR.Instance.uiManager.goldCount += 1;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }

        else if (CompareTag("BattleMonster2"))
        {
            GameMGR.Instance.uiManager.goldCount += 2;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }

        else if (CompareTag("BattleMonster3"))
        {
            GameMGR.Instance.uiManager.goldCount += 3;
            GameMGR.Instance.uiManager.goldTXT.text = "" + GameMGR.Instance.uiManager.goldCount.ToString();
        }
    }
}
