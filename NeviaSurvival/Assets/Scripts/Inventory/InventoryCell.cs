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

        if (isInLeftHand) { PlaceInLeftHand(); Debug.Log("equip"); }
        if (isInRightHand) { PlaceInRightHand(); Debug.Log("equip"); }
    }

    public void Init(Transform draggingParent)
    {
        _draggingParent = draggingParent;
        _originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        int index = transform.GetSiblingIndex();
        transform.parent = _draggingParent;
        //inventory.AddPlaceHolder();
        //_originalParent.GetChild(_originalParent.transform.childCount - 1).SetSiblingIndex(index);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (In((RectTransform)_originalParent)) PlaceInInventory();
        else if (In((RectTransform)inventoryWindow.rightHandSlot)) PlaceInRightHand();
        else if (In((RectTransform)inventoryWindow.leftHandSlot)) PlaceInLeftHand();
        else Drop();
    }

    private bool In(RectTransform originalParent)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(originalParent, transform.position);
    }

    private void PlaceInInventory()
    {
        int closestIndex = 0;

        for (int i = 0; i < _originalParent.transform.childCount; i++)
        {
            if (Vector3.Distance(transform.position, _originalParent.GetChild(i).position) <
                 Vector3.Distance(transform.position, _originalParent.GetChild(closestIndex).position))
            {
                closestIndex = i;
            }
        }

        //if (inventory.filledSlots < inventory.size)
        //{
        //    for (int i = 0; i < inventory.inventoryItems.Count; i++)
        //    {
        //        if (inventory.inventoryItems[i] = null)
        //        {
        //            if (isInRightHand)
        //            {
        //                Debug.Log("2");
        //                inventory.inventoryItems[i] = inventory.RightHand;
        //                inventory.RightHand = null;
        //            }
        //            if (isInLeftHand)
        //            {
        //                Debug.Log("2");
        //                inventory.inventoryItems[i] = inventory.LeftHand;
        //                inventory.LeftHand = null;
        //            }
        //            return;
        //        }
        //    }
        //}

        if (isInRightHand) { Destroy(inventory.ItemInRightHand); isInRightHand = false; }
        if (isInLeftHand) { Destroy(inventory.ItemInLeftHand); isInLeftHand = false; }

        transform.parent = _originalParent;
        //transform.SetSiblingIndex(closestIndex);

        isEquiped = false;
        //Destroy(gameObject);

        inventoryWindow.Redraw();
    }

    private void PlaceInRightHand()
    {
        if (isInLeftHand) { isInLeftHand = false; }

        inventory.RightHand = inventory.inventoryItems[index];
        inventory.inventoryItems[index] = null;
        transform.parent = inventoryWindow.rightHandSlot;
        var item = Instantiate(Prefab, _rightHandParent);
        item.GetComponent<BoxCollider>().enabled = false;
        item.GetComponent<Rigidbody>().isKinematic = true;
        inventory.ItemInRightHand = item;
        isInRightHand = true;
        isEquiped = true;
        inventoryWindow.Redraw();
    }

    private void PlaceInLeftHand()
    {
        if (isInRightHand) { isInRightHand = false; }

        inventory.LeftHand = inventory.inventoryItems[index];
        inventory.inventoryItems[index] = null;
        transform.parent = inventoryWindow.leftHandSlot;
        var item = Instantiate(Prefab, _leftHandParent);
        item.GetComponent<BoxCollider>().enabled = false;
        item.GetComponent<Rigidbody>().isKinematic = true;
        inventory.ItemInLeftHand = item;
        isInLeftHand = true;
        isEquiped = true;
        inventoryWindow.Redraw();
    }

    void SwapHands()
    {
        if (inventory.ItemInLeftHand != null && inventory.ItemInRightHand != null)
        {
            var temp = inventory.LeftHand;
            inventory.LeftHand = inventory.RightHand;
            inventory.RightHand = temp;

            var tempItem = inventory.ItemInLeftHand;
            inventory.ItemInLeftHand = inventory.ItemInRightHand;
            inventory.ItemInRightHand = inventory.ItemInLeftHand;

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
