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
    public bool isSmallCauldron;

    Links links;

    void Awake()
    {
        links = FindObjectOfType<Links>();
        storageWindow = links.storageWindow;
        inventoryWindow = links.inventoryWindow;
        mousePoint = links.mousePoint;

        AddStartItems();

        onOpenStorage += AutoClose;
        for (int i = 0; i < 9; i++)
        {
            storageItemObjects.Add(null);
        }
    }

    private void Start()
    {
        CheckSaveList();
    }

    void AddStartItems()
    {
        if (!links.dayNight.isLoadGame)
            for (int i = 0; i < startItems.Count; i++)
            {
                if (startItems[i] != null)
                    storageItems[i] = startItems[i];
            }
    }

    void CheckSaveList()
    {
        // Если это заспавнившийся котелок, то его надо добавить в список хранилищ и при загрузке кго создавать
        if (!links.saveObjects.storages.Contains(this) && TryGetComponent(out Cauldron cauldron))
            links.saveObjects.storages.Add(this);

        if (!links.saveObjects.storages.Contains(this)) 
            Debug.LogError($"Контейнер { gameObject.name } из { gameObject.transform.parent.name } не добавлен в список сохраняемых объектов!");
    }

    public void SelectStorage()
    {
        storageWindow.targetStorage = this;
    }

    public void OpenStorage()
    {
        Debug.Log("Open");
        onOpenStorage?.Invoke();
        storageWindow.gameObject.SetActive(true);
        links.ui.inventoryPanel.SetActive(true);
        storageWindow.Redraw();
        inventoryWindow.Redraw();
        if (gameObject.TryGetComponent(out Cauldron cauldron) && cauldron.isCooking)
            links.ui.CookingIndicator.gameObject.SetActive(true);
    }

    public void CloseStorage()
    {
        Debug.Log("Close");
        storageWindow.gameObject.SetActive(false);
        mousePoint.isPointUI = false;
        mousePoint.IconHighLight.SetActive(false);
        mousePoint.itemInfoPanel.SetActive(false);
        links.ui.CookingIndicator.gameObject.SetActive(false);
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
