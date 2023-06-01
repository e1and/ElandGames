using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public Links links;
    public AudioClip soundtrack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerWarmCollider player))
        {
            links.music.isAreaMusic = true;
            links.music.AreaMusic(soundtrack);
            Debug.Log("EnterTrigger");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerWarmCollider player))
        {
            ExitZone();
            Debug.Log("Exit");
        }
    }

    public void ExitZone()
    {
        links.music.isAreaMusic = false;
        if (links.dayNight.isDay) links.music.DayMusic();
        else links.music.NightMusic();
    }
}
