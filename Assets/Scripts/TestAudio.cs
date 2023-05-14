using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnclickBackgound()
    {
        audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Background, "Back1");
        audioSource.Play();
    }
    
    public void OnclickUnit()
    {
        audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Unit, "UISFX1");
        audioSource.Play();
    }

    public void OnclickUI()
    {
        audioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.UI, "USFX1");
        audioSource.Play();
    }
}
