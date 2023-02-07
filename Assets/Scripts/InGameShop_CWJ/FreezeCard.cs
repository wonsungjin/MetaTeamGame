using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCard : MonoBehaviour
{
    AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FreezeCard"))
        {
            collision.gameObject.tag = "Monster";
            collision.gameObject.layer = 0;
            audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.UI, "Ice");
            audioSource.Play();
        }

        else if (collision.gameObject.CompareTag("Monster"))
        {
            collision.gameObject.tag = "FreezeCard";
            collision.gameObject.layer = 8;
            audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.UI, "Ice");
            audioSource.Play();
        }
    }
}
