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
    public AudioClip[] stepSounds;
    public AudioClip landing;
    public AudioClip[] skeletonSteps;
    public AudioClip[] keyOpenDoor;
    void Start()
    {
        links = FindObjectOfType<Links>();
        playerSound = links.player.GetComponent<AudioSource>();
    }


    void Update()
    {
        
    }


}
