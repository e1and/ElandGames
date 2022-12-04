using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrel : MonoBehaviour, IPointerClickHandler
{
    public Animator Animator;
    public bool isOpen;
    Storage Storage;

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (TryGetComponent<Storage>(out Storage storage))
        {
            Storage = storage;
            Storage.onAutoClose += OpenClose;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenClose();
        if (isOpen) Storage.OpenStorage();
        if (!isOpen) Storage.CloseStorage();
    }

    void OpenClose()
    {
        Animator.SetTrigger("Open");
        isOpen = !isOpen;
        Debug.Log(isOpen);
    }
}
