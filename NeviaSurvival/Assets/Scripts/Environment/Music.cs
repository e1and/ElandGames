using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    Links links;
    public AudioSource music;
    public AudioClip dayMusic;
    public AudioClip nightMusic;
    public bool isAreaMusic;
    void Start()
    {
        links = FindObjectOfType<Links>();
    }

    public void DayMusic()
    {
        StartCoroutine(FadeOut(dayMusic));
    }

    public void NightMusic()
    {
        StartCoroutine(FadeOut(nightMusic));
    }

    public void AreaMusic(AudioClip clip)
    {
        StartCoroutine(FadeOut(clip));
    }

    IEnumerator FadeOut(AudioClip clip)
    {
        float thisVolume = music.volume;
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return null;
        }
        music.clip = clip;
        music.volume = thisVolume;
        music.Play();
    }
}
