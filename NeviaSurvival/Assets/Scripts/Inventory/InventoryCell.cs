using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public event Action DropItem;
    
    public Inventory inventory;
    public InventoryWindow inventoryWindow;
    public Text Name;
    public int index;
    public GameObject Prefab;
    public bool isInRightHand;
    public bool isInLeftHand;
    public bool isEquiped;
    public Transform dropParent;

    private Transform _draggingParent;
    private Transform _originalParent;

    public Transform _rightHandParent;
    public Transform _leftHandParent;

    public void Equip()
    {

        if (isInLeftHand) { DragInLeftHand(); Debug.Log("equip"); }
        if (isInRightHand) { DragInRightHand(); Debug.Log("equip"); }
    }

    public void Init(Transform draggingParent)
    {
        _draggingParent = draggingParent;
        _originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        transform.parent = _draggingParent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        for (int i = 0; i < 9; i++)
        {
            if (InRectangle(inventoryWindow.slots[i]))
            { DragInSlot(inventoryWindow.slots[i]); return; }
        }
        if (InRectangle(inventoryWindow.rightHandSlot)) DragInRightHand();
        else if (InRectangle(inventoryWindow.leftHandSlot)) DragInLeftHand();
        else Drop();
    }

    private bool InRectangle(RectTransform rectangle)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectangle, transform.position);
    }

    private void DragInSlot(RectTransform slot)
    { 
        int moveToIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            if (inventoryWindow.slots[i] == slot) moveToIndex = i;
        }

        if (isInRightHand)
        {
            Destroy(inventory.ItemInRightHand);
        }
        else if (isInLeftHand) 
        {
            Destroy(inventory.ItemInLeftHand);
        }

        if (inventory.inventoryItems[moveToIndex] != null)
        {
            var prefab = slot.GetChild(0).GetComponent<InventoryCell>().Prefab;
            if (isInRightHand)
            {
                SpawnItem(prefab, _rightHandParent);
                slot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = true;
            }
            if (isInLeftHand)
            {
                SpawnItem(prefab, _leftHandParent);
                slot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = true;
            }
        }

        if (isInRightHand)
        {
            isInRightHand = false;
        }
        else if (isInLeftHand)
        {
            isInLeftHand = false;
        }

        SwapSlots(index, moveToIndex, slot);         

               
        transform.parent = slot;
        isEquiped = false;
    }

    void SwapSlots(int firstSlotIndex, int moveToIndex, RectTransform targetSlot)
    {
        if (inventory.inventoryItems[moveToIndex] != null && firstSlotIndex != moveToIndex)        // Если слот не пустой,
        {
            targetSlot.GetChild(0).gameObject.GetComponent<InventoryCell>().index = index;
            targetSlot.GetChild(0).SetParent(_originalParent);
        }                                                         // то переместить иконку предмета в слот перемещаемого предмета
        
        Item temp;
        temp = inventory.inventoryItems[moveToIndex];

        if (index < 100)
        {
            inventory.inventoryItems[moveToIndex] = inventory.inventoryItems[firstSlotIndex];
            inventory.inventoryItems[firstSlotIndex] = temp;
        }
        else if (index == 100)
        {
            inventory.inventoryItems[moveToIndex] = inventory.LeftHand;
            inventory.LeftHand = temp;
        }
        else if (index == 101)
        {
            inventory.inventoryItems[moveToIndex] = inventory.RightHand;
            inventory.RightHand = temp;
        }
        index = moveToIndex;
    }

    private void DragInRightHand()
    {
        if (inventory.RightHand != null && isInLeftHand)
        {
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = false;
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = true;
            inventoryWindow.rightHandSlot.GetChild(0).SetParent(_originalParent);
            SwapHands();
        }
        else if (inventory.RightHand != null)
        {
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = false;
            inventoryWindow.rightHandSlot.GetChild(0).SetParent(_originalParent);
            Destroy(inventory.ItemInRightHand);
        }

        if (inventory.ItemInRightHand == null || inventory.ItemInRightHand != null && !isInLeftHand)
        {
            SpawnItem(Prefab, _rightHandParent);
        }

        if (isInLeftHand) 
        { 
            isInLeftHand = false;
            Item temp;
            temp = inventory.RightHand;
            inventory.RightHand = inventory.LeftHand;
            inventory.LeftHand = temp;
            Destroy(inventory.ItemInLeftHand);
        }
        else 
        {
            Item temp;
            temp = inventory.RightHand;
            inventory.RightHand = inventory.inventoryItems[index];
            inventory.inventoryItems[index] = temp;
        }


        transform.parent = inventoryWindow.rightHandSlot;
        _originalParent = transform.parent;



        isInRightHand = true;
        isEquiped = true;
        index = 101;
        //inventoryWindow.Redraw();
    }

    private void DragInLeftHand()
    {
        if (inventory.LeftHand != null && isInRightHand)
        {
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = true;
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = false;
            inventoryWindow.leftHandSlot.GetChild(0).SetParent(_originalParent);
            SwapHands();
        }
        else if (inventory.LeftHand != null)
        {
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = false;
            inventoryWindow.leftHandSlot.GetChild(0).SetParent(_originalParent);
            Destroy(inventory.ItemInLeftHand);
        }

        if (inventory.ItemInLeftHand == null || inventory.ItemInLeftHand != null && !isInRightHand)
        {
            SpawnItem(Prefab, _leftHandParent);
        }

        if (isInRightHand)
        {
            isInRightHand = false;
            Item temp;
            temp = inventory.LeftHand;
            inventory.LeftHand = inventory.RightHand;
            inventory.RightHand = temp;
            Destroy(inventory.ItemInRightHand);
        }
        else
        {
            Item temp;
            temp = inventory.LeftHand;
            inventory.LeftHand = inventory.inventoryItems[index];
            inventory.inventoryItems[index] = temp;
        }

        transform.parent = inventoryWindow.leftHandSlot;
        _originalParent = transform.parent;


        isInLeftHand = true;
        isEquiped = true;
        index = 100;
        //inventoryWindow.Redraw();
    }

    void SpawnItem(GameObject prefab, Transform spawnPlace)
    {
        var item = Instantiate(prefab, spawnPlace);
        item.GetComponent<BoxCollider>().enabled = false;
        item.GetComponent<Rigidbody>().isKinematic = true;

        if (spawnPlace == _leftHandParent) inventory.ItemInLeftHand = item;
        else inventory.ItemInRightHand = item;
    }

    void SwapHands()
    {
        if (inventory.ItemInLeftHand != null && inventory.ItemInRightHand != null)
        {
            var tempItem = inventory.ItemInLeftHand;
            inventory.ItemInLeftHand = inventory.ItemInRightHand;
            inventory.ItemInRightHand = tempItem;

            inventory.ItemInLeftHand.transform.position = inventoryWindow._leftHandParent.position;
            inventory.ItemInRightHand.transform.position = inventoryWindow._rightHandParent.position;
        }
    }

    private void Drop()
    {
        var item = Instantiate(Prefab, dropParent);
        item.transform.position = inventory.gameObject.transform.position + new Vector3(0, 1, 0);
        DropItem?.Invoke();
        RemoveFromInventory();

    }

    void RemoveFromInventory()
    {
        Destroy(gameObject);

        if (isInRightHand) { Destroy(inventory.ItemInRightHand); inventory.RightHand = null; isInRightHand = false; }
        else if (isInLeftHand) { Destroy(inventory.ItemInLeftHand); inventory.LeftHand = null; isInLeftHand = false; }
        else
        {
            for (int i = 0; i < inventory.inventoryItems.Count; i++)
            {
                
                if (inventory.inventoryItems[i] != null)
                {
                    if (inventory.inventoryItems[i].Prefab.Equals(Prefab))
                    {
                        Debug.Log("Remove");
                        inventory.inventoryItems[i] = null;
                        inventory.Recount();
                        return;
                    }
                }
            }
        }
    }

}
