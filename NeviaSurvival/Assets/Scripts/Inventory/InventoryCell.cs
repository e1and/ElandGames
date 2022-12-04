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
    public Storage storage;
    public StorageWindow storageWindow;
    public Text Name;
    public int index;
    public GameObject Prefab;
    public bool isInRightHand;
    public bool isInLeftHand;
    public bool isEquiped;
    public bool isInInventory;
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
        for (int i = 0; i < 9; i++)
        {
            if (InRectangle(storageWindow.slots[i]))
            { DragInStorage(storageWindow.slots[i]); return; }
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

        if (slot.childCount > 0)
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
            if (storage != null)
            {
                slot.GetChild(0).GetComponent<InventoryCell>().storage = storage;
            }
        }

        InventoryCell secondCell = null;
        if (slot.childCount > 0) secondCell = slot.GetChild(0).GetComponent<InventoryCell>();
        SwapSlots(this, secondCell, slot);         
    }

    private void DragInStorage(RectTransform slot)
    {
        storage = storageWindow.targetStorage;
        
        int moveToIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            if (storageWindow.slots[i] == slot) moveToIndex = i;
        }

        if (isInRightHand)
        {
            Destroy(inventory.ItemInRightHand);
        }
        else if (isInLeftHand)
        {
            Destroy(inventory.ItemInLeftHand);
        }

        if (slot.childCount > 0)
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

        InventoryCell secondCell = null;
        if (slot.childCount > 0) secondCell = slot.GetChild(0).GetComponent<InventoryCell>();
        SwapSlots(this, secondCell, slot);
    }

    void SwapSlots(InventoryCell firstCell, InventoryCell secondCell, RectTransform targetSlot)
    {
        if (targetSlot.childCount > 0)
        {
            if (firstCell != secondCell | isInInventory != targetSlot.GetChild(0).GetComponent<InventoryCell>().isInInventory)        // Если слот не пустой,
            {
                targetSlot.GetChild(0).parent = _originalParent;
            }                                                                               // то переместить иконку предмета в слот перемещаемого предмета
        }
        transform.parent = targetSlot;

        Item firstItem;
        Item secondItem = null;
        int targetIndex = targetSlot.GetComponent<InventorySlot>().indexSlot;
        int itemIndex = index;
        bool isFromStorage = !firstCell.isInInventory;
        bool isToStorage = targetSlot.GetComponent<InventorySlot>().isStorage;



        if (firstCell.isInLeftHand) { firstItem = inventory.LeftHand; }
        else if (firstCell.isInRightHand) { firstItem = inventory.RightHand; }
        else if (firstCell.isInInventory && !firstCell.isInLeftHand && !firstCell.isInRightHand) { firstItem = inventory.inventoryItems[firstCell.index]; Debug.Log(firstItem); }
        else { firstItem = storage.storageItems[firstCell.index]; Debug.Log(firstItem); }

            if (targetSlot.childCount > 0 && secondCell != null)
        {
            if (secondCell.isInInventory) { secondItem = inventory.inventoryItems[secondCell.index]; }
            else secondItem = storage.storageItems[secondCell.index];
        }

        if (itemIndex < 100)
        {
            if (isToStorage && isFromStorage)
            {
                storage.storageItems[targetIndex] = firstItem;
                storage.storageItems[itemIndex] = secondItem;
                firstCell.isInInventory = false;
            }
            else if (isToStorage && !isFromStorage)
            {
                storage.storageItems[targetIndex] = firstItem;
                inventory.inventoryItems[itemIndex] = secondItem;
                firstCell.isInInventory = false;
            }
            else if (!isToStorage && isFromStorage)
            {
                inventory.inventoryItems[targetIndex] = firstItem;
                storage.storageItems[itemIndex] = secondItem;
                firstCell.isInInventory = true;
            }
            else if (!isToStorage && !isFromStorage && firstItem != null)
            {
                inventory.inventoryItems[targetIndex] = firstItem;
                inventory.inventoryItems[itemIndex] = secondItem;
                firstCell.isInInventory = true;
            }
        }
        else if (itemIndex == 100)
        {
            if (isToStorage)
            {
                storage.storageItems[targetIndex] = inventory.LeftHand;
            }
            else
            {
                inventory.inventoryItems[targetIndex] = inventory.LeftHand;
            }
            inventory.LeftHand = null;
            firstCell.isInLeftHand = false;
        }
        else if (itemIndex == 101)
        {
            if (isToStorage)
            {
                storage.storageItems[targetIndex] = inventory.RightHand;
            }
            else
            {
                inventory.inventoryItems[targetIndex] = inventory.RightHand;
            }
            inventory.RightHand = null;
            firstCell.isInRightHand = false;
        }

        if (targetSlot.childCount > 0 && secondCell != null)
        {
            if (itemIndex < 100)
            {
                if (isToStorage && isFromStorage)
                {
                    storage.storageItems[targetIndex] = firstItem;
                    storage.storageItems[itemIndex] = secondItem;
                    firstCell.isInInventory = false;
                    secondCell.isInInventory = false;
                }
                else if (isToStorage && !isFromStorage)
                {
                    storage.storageItems[targetIndex] = firstItem;
                    inventory.inventoryItems[itemIndex] = secondItem;
                    firstCell.isInInventory = false;
                    secondCell.isInInventory = true;
                }
                else if (!isToStorage && isFromStorage)
                {
                    inventory.inventoryItems[targetIndex] = firstItem;
                    storage.storageItems[itemIndex] = secondItem;
                    firstCell.isInInventory = true;
                    secondCell.isInInventory = false;
                }
                else if (!isToStorage && !isFromStorage)
                {
                    inventory.inventoryItems[targetIndex] = firstItem;
                    inventory.inventoryItems[itemIndex] = secondItem;
                    firstCell.isInInventory = true;
                    secondCell.isInInventory = true;
                }
            }
            else if (itemIndex == 100)
            {
                if (isFromStorage)
                {
                    storage.storageItems[secondCell.index] = inventory.LeftHand;
                }
                else
                {
                    inventory.inventoryItems[secondCell.index] = inventory.LeftHand;
                }
                inventory.LeftHand = secondItem;
                secondCell.isInLeftHand = true;
            }
            else if (itemIndex == 101)
            {
                if (isFromStorage)
                {
                    storage.storageItems[secondCell.index] = inventory.RightHand;
                }
                else
                {
                    inventory.inventoryItems[secondCell.index] = inventory.RightHand;
                }
                inventory.RightHand = secondItem;
                secondCell.isInRightHand = true;
            }
            secondCell.index = itemIndex;
        }
        firstCell.index = targetIndex;

        if (isInRightHand)
        {
            isInRightHand = false;
        }
        else if (isInLeftHand)
        {
            isInLeftHand = false;
        }

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
            if (inventory.RightHand == null) Destroy(inventory.ItemInLeftHand);
            Item temp;
            temp = inventory.RightHand;
            inventory.RightHand = inventory.LeftHand;
            inventory.LeftHand = temp;
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
        else if (inventory.LeftHand != null && isInLeftHand)
        {
            transform.SetParent(_originalParent);
            return;
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
            if (inventory.LeftHand == null) Destroy(inventory.ItemInRightHand);
            Item temp;
            temp = inventory.LeftHand;
            inventory.LeftHand = inventory.RightHand;
            inventory.RightHand = temp;
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

            inventory.ItemInLeftHand.transform.parent = _leftHandParent;
            inventory.ItemInRightHand.transform.parent = _rightHandParent;
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
            if (storage == null)
            {
                for (int i = 0; i < inventory.size; i++)
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
            else
            {
                for (int i = 0; i < storage.size; i++)
                {
                    if (storage.storageItems[i] != null)
                    {
                        if (storage.storageItems[i].Prefab.Equals(Prefab))
                        {
                            Debug.Log("Remove");
                            storage.storageItems[i] = null;
                            storage.Recount();
                            return;
                        }
                    }
                }
            }
        }
    }

}
