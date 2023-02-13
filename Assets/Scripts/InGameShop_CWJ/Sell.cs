using MongoDB.Driver;
using UnityEngine;

public class Sell : MonoBehaviour
{
    AudioSource audioSource;
    bool isPlaying = false;

    private void Start()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "SellTXT";
        transform.GetChild(1).GetComponent<MeshRenderer>().sortingLayerName = "SellTXT";
        this.audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BattleMonster") || collision.CompareTag("BattleMonster2") || collision.CompareTag("BattleMonster3"))
        {
            isPlaying = true;
            Selld(collision);
            if (isPlaying == true)
                SoundStarts();

            GameMGR.Instance.objectPool.DestroyPrefab(collision.gameObject.transform.parent.gameObject);
            GameMGR.Instance.uiManager.OnEnter_Set_SkillExplantion(false, Vector3.zero);
            gameObject.SetActive(false);

        }
    }

    void SoundStarts()
    {
        this.audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.UI, "Public_landing");
        this.audioSource.Play();
        isPlaying = false;
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

        GameMGR.Instance.Event_Sell(coll.gameObject.GetComponent<Card>());
    }
}
