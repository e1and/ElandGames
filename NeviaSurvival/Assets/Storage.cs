using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public Action<Item> onStorageItemAdded;
    public Action onOpenStorage;
    public Action onAutoClose;
    [SerializeField] StorageWindow storageWindow;
    [SerializeField] List<Item> StartRandomItems = new List<Item>();
    public bool isOpen;
    public List<Item> storageItems = new List<Item>(9);

    public int size = 6;
    public int filledSlots;

    void Awake()
    {
        for (int i = 0; i < StartRandomItems.Count; i++)
        {
            storageItems[i] = StartRandomItems[i];
        }

        onOpenStorage += AutoClose;
    }

    public void OpenStorage()
    {
        onOpenStorage.Invoke();
        storageWindow.targetStorage = this;
        storageWindow.gameObject.SetActive(true);
        isOpen = true;
        storageWindow.RedrawStorage();
    }

    public void CloseStorage()
    {
        storageWindow.gameObject.SetActive(false);
        isOpen = false;
    }

    void AutoClose()
    {
        StartCoroutine(AutoCloseCoroutine());
    }

    IEnumerator AutoCloseCoroutine()
    {
        while (Vector3.Distance(storageWindow.Player.gameObject.transform.position, transform.position) < 3)
        {
            yield return null;
        }
        CloseStorage();
        onAutoClose.Invoke();
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < storageItems.Count; i++)
        {
            if (storageItems[i] == null)
            {
                storageItems[i] = item;
                return;
            }
        }
        onStorageItemAdded?.Invoke(item);
    }

    public void Recount()
    {
        filledSlots = 0;
        for (int i = 0; i < storageItems.Count; i++)
        {
            if (storageItems[i] != null)
            {
                filledSlots++;
            }
        }
    }
}
