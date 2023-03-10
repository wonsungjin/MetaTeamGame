using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMGR : MonoBehaviour
{
    // Clip ī�װ��� �з��� ���� Enum
    public enum Type { Background, Unit, UI, Effect };

    // Type �� Audio Clip �з�
    [SerializeField] AudioClip[] BackGroundClip = null;
    [SerializeField] AudioClip[] UnitSFXClip = null;
    [SerializeField] AudioClip[] UISFXClip = null;
    [SerializeField] AudioClip[] EffectSFXClip = null;

    AudioClip audioClip = null;
    AudioSource StoreAudioSource = null;
    AudioSource StoreBGM = null;

    [field: SerializeField] AudioMixerGroup SFXAudioMixer;
    [field: SerializeField] AudioMixerGroup BGMAudioMixer;

    public AudioSource BattleBGM = null;
    public AudioSource BattleAudio = null;

    // AudioClip Name, AudioClip���� Dictionary ����
    Dictionary<string, AudioClip> BackgroundDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UnitSFXDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> UISFXDic = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> EffectSFXDic = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        Init();
    }

    //  AudioClip Name�� Ű, AudioClip�� ������ Dictionary�� �߰� 
    private void Init()
    {
        StoreAudioSource = gameObject.GetComponent<AudioSource>();
        StoreBGM = gameObject.GetComponent<AudioSource>();
        BattleBGM = gameObject.GetComponent<AudioSource>();
        BattleAudio = gameObject.GetComponent<AudioSource>();


        for (int i = 0; i < BackGroundClip.Length; i++) { BackgroundDic.Add(BackGroundClip[i].name, BackGroundClip[i]); }
        for (int i = 0; i < UnitSFXClip.Length; i++) { UnitSFXDic.Add(UnitSFXClip[i].name, UnitSFXClip[i]); }
        for (int i = 0; i < UISFXClip.Length; i++) { UISFXDic.Add(UISFXClip[i].name, UISFXClip[i]); }
        for (int i = 0; i < EffectSFXClip.Length; i++) { EffectSFXDic.Add(EffectSFXClip[i].name, EffectSFXClip[i]); }
    }

    // Ÿ Ŭ�������� �Լ� ȣ�� �� Type, ClipName�� �´� AudioClip ��ȯ
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

    #region storeScene Audio
    public void SoundMouseClick()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "sweeping_sound");
        StoreAudioSource.Play();
    }

    public void SoundButton()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "button_sound");
        StoreAudioSource.Play();
    }

    public void SoundMonsterClick()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.Unit, "pick_sound");
        StoreAudioSource.Play();
    }

    public void StoreSceneBGM(bool isStoreScene)
    {
        StoreBGM = GameObject.Find("BackImage").GetComponent<AudioSource>();
        StoreBGM.clip = ReturnAudioClip(Type.Background, "MP_maintenanceBgm");
        StoreBGM.playOnAwake = isStoreScene;
        StoreBGM.loop = isStoreScene;

        StoreBGM.outputAudioMixerGroup = BGMAudioMixer;

        if (isStoreScene) { StoreBGM.Play(); }
        else if (!isStoreScene) { StoreBGM.Pause(); }
    }
    public void SoundSell()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "gold +1");
        StoreAudioSource.Play();
    }

    public void SoundBuy()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "gold -1");
        StoreAudioSource.Play();
    }

    public void SoundLevelUpButtonFail()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "fail_sound");
        StoreAudioSource.Play();
    }

    public void SoundLevelUpButton()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "StoreLevelup_sound");
        StoreAudioSource.Play();
    }

    public void SoundRefreshButton()
    {
        StoreAudioSource.outputAudioMixerGroup = SFXAudioMixer;
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "Refresh");
        StoreAudioSource.Play();
    }

    #endregion


    #region BattleScene Audio
    public void BattleAudioInit()
    {
        BattleBGM = GameObject.Find("BackGround").GetComponent<AudioSource>();
        BattleAudio = GameObject.Find("BattleAudio").GetComponent<AudioSource>();
        BattleAudio.playOnAwake = false;
        BattleAudio.loop = false;
    }

    // BGM
    public void BattleSceneBGM(bool isBattleScene)
    {
        BattleBGM.clip = ReturnAudioClip(Type.Background, "BattleBgm");
        BattleBGM.playOnAwake = isBattleScene;
        BattleBGM.loop = isBattleScene;
        BattleBGM.outputAudioMixerGroup = BGMAudioMixer;

        if (isBattleScene)
        {
            BattleBGM.Play();
        }
        else if (!isBattleScene) { BattleBGM.Pause(); }
    }

    // Win, Lose
    public void BattleRoundResult(bool isWin)
    {
        BattleAudio.outputAudioMixerGroup = SFXAudioMixer;
        if (isWin ==true)
        {
            BattleAudio.clip = ReturnAudioClip(Type.Effect, "GameWin");
        }
        else 
        {
            BattleAudio.clip = ReturnAudioClip(Type.Effect, "GameLose2");
        }
        Debug.LogError(BattleAudio.clip.name.ToString());
        BattleAudio.Play();

    }

    public void BattleAttackSound(int Damage)
    {
        if (Damage >= 15)
        {
            BattleAudio.outputAudioMixerGroup = SFXAudioMixer;
            BattleAudio.clip = ReturnAudioClip(Type.Unit, "Big_Attack");
            BattleAudio.Play();
        }

        else if (Damage < 15)
        {
            BattleAudio.outputAudioMixerGroup = SFXAudioMixer;
            BattleAudio.clip = ReturnAudioClip(Type.Unit, "SmallAttack");
            BattleAudio.Play();
        }
    }

    public void BattleUnitDeath()
    {
        BattleAudio.outputAudioMixerGroup = SFXAudioMixer;
        BattleAudio.clip = ReturnAudioClip(Type.Unit, "Dead");
        BattleAudio.Play();
    }

    public void BattleUnitHit()
    {
        BattleAudio.outputAudioMixerGroup = SFXAudioMixer;
        BattleAudio.clip = ReturnAudioClip(Type.Unit, "UnitSummoning");
        BattleAudio.Play();
    }

    #endregion
}