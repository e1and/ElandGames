using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    public Animator Animator;
    public bool isOpen;
    Storage Storage;

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (TryGetComponent<Storage>(out Storage storage)) Storage = storage;
    }

    public void OnPointerClick(PointerEventData eventData)
    { 
        Debug.Log("Open/Close"); 
        Animator.SetTrigger("Open"); 
        isOpen = !isOpen;
        if (isOpen) Storage.OpenStorage();
        if (!isOpen) Storage.CloseStorage();
    }

}
