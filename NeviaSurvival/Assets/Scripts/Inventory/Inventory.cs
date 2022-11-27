using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Action<Item> onItemAdded;

    [SerializeField] List<Item> StartItems = new List<Item>();
    [SerializeField] Item PlaceHolder;
    
    public List<Item> inventoryItems = new List<Item>(9);

    public Item LeftHand;
    public Item RightHand;

    public GameObject ItemInRightHand;
    public GameObject ItemInLeftHand;

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

    public void AddPlaceHolder()
    {
        AddItem(PlaceHolder);
    }
}
