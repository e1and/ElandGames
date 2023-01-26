using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Items { Stick, Axe, Torch, Key, BlueKey, Cauldron }

public class Inventory : MonoBehaviour
{
    public Action<Item> onItemAdded;

    [SerializeField] List<Item> StartItems = new List<Item>();
    
    public List<Item> inventoryItems = new List<Item>(9);
    public List<GameObject> inventoryItemObjects = new List<GameObject>(9);

    public Item LeftHand;
    public GameObject ItemInLeftHand;
    public Item RightHand;
    public GameObject ItemInRightHand;  

    public int size = 9;
    public int filledSlots;

    void Awake()
    {
        for (int i = 0; i < StartItems.Count; i++)
        {
            inventoryItems[i] = StartItems[i];
        }
    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = item;
                return;
            }
        }
        onItemAdded?.Invoke(item);
    }

    public void Recount()
    {
        filledSlots = 0;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] != null)
            {
                filledSlots++;
            }
        }
    }
}
