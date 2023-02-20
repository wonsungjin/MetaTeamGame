using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public bool isHere = false;
    [SerializeField] int myNum; // 상점 유닛 배치 순서 (0~5)
    [SerializeField] GameObject aura;
    public GameObject myObj = null;

    Vector3 zPos = new Vector3(0,0,5f);


    private void Start()
    {
        aura.SetActive(true);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BattleMonster") || collision.gameObject.CompareTag("BattleMonster2") || collision.gameObject.CompareTag("BattleMonster3"))
        {
            aura.gameObject.SetActive(false);
            collision.gameObject.transform.parent.position = gameObject.transform.position + Vector3.down;
            collision.gameObject.GetComponent<Drag2D>().pos = this;
            this.isHere = true;
            this.gameObject.tag = "FullZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = collision.gameObject;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isHere = false;

        if (collision.gameObject.CompareTag("BattleMonster") || collision.gameObject.CompareTag("BattleMonster2") || collision.gameObject.CompareTag("BattleMonster3"))
        {
            aura.gameObject.SetActive(true);
            myObj = null;
            this.gameObject.tag = "BattleZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = null;
        }
    }
}
