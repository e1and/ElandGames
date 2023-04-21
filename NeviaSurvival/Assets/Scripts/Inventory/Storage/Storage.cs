using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public Action<Item> onStorageItemAdded;
    public Action onOpenStorage;
    public Action onAutoClose;
    StorageWindow storageWindow;
    InventoryWindow inventoryWindow;
    MousePoint mousePoint;
    public List<Item> startItems = new List<Item>();
    public bool isOpen;
    public List<Item> storageItems = new List<Item>(9);
    public List<GameObject> storageItemObjects = new List<GameObject>(9);

    public int size = 6;
    public int filledSlots;

    Links links;

    void Awake()
    {
        links = FindObjectOfType<Links>();
        storageWindow = links.storageWindow;
        inventoryWindow = links.inventoryWindow;
        mousePoint = links.mousePoint;

        for (int i = 0; i < startItems.Count; i++)
        {
            if (startItems[i] != null)
            storageItems[i] = startItems[i];
        }

        onOpenStorage += AutoClose;
        for (int i = 0; i < 9; i++)
        {
            storageItemObjects.Add(null);
        }
    }

    public void SelectStorage()
    {
        storageWindow.targetStorage = this;
    }

    public void OpenStorage()
    {
        Debug.Log("Open");
        onOpenStorage.Invoke();
        storageWindow.gameObject.SetActive(true);
        storageWindow.Redraw();
        inventoryWindow.Redraw();
    }

    public void CloseStorage()
    {
        Debug.Log("Close");
        storageWindow.gameObject.SetActive(false);
        mousePoint.isPointUI = false;
        mousePoint.IconHighLight.SetActive(false);
        mousePoint.itemInfoPanel.SetActive(false);
    }

    void AutoClose()
    { 
        StartCoroutine(AutoCloseCoroutine());
    }

    IEnumerator AutoCloseCoroutine()
    {
        while (Vector3.Distance(links.player.gameObject.transform.position, transform.position) < 3)
        {
            if (mousePoint.inputs.isPlayerMove || mousePoint.inputs.jump) break;
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
