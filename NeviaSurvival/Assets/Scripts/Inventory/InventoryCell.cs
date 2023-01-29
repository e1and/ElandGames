using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InventoryCell : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public event Action DropItem;

    public Item item;
    
    public Inventory inventory;
    public InventoryWindow inventoryWindow;
    public Storage storage;
    public StorageWindow storageWindow;
    public Text Name;
    public int index;
    public GameObject Prefab;
    public GameObject item3dObject;
    public bool isInRightHand;
    public bool isInLeftHand;
    public bool isEquiped;
    public bool isInInventory;
    public Transform dropParent;

    private Transform _draggingParent;
    public Transform _originalParent;

    public Transform _rightHandParent;
    public Transform _leftHandParent;

    // � ������ ������������ 3� ������� � ������� �������������� ������ ���������, �����������, ���������
    private void Start()
    {
        if (storage == null)
        {
            if (index == 100 && inventory.ItemInLeftHand != null) item3dObject = inventory.ItemInLeftHand;
            if (index == 101 && inventory.ItemInRightHand != null) item3dObject = inventory.ItemInRightHand;
            if (index < 9 && inventory.inventoryItemObjects[index] != null)
            {
                item3dObject = inventory.inventoryItemObjects[index];
            }
        }
        else if (storage.storageItemObjects[index] != null)
        {
            item3dObject = storage.storageItemObjects[index];
        }
    }

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

    // ��� ������ �������������� ������ ������ ������, � � �������� ������ ������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            _originalParent = transform.parent;
            transform.parent = _draggingParent;
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

    // ����� ������������, ��� ������ �� ������������ 2�-�������
    private bool InRectangle(RectTransform rectangle)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectangle, transform.position);
    }

    private void DragInSlot(RectTransform slot)
    {
        // ���������� ����� �����, ���� ����������
        int moveToIndex = 0;
        for (int i = 0; i < 9; i++)
        {
            if (inventoryWindow.slots[i] == slot) moveToIndex = i;
        }

        if (isInRightHand) // ���� �� ������ ����, �� ������ ������ 3�-�������� ������� � ������ �������
        {
            inventory.ItemInRightHand.SetActive(false); // 3�-������ ������������

            GameObject thisObject;
            thisObject = inventory.ItemInRightHand;
            inventory.ItemInRightHand = inventory.inventoryItemObjects[moveToIndex];
            inventory.inventoryItemObjects[moveToIndex] = thisObject;
            isEquiped = false;
        }
        else if (isInLeftHand) // ���� �� ����� ����, �� ������ ������ 3� �������� ������� � ������ �������
        {
            inventory.ItemInLeftHand.SetActive(false); // 3�-������ ������������

            GameObject thisObject;
            thisObject = inventory.ItemInLeftHand;
            inventory.ItemInLeftHand = inventory.inventoryItemObjects[moveToIndex];
            inventory.inventoryItemObjects[moveToIndex] = thisObject;
            isEquiped = false;
        }

        if (slot.childCount > 0)   // ���� ��������������� �� �� ������ ���� ���������
        {
            var prefab = slot.GetChild(0).GetComponent<InventoryCell>().Prefab;

            if (isInRightHand)
            {
                if (inventory.ItemInRightHand == null)
                {
                    SpawnItem(prefab, _rightHandParent);
                }

                slot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = true;
            }
            else if (isInLeftHand)
            {
                if (inventory.ItemInLeftHand == null)
                {
                    SpawnItem(prefab, _leftHandParent);
                }
                slot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = true;
            }
            else if (storage != null)
            {
                if (item3dObject != null || slot.GetChild(0).GetComponent<InventoryCell>().item3dObject != null)
                {
                    GameObject thisItem = storage.storageItemObjects[index];
                    storage.storageItemObjects[index] = inventory.inventoryItemObjects[moveToIndex];
                    inventory.inventoryItemObjects[moveToIndex] = thisItem;
                }

                slot.GetChild(0).GetComponent<InventoryCell>().storage = storage;
            }
        }
        else
        {
            if (isInInventory)
            {
                if (item3dObject != null)
                {
                    GameObject thisItem = inventory.inventoryItemObjects[index];
                    inventory.inventoryItemObjects[index] = inventory.inventoryItemObjects[moveToIndex];
                    inventory.inventoryItemObjects[moveToIndex] = thisItem;
                }
            }
            else if (storage != null)
            {
                if (item3dObject != null)
                {
                    GameObject thisItem = storage.storageItemObjects[index];
                    storage.storageItemObjects[index] = inventory.inventoryItemObjects[moveToIndex];
                    inventory.inventoryItemObjects[moveToIndex] = thisItem;
                }
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

        if (slot.childCount > 0) // ���� ���� ��������� �� ������
        {
            var prefab = slot.GetChild(0).GetComponent<InventoryCell>().Prefab;
            if (isInRightHand) // ������� �� ������ ����
            {
                if (storage.storageItemObjects[moveToIndex] == null) // ���� 3�-������� ���, �� ����� ��� � ������ ����
                {
                    storage.storageItemObjects[moveToIndex] = item3dObject = Instantiate(prefab, _rightHandParent);
                    storage.storageItemObjects[moveToIndex].GetComponent<BoxCollider>().enabled = false;
                    if (storage.storageItemObjects[moveToIndex].TryGetComponent(out MeshCollider mesh)) mesh.enabled = false;
                    storage.storageItemObjects[moveToIndex].GetComponent<Rigidbody>().isKinematic = true;
                }

                slot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = true;
            }
            else if (isInLeftHand) // ������� �� ����� ����
            {
                if (storage.storageItemObjects[moveToIndex] == null) // ���� 3�-������� ���, �� ����� ��� � ����� ����
                {
                    storage.storageItemObjects[moveToIndex] = Instantiate(prefab, _leftHandParent);
                    storage.storageItemObjects[moveToIndex].GetComponent<BoxCollider>().enabled = false;
                    if (storage.storageItemObjects[moveToIndex].TryGetComponent(out MeshCollider mesh)) mesh.enabled = false;
                    storage.storageItemObjects[moveToIndex].GetComponent<Rigidbody>().isKinematic = true;
                }

                slot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = true;
            }
            else if (storage != null) // ������� �� ���������
            {
                if (item3dObject != null || slot.GetChild(0).GetComponent<InventoryCell>().item3dObject != null)
                {
                    GameObject thisItem = storage.storageItemObjects[index];
                    storage.storageItemObjects[index] = storage.storageItemObjects[moveToIndex];
                    storage.storageItemObjects[moveToIndex] = thisItem;
                }
            }
            else if (isInInventory) // ������� �� ���������
            {
                if (item3dObject != null || slot.GetChild(0).GetComponent<InventoryCell>().item3dObject != null)
                {
                    GameObject thisItem = inventory.inventoryItemObjects[index];
                    inventory.inventoryItemObjects[index] = storage.storageItemObjects[moveToIndex];
                    storage.storageItemObjects[moveToIndex] = thisItem;
                }
            }
        }
        else 
        {
            if (isInInventory)
            {
                if (item3dObject != null)
                {
                    GameObject thisItem = inventory.inventoryItemObjects[index];
                    inventory.inventoryItemObjects[index] = storage.storageItemObjects[moveToIndex];
                    storage.storageItemObjects[moveToIndex] = thisItem;
                }
            }
        }

        if (isInRightHand) // 3�-������ �� ������ ����
        {
            inventory.ItemInRightHand.SetActive(false);

            GameObject thisObject;
            thisObject = inventory.ItemInRightHand;
            inventory.ItemInRightHand = storage.storageItemObjects[moveToIndex];
            storage.storageItemObjects[moveToIndex] = thisObject;

            isEquiped = false;
        }
        else if (isInLeftHand) // 3�-������ �� ����� ����
        {
            inventory.ItemInLeftHand.SetActive(false);

            GameObject thisObject;
            thisObject = inventory.ItemInLeftHand;
            inventory.ItemInLeftHand = storage.storageItemObjects[moveToIndex];
            storage.storageItemObjects[moveToIndex] = thisObject;

            isEquiped = false;
        }

        InventoryCell secondCell = null;
        if (slot.childCount > 0) secondCell = slot.GetChild(0).GetComponent<InventoryCell>();
        SwapSlots(this, secondCell, slot);
    }

    void SwapSlots(InventoryCell firstCell, InventoryCell secondCell, RectTransform targetSlot)
    {
        Debug.Log("Swap slots" + firstCell + " and " + secondCell);
        if (targetSlot.childCount > 0)
        {
            if (firstCell != secondCell | 
                isInInventory != secondCell.isInInventory)        // ���� ���� �� ������,
            {
                targetSlot.GetChild(0).parent = _originalParent;
            }                    // �� ����������� ������ �������� � ���� ������������� ��������
        }
        if (targetSlot.GetComponent<InventorySlot>().isStorage == false) storageWindow.drawnIcons.Remove(gameObject);
        transform.parent = targetSlot;

        Item firstItem;
        Item secondItem = null;
        int targetIndex = targetSlot.GetComponent<InventorySlot>().indexSlot;
        int itemIndex = index;
        bool isFromStorage = !firstCell.isInInventory;
        bool isToStorage = targetSlot.GetComponent<InventorySlot>().isStorage;



        if (firstCell.isInLeftHand) { firstItem = inventory.LeftHand; }
        else if (firstCell.isInRightHand) { firstItem = inventory.RightHand; }
        else if (firstCell.isInInventory && !firstCell.isInLeftHand && !firstCell.isInRightHand) 
        { firstItem = inventory.inventoryItems[firstCell.index]; Debug.Log("First item is " + firstItem); }
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
                
                firstCell.storage = null; //////////////////////////////////////////

                inventoryWindow.drawnIcons.Add(firstCell.gameObject);
            }
            else if (!isToStorage && !isFromStorage && firstItem != null)
            {
                inventory.inventoryItems[itemIndex] = secondItem;
                inventory.inventoryItems[targetIndex] = firstItem;
                firstCell.isInInventory = true;
            }
        }
        else if (itemIndex == 100)
        {
            if (isToStorage)
            {
                storage.storageItems[targetIndex] = inventory.LeftHand;
                storageWindow.drawnIcons.Add(firstCell.gameObject);
            }
            else
            {
                inventory.inventoryItems[targetIndex] = inventory.LeftHand;
                inventoryWindow.drawnIcons.Add(firstCell.gameObject);
            }
            inventory.LeftHand = null;
            firstCell.isInLeftHand = false;
        }
        else if (itemIndex == 101)
        {
            if (isToStorage)
            {
                storage.storageItems[targetIndex] = inventory.RightHand;
                storageWindow.drawnIcons.Add(firstCell.gameObject);
            }
            else
            {
                inventory.inventoryItems[targetIndex] = inventory.RightHand;
                inventoryWindow.drawnIcons.Add(firstCell.gameObject);
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
                    Debug.Log($"Icon {secondCell} from storage to Left Hand");
                    storage.storageItems[secondCell.index] = firstItem;
                    storageWindow.drawnIcons.Add(firstCell.gameObject);
                    storageWindow.drawnIcons.Remove(secondCell.gameObject);
                }
                else
                {
                    inventory.inventoryItems[secondCell.index] = firstItem;
                    inventoryWindow.drawnIcons.Add(firstCell.gameObject);
                    inventoryWindow.drawnIcons.Remove(secondCell.gameObject);
                }
                inventory.LeftHand = secondItem;
                secondCell.isInLeftHand = true;
            }
            else if (itemIndex == 101)
            {
                if (isFromStorage)
                {
                    storage.storageItems[secondCell.index] = firstItem;
                    storageWindow.drawnIcons.Add(firstCell.gameObject);
                    storageWindow.drawnIcons.Remove(secondCell.gameObject);
                }
                else
                {
                    inventory.inventoryItems[secondCell.index] = firstItem;
                    inventoryWindow.drawnIcons.Add(firstCell.gameObject);
                    inventoryWindow.drawnIcons.Remove(secondCell.gameObject);
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
        else if (isInInventory) isInInventory = false;

        storageWindow.Redraw();
        inventoryWindow.Redraw();
    }

    private void DragInRightHand()
    {
        // ���� � ������ ���� ��� ���-�� ����, �� ������������ ��� ������ �� ������ ���� �� ����� ���������������
        if (inventory.RightHand != null && isInLeftHand) // � ����� ����
        {
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = false;
            inventoryWindow.rightHandSlot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = true;
            inventoryWindow.rightHandSlot.GetChild(0).SetParent(_originalParent);

            SwapHands();
        }
        else if (inventory.RightHand != null && isInRightHand) // ������� ������ ������� � ��� �� ����
        {
            transform.SetParent(_originalParent);
            return;
        }
        else if (inventory.RightHand != null) // � ���������, ���� � ���������
        {
            // ����������� ������, �.�. ���������� �����
            if (isInInventory || storage != null)
                Destroy(inventoryWindow.leftHandSlot.GetChild(0).gameObject);
        }

        if (inventory.ItemInRightHand == null || inventory.ItemInRightHand != null && !isInLeftHand)
        {
            if (item3dObject == null) // ����� �������� � ��� ������������� ����������� ���������� � �����
            {
                if (inventory.ItemInRightHand != null)
                {
                    inventory.inventoryItemObjects[index] = inventory.ItemInRightHand;
                    inventory.ItemInRightHand.SetActive(false);
                }
                SpawnItem(Prefab, _rightHandParent);
            }
            else // ���� ����� ��������� �������, ��������� � ����, �������� ���������, ��������� ���������
            {
                item3dObject.GetComponent<Rigidbody>().isKinematic = true;
                item3dObject.transform.parent = _rightHandParent;
                item3dObject.transform.localPosition = new Vector3(0, 0, 0);
                item3dObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                if (isInInventory) // � ������ ������� �� � ������ ��������� � ����
                {
                    GameObject temp = inventory.ItemInRightHand;
                    inventory.ItemInRightHand = inventory.inventoryItemObjects[index];
                    inventory.inventoryItemObjects[index] = temp;
                }
                if (storage != null) // ���� ������ � ������ ��������� � ����
                {
                    GameObject temp = inventory.ItemInRightHand;
                    inventory.ItemInRightHand = storage.storageItemObjects[index];
                    storage.storageItemObjects[index] = temp;
                }
                inventory.ItemInRightHand.SetActive(true);
                item3dObject.GetComponent<BoxCollider>().enabled = false;
            }
        }

        // ������������� ������� ��������� Item �� ������ ���� � �����
        if (isInLeftHand) 
        {
            isInLeftHand = false;
            Item temp;
            temp = inventory.RightHand;
            inventory.RightHand = inventory.LeftHand;
            inventory.LeftHand = temp;
        }
        else // ���� ������������� ������� ��������� Item �� ��������� ��� ���������
        {
            Item temp;
            temp = inventory.RightHand;
            if (_originalParent.GetComponent<InventorySlot>().isStorage)
            {
                inventory.RightHand = storage.storageItems[index];
                storage.storageItems[index] = temp;
            }
            else
            {
                inventory.RightHand = inventory.inventoryItems[index];
                inventory.inventoryItems[index] = temp;
            }
        }

        // ���� ������ �� ���������, �� ������� � �� ������ ������������ ������
        if (_originalParent.GetComponent<InventorySlot>().isStorage) storageWindow.drawnIcons.Remove(gameObject);

        // �������� ������ � ���� ����� ���� � ������ ��� ��������
        transform.parent = inventoryWindow.rightHandSlot;
        _originalParent = transform.parent;

        // ����������� ��������� ������������ ������ ������� � ����� ����� ����
        storage = null;
        isInRightHand = true;
        isEquiped = true;
        isInInventory = false;
        index = 101;

        // �������������� ��� ������ ��������� � ����������
        inventoryWindow.Redraw();
        storageWindow.Redraw();
    }

    private void DragInLeftHand()
    {        
        // ���� � ����� ���� ��� ���-�� ����, �� ������������ ��� ������ �� ����� ���� �� ����� ���������������
        if (inventory.LeftHand != null && isInRightHand) // � ������ ����
        {
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().index = index;
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().isInRightHand = true;
            inventoryWindow.leftHandSlot.GetChild(0).GetComponent<InventoryCell>().isInLeftHand = false;
            inventoryWindow.leftHandSlot.GetChild(0).SetParent(_originalParent);
           
            SwapHands();
        }
        else if (inventory.LeftHand != null && isInLeftHand) // ������� ������ ������� � ��� �� ����
        {
            transform.SetParent(_originalParent);
            return;
        }
        else if (inventory.LeftHand != null) // � ���������, ���� � ���������
        {
            // ����������� ������, �.�. ���������� �����
            if (isInInventory || storage != null)
            Destroy(inventoryWindow.leftHandSlot.GetChild(0).gameObject);
        }

        // ����������� 3�-��������, ���� �������� �������
        if (inventory.ItemInLeftHand == null || inventory.ItemInLeftHand != null && !isInRightHand)
        {
            if (item3dObject == null) // ����� �������� � ��� ������������� ����������� ���������� � �����
            {
                if (inventory.ItemInLeftHand != null)
                {
                    inventory.inventoryItemObjects[index] = inventory.ItemInLeftHand;
                    inventory.ItemInLeftHand.SetActive(false);
                }
                SpawnItem(Prefab, _leftHandParent);              
            }
            else // ���� ����� ��������� �������, ��������� � ����, �������� ���������, ��������� ���������
            {
                item3dObject.GetComponent<Rigidbody>().isKinematic = true;
                item3dObject.transform.parent = _leftHandParent;
                item3dObject.transform.localPosition = new Vector3(0, 0, 0);
                item3dObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

                if (isInInventory) // � ������ ������� �� � ������ ��������� � ����
                {
                    Debug.Log("Swap gameobjects");
                    GameObject temp = inventory.ItemInLeftHand;
                    inventory.ItemInLeftHand = inventory.inventoryItemObjects[index];
                    inventory.inventoryItemObjects[index] = temp;
                }
                if (storage != null) // ���� ������ � ������ ��������� � ����
                {
                    GameObject temp = inventory.ItemInLeftHand;
                    inventory.ItemInLeftHand = storage.storageItemObjects[index];
                    storage.storageItemObjects[index] = temp;
                }

                inventory.ItemInLeftHand.SetActive(true);
                item3dObject.GetComponent<BoxCollider>().enabled = false;
            }

        }

        // ������������� ������� ��������� Item �� ������ ���� � �����
        if (isInRightHand)
        {
            isInRightHand = false;
            Item temp;
            temp = inventory.LeftHand;
            inventory.LeftHand = inventory.RightHand;
            inventory.RightHand = temp;
        }
        else // ���� ������������� ������� ��������� Item �� ��������� ��� ���������
        {
            Item temp;
            temp = inventory.LeftHand;
            if (_originalParent.GetComponent<InventorySlot>().isStorage)
            {
                inventory.LeftHand = storage.storageItems[index];
                storage.storageItems[index] = temp;

            }
            else
            {
                inventory.LeftHand = inventory.inventoryItems[index];
                inventory.inventoryItems[index] = temp;
            }
        }

        // ���� ������ �� ���������, �� ������� � �� ������ ������������ ������
        if (_originalParent.GetComponent<InventorySlot>().isStorage) storageWindow.drawnIcons.Remove(gameObject);

        // �������� ������ � ���� ����� ���� � ������ ��� ��������
        transform.parent = inventoryWindow.leftHandSlot;
        _originalParent = transform.parent;
        
        // ����������� ��������� ������������ ������ ������� � ����� ����� ����
        storage = null;
        isInLeftHand = true;
        isEquiped = true;
        isInInventory = false;
        index = 100;

        // �������������� ��� ������ ��������� � ����������
        inventoryWindow.Redraw();
        storageWindow.Redraw();
    }

    void SpawnItem(GameObject prefab, Transform spawnPlace)
    {
        if (item3dObject == null)
        {
            item3dObject = Instantiate(prefab, spawnPlace);
        }
        else
        {
            item3dObject.SetActive(true);
            item3dObject.transform.parent = spawnPlace;
        }

        item3dObject.GetComponent<BoxCollider>().enabled = false;
        if (item3dObject.TryGetComponent(out MeshCollider mesh)) mesh.enabled = false;
        item3dObject.GetComponent<Rigidbody>().isKinematic = true;
        

        if (spawnPlace == _leftHandParent) inventory.ItemInLeftHand = item3dObject;
        else inventory.ItemInRightHand = item3dObject;
    }

    void SwapHands()
    {
        Debug.Log("Swap Hands");
        var tempItem = inventory.ItemInLeftHand;
        inventory.ItemInLeftHand = inventory.ItemInRightHand;
        inventory.ItemInRightHand = tempItem;

        if (inventory.ItemInLeftHand != null && inventory.ItemInRightHand != null)
        {
            inventory.ItemInLeftHand.transform.position = inventoryWindow._leftHandParent.position;
            inventory.ItemInRightHand.transform.position = inventoryWindow._rightHandParent.position;

            inventory.ItemInLeftHand.transform.parent = _leftHandParent;
            inventory.ItemInRightHand.transform.parent = _rightHandParent;
        }
    }

    private void Drop()
    {
        if (item3dObject == null)
        {
            Debug.Log("Spawn");
            item3dObject = Instantiate(Prefab, dropParent);   
        }
        else
        {
            item3dObject.transform.parent = dropParent;
            item3dObject.GetComponent<Rigidbody>().isKinematic = false;
            item3dObject.GetComponent<BoxCollider>().enabled = true; ;
            item3dObject.SetActive(true);
        }
        item3dObject.transform.position = inventory.gameObject.transform.position + new Vector3(0, 1, 0);
        RemoveFromInventory();
        DropItem?.Invoke();
        Destroy(gameObject, 0);
        inventory.gameObject.GetComponent<MousePoint>().isPointUI = false;

    }

    void RemoveFromInventory()
    {
        if (isInRightHand) 
        { 
            inventory.ItemInRightHand = null; 
            inventory.RightHand = null; 
            isInRightHand = false;
            isEquiped = false;
        }
        else if (isInLeftHand) 
        { 
            inventory.ItemInLeftHand = null; 
            inventory.LeftHand = null; 
            isInLeftHand = false;
            isEquiped = false;
        }
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
                            inventory.inventoryItemObjects[i] = null;
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
                            storage.storageItemObjects[i] = null;
                            storage.Recount();
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
