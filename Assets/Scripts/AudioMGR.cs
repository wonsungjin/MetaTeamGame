using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

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

    AudioSource BattleBGM = null;
    AudioSource BattleAudio = null;

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
        StoreAudioSource = GetComponent<AudioSource>();

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

    public void StoreSceneBGM(bool isStoreScene)
    {
        StoreBGM = GameObject.Find("BackImage").GetComponent<AudioSource>();
        StoreBGM.clip = ReturnAudioClip(Type.Background, "Maintenance");
        StoreBGM.playOnAwake = isStoreScene;
        StoreBGM.loop = isStoreScene;

        if (isStoreScene) { StoreBGM.Play(); }
        else if (!isStoreScene) { StoreBGM.Pause(); }
    }
    public void SoundSell()
    {
        StoreAudioSource.clip = ReturnAudioClip(Type.UI, "Public_landing");
        StoreAudioSource.Play();
    }

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

        if (isBattleScene) { BattleBGM.Play(); }
        else if (!isBattleScene) { BattleBGM.Pause(); }
    }

    // Win, Lose
    public void BattleRoundResult(bool isResult)
    {
        if (isResult)
        {
            BattleAudio.clip = ReturnAudioClip(Type.Effect, "GameWin");
            BattleAudio.Play();
        }

        else if (!isResult)
        {
            BattleAudio.clip = ReturnAudioClip(Type.Effect, "GameLose");
            BattleAudio.Play();
        }
    }

    public void BattleAttackSound(int Damage)
    {
        if (Damage >= 15)
        {
            BattleAudio.clip = ReturnAudioClip(Type.Unit, "Big_Attack");
            BattleAudio.Play();
        }

        else if (Damage < 15)
        {
            BattleAudio.clip = ReturnAudioClip(Type.Unit, "SmallAttack");
            BattleAudio.Play();
        }
    }

    public void BattleUnitDeath()
    {
        BattleAudio.clip = ReturnAudioClip(Type.Unit, "Dead");
        BattleAudio.Play();
    }

    public void BattleUnitHit()
    {
        BattleAudio.clip = ReturnAudioClip(Type.Unit, "UnitSummoning");
        BattleAudio.Play();
    }

    #endregion
}
