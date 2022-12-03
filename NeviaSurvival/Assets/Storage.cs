using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public Action<Item> onStorageItemAdded;
    public Action onOpenStorage;
    [SerializeField] StorageWindow storageWindow;
    [SerializeField] List<Item> StartRandomItems = new List<Item>();

    public List<Item> storageItems = new List<Item>(9);

    public int size = 6;
    public int filledSlots;

    void Awake()
    {
        for (int i = 0; i < StartRandomItems.Count; i++)
        {
            storageItems[i] = StartRandomItems[i];
        }
    }

    public void OpenStorage()
    {
        onOpenStorage.Invoke();
        storageWindow.targetStorage = this;
        storageWindow.gameObject.SetActive(true);
    }

    public void CloseStorage()
    {
        storageWindow.gameObject.SetActive(false);
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
