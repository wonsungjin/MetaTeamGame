using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public bool isHere = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.isHere = true;
            this.gameObject.tag = "FullZone";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isHere = false;

        if(collision.gameObject.CompareTag("BattleMonster"))
        this.gameObject.tag = "BattleZone";
    }
}
