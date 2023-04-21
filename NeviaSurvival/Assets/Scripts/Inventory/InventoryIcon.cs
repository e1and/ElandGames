using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryIcon : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public event Action DropItem;

    public Item item;

    InventoryWindow inventoryWindow;
    public Storage storage;
    StorageWindow storageWindow;
    public Text Name;
    public int index;
    public GameObject Prefab;
    public GameObject item3dObject;
    [Header("�������������� �������� ������-�� �����")]
    public bool isInRightHand;
    public bool isInLeftHand;
    public bool isInBagSlot;
    public bool isInBeltSlot;
    public bool isInFeetSlot;
    public bool isInLegsSlot;
    public bool isInHandsSlot;
    public bool isInBodySlot;
    public bool isInShouldersSlot;
    public bool isInHeadSlot;
    [Header("��������� ��������")]
    public bool isEquiped;
    public bool isInInventory;

    private Transform draggingParent;
    public Transform originalParent;

    public Links links;

    //public Transform rightHandParent;
    //public Transform leftHandParent;
    //public Transform backpackParent;

    // � ������ ���������� 3� ������� � ������� �������������� ������ ���������, �����������, ���������
    // 100 - Left Hand, 101 - Right Hand, 102 - Backpack, 103 - Belt, 104 - Foots, 105 - Legs, 106 - Hands, 107 - Body, 
    // 108 - Shoulders, 109 - Head
    private void Start()
    {
        inventoryWindow = links.inventoryWindow;
        storageWindow = links.storageWindow;
        
        Name = GetComponentInChildren<Text>(); // ��������� ���� ������ ��������� ������ ������

        if (storage == null)
        {
            if (index == 100 && links.inventoryWindow.LeftHandObject != null)
            {
                item3dObject = links.inventoryWindow.LeftHandObject;
                item3dObject.SetActive(true);
            }
            else if (index == 101 && links.inventoryWindow.RightHandObject != null)
            {
                item3dObject = links.inventoryWindow.RightHandObject;
                item3dObject.SetActive(true);
            }
            else if (index == 102 && links.inventoryWindow.Backpack != null)
            {
                item3dObject = links.inventoryWindow.Backpack;
                item3dObject.SetActive(true);
            }
            else if (index == 103 && links.inventoryWindow.Belt != null)
            {
                item3dObject = links.inventoryWindow.Belt;
                item3dObject.SetActive(false);
            }
            else if (index == 104 && links.inventoryWindow.Feet != null)
            {
                item3dObject = links.inventoryWindow.Feet;
                item3dObject.SetActive(false);
            }
            else if (index == 105 && links.inventoryWindow.Legs != null)
            {
                item3dObject = links.inventoryWindow.Legs;
                item3dObject.SetActive(false);
            }
            else if (index == 106 && links.inventoryWindow.Arms != null)
            {
                item3dObject = links.inventoryWindow.Arms;
                item3dObject.SetActive(false);
            }
            else if (index == 107 && links.inventoryWindow.Body != null)
            {
                item3dObject = links.inventoryWindow.Body;
                item3dObject.SetActive(false);
            }
            else if (index == 108 && links.inventoryWindow.Shoulders != null)
            {
                item3dObject = links.inventoryWindow.Shoulders;
                item3dObject.SetActive(false);
            }
            else if (index == 109 && links.inventoryWindow.Head != null)
            {
                item3dObject = links.inventoryWindow.Head;
                item3dObject.SetActive(false);
            }
            else if (index < 9 && links.inventoryWindow.inventory.inventoryItemObjects[index] != null)
            {
                item3dObject = links.inventoryWindow.inventory.inventoryItemObjects[index];
                item3dObject.SetActive(false);
            }
            else
            {
                if (index < 103) item3dObject = Instantiate(Prefab, links.inventoryWindow.dropParent);
                if (index < 100) links.inventoryWindow.inventory.inventoryItemObjects[index] = item3dObject;
                else if (index == 100) links.inventoryWindow.LeftHandObject = item3dObject;
                else if (index == 101) links.inventoryWindow.RightHandObject = item3dObject;
                else if (index == 102) links.inventoryWindow.Backpack = item3dObject;
                item3dObject.SetActive(false);
            }
        }
        else
        {
            if (storage.storageItemObjects[index] != null)
            {
                item3dObject = storage.storageItemObjects[index];
            }
            else
            {
                item3dObject = Instantiate(Prefab, links.inventoryWindow.dropParent);
                if (index < 100) storage.storageItemObjects[index] = item3dObject;
                item3dObject.SetActive(false);
            }
        }

        if (index < 100) item3dObject.transform.parent = links.inventoryWindow.dropParent;
        else if (index == 100)
        {
            item3dObject.transform.parent = links.inventoryWindow.leftHandParent;
            item3dObject.transform.localPosition = Vector3.zero;
            item3dObject.transform.localRotation = Quaternion.identity;

            if (item3dObject.TryGetComponent(out HandPositions hand))
            {
                item3dObject.transform.localPosition = hand.LeftHandPosition;
                item3dObject.transform.localRotation = hand.LeftHandRotation;
            }
        }
        else if (index == 101)
        {
            item3dObject.transform.parent = links.inventoryWindow.rightHandParent;
            item3dObject.transform.localPosition = Vector3.zero;
            item3dObject.transform.localRotation = Quaternion.identity;
            if (item3dObject.TryGetComponent(out HandPositions hand))
            {
                item3dObject.transform.localPosition = hand.RightHandPosition;
                item3dObject.transform.localRotation = hand.RightHandRotation;
            }
        }
        else if (index == 102)
        {
            item3dObject.transform.parent = links.inventoryWindow.backpackParent;
            item3dObject.transform.localPosition = Vector3.zero;
            item3dObject.transform.localRotation = Quaternion.identity;
            if (item3dObject.TryGetComponent(out HandPositions hand))
            {
                item3dObject.transform.localPosition = hand.backpackPosition;
                item3dObject.transform.localRotation = hand.backpackRotation;
            }
        }


        item3dObject.GetComponent<Rigidbody>().isKinematic = true;
        item3dObject.GetComponent<BoxCollider>().enabled = false;


    }

    public void Init(Transform draggingParent)
    {
        this.draggingParent = draggingParent;
        originalParent = transform.parent;
    }

    // ��� ������ �������������� ������ ������ ������, � � �������� ������ ������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            originalParent = transform.parent;
            transform.parent = draggingParent;
        }
    }

    // ��� ������� ������ ���� ������ ��������������� ������ � ��������
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = Input.mousePosition;
        }
    }

    // ��� ���������� ������ ��� ������� ������ ��������� � ���� ��� ��������� �������
    public void OnEndDrag(PointerEventData eventData)
    {
        for (int i = 0; i < 9; i++)
        {
            if (InRectangle(links.inventoryWindow.slots[i]))
            { DragInSlot(links.inventoryWindow.slots[i]); return; }
        }
        for (int i = 0; i < 9; i++)
        {
            if (InRectangle(links.storageWindow.slots[i]) && links.storageWindow.gameObject.activeSelf == true)
            { DragInSlot(links.storageWindow.slots[i]); return; }
        }
        if (InRectangle(links.inventoryWindow.rightHandSlot)) DragInSlot(links.inventoryWindow.rightHandSlot);
        else if (InRectangle(links.inventoryWindow.leftHandSlot)) DragInSlot(links.inventoryWindow.leftHandSlot);
        else if (InRectangle(links.inventoryWindow.backpackSlot)) // ����������� ������ � ���� �������
        {
            if (item.Type == ItemType.Bag) DragInSlot(links.inventoryWindow.backpackSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� �������� ��� �� �����! ���� ���� ��� �������!");
            }
        }
        else if (InRectangle(links.inventoryWindow.beltSlot)) // ����������� ������ � ���� �����
        {
            if (item.Type == ItemType.Belt) DragInSlot(links.inventoryWindow.beltSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� �������� ��� �� ����! ���� ���� ��� �����!");
            }
        }
        else if (InRectangle(links.inventoryWindow.feetSlot)) // ����������� ������ � ���� �����
        {
            if (item.Type == ItemType.Feet) DragInSlot(links.inventoryWindow.feetSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� ������! ���� ���� ��� �����!");
            }
        }
        else if (InRectangle(links.inventoryWindow.legsSlot)) // ����������� ������ � ���� ���
        {
            if (item.Type == ItemType.Legs) DragInSlot(links.inventoryWindow.legsSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� ����! ���� ���� ��� ������!");
            }
        }
        else if (InRectangle(links.inventoryWindow.armsSlot)) // ����������� ������ � ���� ���
        {
            if (item.Type == ItemType.Hands) DragInSlot(links.inventoryWindow.armsSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� ����! ���� ���� ��� �������!");
            }
        }
        else if (InRectangle(links.inventoryWindow.bodySlot)) // ����������� ������ � ���� ����
        {
            if (item.Type == ItemType.Body) DragInSlot(links.inventoryWindow.bodySlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� ����! ���� ���� ��� ������ ��� �������!");
            }
        }
        else if (InRectangle(links.inventoryWindow.shouldersSlot)) // ����������� ������ � ���� ������
        {
            if (item.Type == ItemType.Shoulders) DragInSlot(links.inventoryWindow.shouldersSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� �����! ���� ���� ��� ����� ��� �����������!");
            }
        }
        else if (InRectangle(links.inventoryWindow.headSlot)) // ����������� ������ � ���� ������
        {
            if (item.Type == ItemType.Head) DragInSlot(links.inventoryWindow.headSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("�� ���� ������ ��� �� ������! ���� ���� ��� �����!");
            }
        }
        else Drop();
    }

    // ����� ������������, ��� ������ �� ������������ 2�-�������
    private bool InRectangle(RectTransform rectangle)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectangle, transform.position);
    }

    // �����, �������� ������� 3� ������� � Item � �� ������
    void Swap3dObjectsAndItems(int firstIndex, Storage firstStorage, int secondIndex, Storage secondStorage)
    {
        GameObject firstObject = null;
        GameObject secondObject = null;
        Item firstItem = null;
        Item secondItem = null;
        
        // ���������� �� ������� � ��������� ������ Item � ��� 3�-������
        if (firstIndex < 100)
        {
            if (firstStorage == null)
            {
                firstObject = inventoryWindow.inventory.inventoryItemObjects[firstIndex];
                firstItem = inventoryWindow.inventory.inventoryItems[firstIndex];
            }
            else
            {
                firstObject = firstStorage.storageItemObjects[firstIndex];
                firstItem = firstStorage.storageItems[firstIndex];
            }
        }
        else
        {
            if (firstIndex == 100) { firstObject = inventoryWindow.LeftHandObject; firstItem = inventoryWindow.LeftHandItem; }
            if (firstIndex == 101) { firstObject = inventoryWindow.RightHandObject; firstItem = inventoryWindow.RightHandItem; }
            if (firstIndex == 102) { firstObject = inventoryWindow.Backpack; firstItem = inventoryWindow.BackpackItem; }
            if (firstIndex == 103) { firstObject = inventoryWindow.Belt; firstItem = inventoryWindow.BeltItem; }
            if (firstIndex == 104) { firstObject = inventoryWindow.Feet; firstItem = inventoryWindow.FeetItem; }
            if (firstIndex == 105) { firstObject = inventoryWindow.Legs; firstItem = inventoryWindow.LegsItem; }
            if (firstIndex == 106) { firstObject = inventoryWindow.Arms; firstItem = inventoryWindow.ArmsItem; }
            if (firstIndex == 107) { firstObject = inventoryWindow.Body; firstItem = inventoryWindow.BodyItem; }
            if (firstIndex == 108) { firstObject = inventoryWindow.Shoulders; firstItem = inventoryWindow.ShouldersItem; }
            if (firstIndex == 109) { firstObject = inventoryWindow.Head; firstItem = inventoryWindow.HeadItem; }
        }
        // ���������� �� ������� � ��������� ������ Item � ��� 3�-������
        if (secondIndex < 100)
        {
            if (secondStorage == null)
            {
                secondObject = inventoryWindow.inventory.inventoryItemObjects[secondIndex];
                secondItem = inventoryWindow.inventory.inventoryItems[secondIndex];
            }
            else
            {
                secondObject = secondStorage.storageItemObjects[secondIndex];
                secondItem = secondStorage.storageItems[secondIndex];
            }
        }
        else
        {
            if (secondIndex == 100) { secondObject = inventoryWindow.LeftHandObject; secondItem = inventoryWindow.LeftHandItem; }
            if (secondIndex == 101) { secondObject = inventoryWindow.RightHandObject; secondItem = inventoryWindow.RightHandItem; }
            if (secondIndex == 102) { secondObject = inventoryWindow.Backpack; secondItem = inventoryWindow.BackpackItem; }
            if (secondIndex == 103) { secondObject = inventoryWindow.Belt; secondItem = inventoryWindow.BeltItem; }
            if (secondIndex == 104) { secondObject = inventoryWindow.Feet; secondItem = inventoryWindow.FeetItem; }
            if (secondIndex == 105) { secondObject = inventoryWindow.Legs; secondItem = inventoryWindow.LegsItem; }
            if (secondIndex == 106) { secondObject = inventoryWindow.Arms; secondItem = inventoryWindow.ArmsItem; }
            if (secondIndex == 107) { secondObject = inventoryWindow.Body; secondItem = inventoryWindow.BodyItem; }
            if (secondIndex == 108) { secondObject = inventoryWindow.Shoulders; secondItem = inventoryWindow.ShouldersItem; }
            if (secondIndex == 109) { secondObject = inventoryWindow.Head; secondItem = inventoryWindow.HeadItem; }
        }

        // ������ � ���� ���������
        if (firstIndex < 100 && firstStorage == null)
        {
            inventoryWindow.inventory.inventoryItemObjects[firstIndex] = secondObject;
            inventoryWindow.inventory.inventoryItems[firstIndex] = secondItem;
        }
        if (secondIndex < 100 && secondStorage == null)
        {
            inventoryWindow.inventory.inventoryItemObjects[secondIndex] = firstObject;
            inventoryWindow.inventory.inventoryItems[secondIndex] = firstItem;
        }

        // ������ � ���� ���������
        if (firstIndex < 100 && firstStorage != null)
        {
            firstStorage.storageItemObjects[firstIndex] = secondObject;
            firstStorage.storageItems[firstIndex] = secondItem;
        }
        if (secondIndex < 100 && secondStorage != null)
        {
            secondStorage.storageItemObjects[secondIndex] = firstObject;
            secondStorage.storageItems[secondIndex] = firstItem;
        }

        // ������ � ���� ����� ����
        if (firstIndex == 100 && firstStorage == null) { inventoryWindow.LeftHandObject = secondObject; inventoryWindow.LeftHandItem = secondItem; }
        if (secondIndex == 100 && secondStorage == null) { inventoryWindow.LeftHandObject = firstObject; inventoryWindow.LeftHandItem = firstItem; }

        // ������ � ���� ������ ����
        if (firstIndex == 101 && firstStorage == null) { inventoryWindow.RightHandObject = secondObject; inventoryWindow.RightHandItem = secondItem; }
        if (secondIndex == 101 && secondStorage == null) { inventoryWindow.RightHandObject = firstObject; inventoryWindow.RightHandItem = firstItem; }

        // ������ � ���� �������
        if (firstIndex == 102 && firstStorage == null) { inventoryWindow.Backpack = secondObject; inventoryWindow.BackpackItem = secondItem; }
        if (secondIndex == 102 && secondStorage == null) { inventoryWindow.Backpack = firstObject; inventoryWindow.BackpackItem = firstItem; }

        // ������ � ���� �����
        if (firstIndex == 103 && firstStorage == null) { inventoryWindow.Belt = secondObject; inventoryWindow.BeltItem = secondItem; }
        if (secondIndex == 103 && secondStorage == null) { inventoryWindow.Belt = firstObject; inventoryWindow.BeltItem = firstItem; }

        // ������ � ���� �������
        if (firstIndex == 104 && firstStorage == null) { inventoryWindow.Feet = secondObject; inventoryWindow.FeetItem = secondItem; }
        if (secondIndex == 104 && secondStorage == null) { inventoryWindow.Feet = firstObject; inventoryWindow.FeetItem = firstItem; }

        // ������ � ���� ���
        if (firstIndex == 105 && firstStorage == null) { inventoryWindow.Legs = secondObject; inventoryWindow.LegsItem = secondItem; }
        if (secondIndex == 105 && secondStorage == null) { inventoryWindow.Legs = firstObject; inventoryWindow.LegsItem = firstItem; }

        // ������ � ���� ���
        if (firstIndex == 106 && firstStorage == null) { inventoryWindow.Arms = secondObject; inventoryWindow.ArmsItem = secondItem; }
        if (secondIndex == 106 && secondStorage == null) { inventoryWindow.Arms = firstObject; inventoryWindow.ArmsItem = firstItem; }

        // ������ � ���� ����
        if (firstIndex == 107 && firstStorage == null) { inventoryWindow.Body = secondObject; inventoryWindow.BodyItem = secondItem; }
        if (secondIndex == 107 && secondStorage == null) { inventoryWindow.Body = firstObject; inventoryWindow.BodyItem = firstItem; }

        // ������ � ���� ������
        if (firstIndex == 108 && firstStorage == null) { inventoryWindow.Shoulders = secondObject; inventoryWindow.ShouldersItem = secondItem; }
        if (secondIndex == 108 && secondStorage == null) { inventoryWindow.Shoulders = firstObject; inventoryWindow.ShouldersItem = firstItem; }

        // ������ � ���� ������
        if (firstIndex == 109 && firstStorage == null) { inventoryWindow.Head = secondObject; inventoryWindow.HeadItem = secondItem; }
        if (secondIndex == 109 && secondStorage == null) { inventoryWindow.Head = firstObject; inventoryWindow.HeadItem = firstItem; }

        inventoryWindow.Redraw();
    }

    private void DragInSlot(RectTransform slot)
    {
        // ���������� ����� �����, ���� ���������� � ����������� �� ���� �������� ���������
        int moveToIndex = slot.GetComponent<InventorySlot>().indexSlot;
        Storage toStorage = null;
        if (slot.GetComponent<InventorySlot>().isStorage) toStorage = storageWindow.targetStorage;

        Item itemInSlot = null;
        if (slot.childCount > 0) itemInSlot = slot.GetChild(0).GetComponent<InventoryIcon>().item;

        if (item.Type == ItemType.Bag && toStorage == null && moveToIndex < 16)
        {
            transform.parent = originalParent;
            links.mousePoint.Comment("�������� ����� � ����� ������!");
            storageWindow.Redraw();
            inventoryWindow.Redraw();
            return;
        }
        
        if (isInRightHand) // ���� �� ������ ����, �� ������ ������ 3�-�������� � Item ������� � ������ �������
        {
            inventoryWindow.RightHandObject.SetActive(false);

            Swap3dObjectsAndItems(101, null, moveToIndex, toStorage);
        }
        else if (isInLeftHand) // ���� �� ����� ����, �� ������ ������ 3� �������� ������� � ������ �������
        {
            inventoryWindow.LeftHandObject.SetActive(false);

            Swap3dObjectsAndItems(100, null, moveToIndex, toStorage);
        }
        else if (isInBagSlot) // ���� �� ����� �������, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Bag) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ ������ ����� ������ ������� �� ���������!");
                return;
            }
            
            if (toStorage != null || toStorage == null && moveToIndex >= 100)
            {
                inventoryWindow.Backpack.SetActive(false);
                Swap3dObjectsAndItems(102, null, moveToIndex, toStorage);
            }
            else 
            { 
                transform.parent = originalParent;
                links.mousePoint.Comment("�������� ����� ���� � ����? ���? � ������� - �����?!");
            }
        }
        else if (isInBeltSlot) // ���� �� ����� �����, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Belt) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ ������ ����� ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Belt.SetActive(false);
            Swap3dObjectsAndItems(103, null, moveToIndex, toStorage);
        }
        else if (isInFeetSlot) // ���� �� ����� �������, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Feet) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ ������ ����� ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Feet.SetActive(false);
            Swap3dObjectsAndItems(104, null, moveToIndex, toStorage);
        }
        else if (isInLegsSlot) // ���� �� ����� ���, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Legs) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ ������ ������ ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Legs.SetActive(false);
            Swap3dObjectsAndItems(105, null, moveToIndex, toStorage);
        }
        else if (isInHandsSlot) // ���� �� ����� ���, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Hands) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ ������ ������� ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Arms.SetActive(false);
            Swap3dObjectsAndItems(106, null, moveToIndex, toStorage);
        }
        else if (isInBodySlot) // ���� �� ����� ����, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Body) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ �� ���� ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Body.SetActive(false);
            Swap3dObjectsAndItems(107, null, moveToIndex, toStorage);
        }
        else if (isInShouldersSlot) // ���� �� ����� ������, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Shoulders) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ �� ����� ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Shoulders.SetActive(false);
            Swap3dObjectsAndItems(108, null, moveToIndex, toStorage);
        }
        else if (isInHeadSlot) // ���� �� ����� ������, �� ������ ������ 3� �������� ������� � ������ �������
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Head) // ���� �������� �������� �� ������������ �������
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("������ �� ������ ������ ������� �� ���������!");
                return;
            }

            inventoryWindow.Head.SetActive(false);
            Swap3dObjectsAndItems(109, null, moveToIndex, toStorage);
        }
        else if (storage != null) // ���� ������ �� ���������
        {
            Swap3dObjectsAndItems(index, storage, moveToIndex, toStorage);

            // ������� �� ������ ��������� ��������� � ��������� � ������ ��������� ���������
            if (storage != toStorage)
            {
                inventoryWindow.drawnIcons.Add(gameObject);
                storageWindow.drawnIcons.Remove(gameObject);
                if (slot.childCount > 0) slot.GetChild(0).GetComponent<InventoryIcon>().storage = storage;
            } 
        }
        else if (isInInventory)
        {
            if (toStorage != null && toStorage.storageItems[moveToIndex].Type == ItemType.Bag ||
                toStorage == null && (moveToIndex == 100 && inventoryWindow.LeftHandItem.Type == ItemType.Bag ||
                moveToIndex == 101 && inventoryWindow.RightHandItem.Type == ItemType.Bag))
            {
                links.mousePoint.Comment("�������� ����� � ����� �� ���������!");
                transform.parent = originalParent;
                return;
            }

            Swap3dObjectsAndItems(index, null, moveToIndex, toStorage);

            // ������� �� ������ ��������� ��������� � ��������� � ������ ��������� ���������
            if (storage != toStorage)
            {
                inventoryWindow.drawnIcons.Remove(gameObject);
                storageWindow.drawnIcons.Add(gameObject);
                if (slot.childCount > 0) slot.GetChild(0).GetComponent<InventoryIcon>().storage = null;
                storage = toStorage;
            }
        }

        // ���� � ����� ��� ���� ������, �� ���������� �� 3�-������ � ����� ��� ������ ����
        if (moveToIndex == 100 && inventoryWindow.LeftHandObject != null)
        {
            inventoryWindow.LeftHandObject.SetActive(true);
            inventoryWindow.LeftHandObject.GetComponent<Rigidbody>().isKinematic = true;
            inventoryWindow.LeftHandObject.transform.parent = inventoryWindow.leftHandParent;
        }
        else if (moveToIndex == 101 && inventoryWindow.RightHandObject != null)
        {
            inventoryWindow.RightHandObject.SetActive(true);
            inventoryWindow.RightHandObject.GetComponent<Rigidbody>().isKinematic = true;
            inventoryWindow.RightHandObject.transform.parent = inventoryWindow.rightHandParent;
        }

        // �������� ������ � ���� ��������� � ������ ��� ��������
        transform.parent = slot;
        originalParent = slot;
        storageWindow.Redraw();
        inventoryWindow.Redraw();  
    }

    public void Drop()
    {
        if (item3dObject == null)
        {
            Debug.Log("Spawn");
            item3dObject = Instantiate(Prefab, inventoryWindow.dropParent);   
        }
        else
        {
            item3dObject.transform.parent = inventoryWindow.dropParent;
            item3dObject.GetComponent<Rigidbody>().isKinematic = false;
            item3dObject.GetComponent<BoxCollider>().enabled = true;
            item3dObject.layer = 0;
            item3dObject.SetActive(true);
        }
        item3dObject.transform.position = links.player.transform.position + new Vector3(0, 1, 0);
        RemoveFromInventory();
        DropItem?.Invoke();
        Destroy(gameObject, 0);
        links.mousePoint.isPointUI = false;

    }

    public void RemoveFromInventory()
    {
        if (item.Type == ItemType.Wood && storage == null) links.player.Wood--;

        if (isInRightHand) 
        {
            inventoryWindow.RightHandObject = null;
            inventoryWindow.RightHandItem = null; 
        }
        else if (isInLeftHand) 
        {
            inventoryWindow.LeftHandObject = null;
            inventoryWindow.LeftHandItem = null; 
        }
        else if (isInBagSlot)
        {
            inventoryWindow.Backpack = null;
            inventoryWindow.BackpackItem = null;
        }
        else if (isInBeltSlot)
        {
            inventoryWindow.Belt = null;
            inventoryWindow.BeltItem = null;
        }
        else if (isInFeetSlot)
        {
            inventoryWindow.Feet = null;
            inventoryWindow.FeetItem = null;
        }
        else if (isInLegsSlot)
        {
            inventoryWindow.Legs = null;
            inventoryWindow.LegsItem = null;
        }
        else if (isInHandsSlot)
        {
            inventoryWindow.Arms = null;
            inventoryWindow.ArmsItem = null;
        }
        else if (isInBodySlot)
        {
            inventoryWindow.Body = null;
            inventoryWindow.BodyItem = null;
        }
        else if (isInShouldersSlot)
        {
            inventoryWindow.Shoulders = null;
            inventoryWindow.ShouldersItem = null;
        }
        else if (isInHeadSlot)
        {
            inventoryWindow.Head = null;
            inventoryWindow.HeadItem = null;
        }
        else
        {
            if (storage == null)
            {
                for (int i = 0; i < inventoryWindow.inventory.size; i++)
                {
                    if (inventoryWindow.inventory.inventoryItems[i] != null)
                    {
                        if (inventoryWindow.inventory.inventoryItems[i].Prefab.Equals(Prefab))
                        {
                            Debug.Log("Remove");
                            inventoryWindow.inventory.inventoryItems[i] = null;
                            inventoryWindow.inventory.inventoryItemObjects[i] = null;
                            inventoryWindow.inventory.Recount();
                            inventoryWindow.Redraw();
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
                            storage.storageItemObjects[i] = null;
                            storage.Recount();
                            inventoryWindow.Redraw();
                            return;
                        }
                    }
                }
            }
        }
        
        inventoryWindow.Redraw();
        storageWindow.Redraw();
    }

}
