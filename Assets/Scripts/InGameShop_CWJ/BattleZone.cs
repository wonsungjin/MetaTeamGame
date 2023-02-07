using UnityEngine;

public class BattleZone : MonoBehaviour
{
    AudioSource audioSource;
    public bool isHere = false;
    [SerializeField] int myNum; // 상점 유닛 배치 순서 (0~5)

    private void Start()
    {
        this.audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("FreezeCard"))
        {
            SoundStart();
        }

        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.isHere = true;
            this.gameObject.tag = "FullZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = collision.gameObject;
        }
    }

    void SoundStart()
    {
        this.audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.UI, "Public_landing");
        this.audioSource.Play();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        this.isHere = false;

        if (collision.gameObject.CompareTag("BattleMonster"))
        {
            this.gameObject.tag = "BattleZone";
            GameMGR.Instance.spawner.cardBatch[myNum] = null;
        }
    }
}
