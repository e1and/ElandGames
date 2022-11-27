using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    public Animator Animator;
    void Start()
    {
        Animator = GetComponent<Animator>(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    { Debug.Log("Open/Close"); Animator.SetTrigger("Open"); }

}
