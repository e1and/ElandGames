using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Human = 0,
    Skeleton = 1,
    Spider = 2
}

public class Sounds : MonoBehaviour
{
    Links links;
    public AudioSource playerSound;
    public AudioSource gameSounds;
    public AudioClip[] stepSounds;
    public AudioClip landing;
    public AudioClip[] skeletonSteps;
    public AudioClip[] keyOpenDoor;
    public AudioClip gainXPsound;
    public AudioClip levelUpSound;
    public AudioClip clickSound;
    public AudioClip pointSound;
    void Start()
    {
        links = FindObjectOfType<Links>();
        playerSound = links.player.GetComponent<AudioSource>();
    }


    public void GainXPSound()
    {
        gameSounds.PlayOneShot(gainXPsound);
    }
    
    public void LevelUpSound()
    {
        gameSounds.PlayOneShot(levelUpSound);
    }
    
    public void ClickSound()
    {
        gameSounds.PlayOneShot(clickSound);
    }

    public void PointSound()
    {
        gameSounds.PlayOneShot(pointSound);
    }


}
