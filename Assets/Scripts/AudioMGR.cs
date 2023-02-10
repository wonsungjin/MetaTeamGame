using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioMGR : MonoBehaviour
{
    // Clip 카테고리 분류를 위한 Enum
    public enum Type { Background, Unit, UI, Effect };

    // Type 별 Audio Clip 분류
    [SerializeField] AudioClip[] BackGroundClip = null;
    [SerializeField] AudioClip[] UnitSFXClip = null;
    [SerializeField] AudioClip[] UISFXClip = null;
    [SerializeField] AudioClip[] EffectSFXClip = null;

    AudioClip audioClip = null;
    AudioSource audioSource = null;

    // AudioClip Name, AudioClip으로 Dictionary 생성
    Dictionary<string, AudioClip> BackgroundDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UnitSFXDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UISFXDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> EffectSFXDic = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Init();
    }

    //  AudioClip Name을 키, AudioClip을 값으로 Dictionary에 추가 
    private void Init()
    {
        for (int i = 0; i < BackGroundClip.Length; i++)
        {
            BackgroundDic.Add(BackGroundClip[i].name, BackGroundClip[i]);
        }


        for (int i = 0; i < UnitSFXClip.Length; i++)
        {
            UnitSFXDic.Add(UnitSFXClip[i].name, UnitSFXClip[i]);
        }

        for (int i = 0; i < UISFXClip.Length; i++)
        {
            UISFXDic.Add(UISFXClip[i].name, UISFXClip[i]);
        }

        for (int i = 0; i < EffectSFXClip.Length; i++)
        {
            EffectSFXDic.Add(EffectSFXClip[i].name, EffectSFXClip[i]);
        }

        audioSource = GetComponent<AudioSource>();
    }

    // 타 클래스에서 함수 호출 시 Type, ClipName에 맞는 AudioClip 반환
    public AudioClip ReturnAudioClip(Type AudioType, string clipName)
    {
        switch (AudioType.ToString())
        {
            case "Background":
                audioClip = BackgroundDic[clipName];
                break;
            case "Unit":
                audioClip = UnitSFXDic[clipName];
                break;
            case "UI":
                audioClip = UISFXDic[clipName];
                break;
            case "Effect":
                audioClip = EffectSFXDic[clipName];
                break;
        }
        return audioClip;
    }

    public void SoundSell()
    {
        audioSource.clip = ReturnAudioClip(Type.UI, "Public_landing");
        audioSource.Play();
    }
}
