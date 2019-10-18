using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;
    
    public AudioClip boxSound;
    public AudioClip mainBGM;

    public AudioSource BgmSource;
    public AudioSource EFXSource;

    public enum BgmType
    {
        Normal,
        MadMovie,
        Dorororng,
        BamBamBaBam,
        DingDingDing,
        KingJS
    }

    [Serializable]
    public class BGM
    {
        public BgmType type;
        public AudioClip clip;
    }

    public List<BGM> BgmList;

    public float LowPitchRange = 0.95f;
    public float HighPictchRange = 1.05f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    public void PlaySingle(AudioClip clip)
    {
        EFXSource.clip = clip;
        EFXSource.Play();
    }

    public void ChangeBGM(BgmType type)
    {
        var c = BgmList.Find(q => q.type == type).clip;

        BgmSource.clip = c;
        BgmSource.loop = true;
        BgmSource.Play();
    }

    public void PlaySingle(AudioClip clip, Vector3 position, float volume = 1)
    {
        GameObject go = new GameObject("OneShotAudio" + DateTime.Now.ToString("yyyyMMddhhmmssf"));
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.clip = clip;
        source.volume = volume;
        if (volume > 1)
        {
            source.minDistance = volume;
        }
        source.Play();
        Destroy(go, clip.length);
    }
    
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        BgmSource.clip = clip;
        BgmSource.loop = loop;
        BgmSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = UnityEngine.Random.Range(0, clips.Length);
        float randomPitch = UnityEngine.Random.Range(LowPitchRange, HighPictchRange);
        EFXSource.pitch = randomPitch;
        EFXSource.clip = clips[randomIndex];
        EFXSource.Play();
    }
}