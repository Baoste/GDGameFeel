using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BgmAudio;
    public AudioSource SfxAudio;
    public AudioSource LoopSfxAudio1;
    public AudioSource LoopSfxAudio2;

    public AudioClip bgm;

    [Header("player")]
    public AudioClip ready;
    public AudioClip foot;
    public AudioClip pickup;
    public AudioClip fire;
    public AudioClip dash;
    public AudioClip playerFall;
    [Header("Arrow")]
    public AudioClip hitPlayer;
    public AudioClip hitArrow;
    public AudioClip hitWall;
    [Header("Env")]
    public AudioClip wallBroken;
    public AudioClip lightningReady;
    public AudioClip lightning;
    public AudioClip Explosion;

    void Start()
    {
        //BgmAudio.clip = bgm;
        //BgmAudio.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        SfxAudio.PlayOneShot(clip);
    }
    public void MuteSfx()
    {
        DOTween.To(() => SfxAudio.volume, x => SfxAudio.volume = x, 0, 1f);
    }

    public void PlayLoopSfx(int index, AudioClip clip)
    {
        if (index == 0)
        {
            LoopSfxAudio1.clip = clip;
            LoopSfxAudio1.Play();
        }
        else
        {
            LoopSfxAudio2.clip = clip;
            LoopSfxAudio2.Play();
        }
    }

    public void StopLoopSfx(int index)
    {
        if (index == 0)
            LoopSfxAudio1.Stop();
        else
            LoopSfxAudio2.Stop();
    }
}
