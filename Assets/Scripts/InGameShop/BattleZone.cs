using UnityEngine;

public class BattleZone : MonoBehaviour
{
    public bool isHere = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.isHere = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        this.isHere = false;
    }
}
