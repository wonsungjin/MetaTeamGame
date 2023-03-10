using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSound : MonoBehaviour
{
    AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void TimeSound()
    {
        m_AudioSource.clip = GameMGR.Instance.audioMGR.ReturnAudioClip(AudioMGR.Type.Effect, "MP_Ticking Clock");
        m_AudioSource.Play();
        m_AudioSource.loop = true;
    }

    public void TimeSoundEnd()
    {
        m_AudioSource.Stop();
        m_AudioSource.loop = false;
    }
}
