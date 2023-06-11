using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    Links links;
    public AudioSource audioSource;
    public SoundType soundType;
    public Animator animator;

    void Start()
    {
        links = FindObjectOfType<Links>();
        animator = GetComponent<Animator>();
        if (soundType == SoundType.Human) steps = links.sounds.stepSounds;
        if (soundType == SoundType.Skeleton) steps = links.sounds.skeletonSteps;
    }

    AudioClip[] steps;
    public void Step()
    {
        Debug.Log("step");
        if (animator.GetFloat("Speed") > 0.1f || animator.GetFloat("Velocity") > 0.1f)
        audioSource.PlayOneShot(steps[Random.Range(0, steps.Length)]);
    }

    public void Landing()
    {
            audioSource.PlayOneShot(links.sounds.landing);
    }
}
