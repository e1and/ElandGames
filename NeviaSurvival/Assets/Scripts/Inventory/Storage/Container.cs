using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour, IPointerClickHandler
{
    public Animator Animator;
    public bool isOpen;
    Storage Storage;
    Player player;
    public float openingTime;

    void Start()
    {
        player = FindObjectOfType<Player>();
        Animator = GetComponent<Animator>();
        if (TryGetComponent(out Storage storage))
        {
            Storage = storage;
            Storage.onAutoClose += AutoClose;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Storage.SelectStorage();
        //if (Vector3.Distance(Player.transform.position, transform.position) <= 3)
        //{
        //    OpenClose();
        //    if (isOpen) Storage.OpenStorage();
        //    if (!isOpen) Storage.CloseStorage();
        //}
    }

    public void Click()
    {
        Storage.SelectStorage();
        if (Vector3.Distance(player.transform.position, transform.position) <= 3)
        {
            OpenClose();
            if (isOpen) Storage.OpenStorage();
            if (!isOpen) Storage.CloseStorage();
        }
    }

    void OpenClose()
    {
        if (Animator != null) Animator.SetTrigger("Open");
        isOpen = !isOpen;
        Storage.isOpen = isOpen;
    }

    void AutoClose()
    {
        if (Storage.isOpen == true)
        {
            if (Animator != null) Animator.SetTrigger("Open");
            isOpen = false;
        }
    }
}
