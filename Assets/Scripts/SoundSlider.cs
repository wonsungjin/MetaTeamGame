using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider audioSlider;
    public Slider SfxSlider;

    public void SFXAudioControl(float sliderVal)
    {
        audioMixer.SetFloat("SFX", sliderVal);
    }
    public void BGMAudioControl(float sliderVal)
    {
        audioMixer.SetFloat("BGM", sliderVal);
    }
    public void MasterAudioControl(float sliderVal)
    {
        audioMixer.SetFloat("Master", sliderVal);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }

    public void AudioMute(bool i)
    {
        if (i == true)
        {
            AudioListener.volume = 0;
            audioSlider.value = float.MinValue;
        }

        else
        {
            AudioListener.volume = 1;
            audioSlider.value = float.MaxValue;

        }
    }

}