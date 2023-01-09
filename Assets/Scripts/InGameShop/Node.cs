using UnityEngine;

public class Node : MonoBehaviour
{
    Spawner spawner;

    // 이 타일에 몬스터가 있는지?
    public bool isNotSpawn = false;

    private void Start()
    {
        spawner = FindObjectOfType<Spawner>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FreezeCard"))
        {
            this.isNotSpawn = true;
        }

        if (collision.gameObject.CompareTag("MeltCard"))
        {
            this.isNotSpawn = false;
            collision.gameObject.tag = "Monster";
        }
    }
}
