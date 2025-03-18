using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BgmAudio;
    public AudioSource SfxAudio;
    public AudioSource LoopSfxAudio;

    public AudioClip bgm;

    public AudioClip hitPlayer;
    public AudioClip fire;
    public AudioClip hitWall;
    public AudioClip hitArrow;
    public AudioClip dash;
    public AudioClip wallBroken;
    public AudioClip lightningReady;
    public AudioClip lightning;

    void Start()
    {
        //BgmAudio.clip = bgm;
        //BgmAudio.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        SfxAudio.PlayOneShot(clip);
    }
    public void PlayLoopSfx(AudioClip clip)
    {
        LoopSfxAudio.PlayOneShot(clip);
    }

    public void StopLoopSfx()
    {
        LoopSfxAudio.Stop();
    }
}
