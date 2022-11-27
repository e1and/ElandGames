using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorTrigger : MonoBehaviour
{
    //public Animator Animator;
    [SerializeField] GameObject Door;
    public DoorOpenDown doorScript;
    void Start()
    {
        //Animator = GetComponent<Animator>();
        doorScript = Door.GetComponent<DoorOpenDown>();
    }

    public void OpenDoor()
    { 
        doorScript.StartOpenDoor();
    }

}