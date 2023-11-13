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
    public AudioClip landing2;
    public AudioClip fallDamageSound;
    public AudioClip fallDamageSound2;
    public AudioClip[] skeletonSteps;
    public AudioClip[] keyOpenDoor;
    public AudioClip deathSound;
    public AudioClip death2Sound;
    public AudioClip gainXPSound;
    public AudioClip exploreSound;
    public AudioClip newQuestSound;
    public AudioClip levelUpSound;
    public AudioClip clickSound;
    public AudioClip pointSound;
    public AudioClip menuPointSound;
    public AudioClip startGameSound;
    public AudioClip startDaySound;
    public AudioClip startNightSound;
    public AudioClip bellSound;
    public AudioClip freezingTeeth;
    public AudioClip[] freezingSound;
    public AudioClip[] tiredSound;
    public AudioClip[] hungerSound;
    
    void Start()
    {
        links = FindObjectOfType<Links>();
        playerSound = links.player.GetComponent<AudioSource>();
    }

    public void FallSound(int index)
    {
        if (index == 0) gameSounds.PlayOneShot(landing);
        else gameSounds.PlayOneShot(landing2);
    }
    
    public void FallDamageSound(int index)
    {
        if (index == 0) gameSounds.PlayOneShot(fallDamageSound);
        else gameSounds.PlayOneShot(fallDamageSound2);
    }
    
    public void DeathSound()
    {
        gameSounds.PlayOneShot(deathSound);
        gameSounds.PlayOneShot(death2Sound);
    }

    public void GainXPSound()
    {
        gameSounds.PlayOneShot(gainXPSound);
    }
    
    public void LevelUpSound()
    {
        gameSounds.PlayOneShot(levelUpSound);
    }
    
    public void ExploreSound()
    {
        gameSounds.PlayOneShot(exploreSound);
    }
    
    public void NewQuestSound()
    {
        gameSounds.PlayOneShot(newQuestSound);
    }
    
    public void ClickSound()
    {
        gameSounds.PlayOneShot(clickSound);
    }

    public void PointSound()
    {
        gameSounds.PlayOneShot(pointSound);
    }
    
    public void MenuPointSound()
    {
        gameSounds.PlayOneShot(menuPointSound);
    }
    
    public void StartGameSound()
    {
        gameSounds.PlayOneShot(startGameSound);
    }    
    
    public void StartDaySound()
    {
        gameSounds.PlayOneShot(bellSound);
        gameSounds.PlayOneShot(startDaySound);
    }    
    
    public void StartNightSound()
    {
        gameSounds.PlayOneShot(bellSound);
        gameSounds.PlayOneShot(startNightSound);
    }    
    
    public void FreezingSound(int index)
    {
        if (freezingSound.Length >= index && freezingSound[index] != null) 
            gameSounds.PlayOneShot(freezingSound[index]);
    }

    private bool isFreezingPlay;
    public void Freezing(bool isFreezing)
    {
        if (isFreezing)
        {
            if (!isFreezingPlay)
            {
                isFreezingPlay = true;

                gameSounds.clip = freezingTeeth;
                gameSounds.loop = true;
                gameSounds.Play();
            }
        }
        else
        {
            if (isFreezingPlay)
            {
                isFreezingPlay = false;
                
                gameSounds.Stop();
            }
        }
    }
    
    public void TiredSound(int index)
    {
        if (tiredSound.Length >= index && tiredSound[index] != null) 
            gameSounds.PlayOneShot(tiredSound[index]);
    } 
    
    public void HungerSound(int index)
    {
        if (hungerSound.Length >= index && hungerSound[index] != null) 
            gameSounds.PlayOneShot(hungerSound[index]);
    } 

}
