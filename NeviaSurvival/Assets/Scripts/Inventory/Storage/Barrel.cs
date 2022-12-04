using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrel : MonoBehaviour, IPointerClickHandler
{
    public Animator Animator;
    public bool isOpen;
    Storage Storage;
    public Player Player;

    void Start()
    {
        Animator = GetComponent<Animator>();
        if (TryGetComponent<Storage>(out Storage storage))
        {
            Storage = storage;
            Storage.onAutoClose += AutoClose;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Storage.SelectStorage();
        if (Vector3.Distance(Player.transform.position, transform.position) <= 3)
        {
            OpenClose();
            if (isOpen) Storage.OpenStorage();
            if (!isOpen) Storage.CloseStorage();
        }
    }

    void OpenClose()
    {
        Animator.SetTrigger("Open");
        isOpen = !isOpen;
        Storage.isOpen = isOpen;
    }

    void AutoClose()
    {
        if (Storage.isOpen == true)
        {
            Animator.SetTrigger("Open");
            isOpen = false;
        }
    }
}
