using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public bool isHere = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.isHere = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isHere = false;
        Debug.Log("³ª°¨");
    }
}
