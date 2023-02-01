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

    // В старте привязывем 3д объекты к иконкам соответсвующей ячейки инвентаря, экмипировки, хранилища
    private void Start()
    {
        if (storage == null)
        {
            if (index == 100 && inventory.ItemInLeftHand != null)
            {
                item3dObject = inventory.ItemInLeftHand;
                item3dObject.SetActive(true);
            }
            else if (index == 101 && inventory.ItemInRightHand != null)
            {
                item3dObject = inventory.ItemInRightHand;
                item3dObject.SetActive(true);
            }
            else if (index < 9 && inventory.inventoryItemObjects[index] != null)
            {
                item3dObject = inventory.inventoryItemObjects[index];
                item3dObject.SetActive(false);
            }
            else
            {
                item3dObject = Instantiate(Prefab, dropParent);
                if (index < 100) inventory.inventoryItemObjects[index] = item3dObject;
                else if (index == 100) inventory.ItemInLeftHand = item3dObject;
                else if (index == 101) inventory.ItemInRightHand = item3dObject;
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
                item3dObject = Instantiate(Prefab, dropParent);
                if (index < 100) storage.storageItemObjects[index] = item3dObject;
                item3dObject.SetActive(false);
            }
        }

        if (index < 100) item3dObject.transform.parent = dropParent;
        else if (index == 100)
        {
            item3dObject.transform.parent = _leftHandParent;
            if (item3dObject.TryGetComponent(out HandPositions hand))
            {
                item3dObject.transform.localPosition = hand.LeftHandPosition;
                item3dObject.transform.localRotation = hand.LeftHandRotation;
            }
        }
        else if (index == 101)
        {
            item3dObject.transform.parent = _rightHandParent;
            if (item3dObject.TryGetComponent(out HandPositions hand))
            {
                item3dObject.transform.localPosition = hand.RightHandPosition;
                item3dObject.transform.localRotation = hand.RightHandRotation;
            }
        }


        item3dObject.GetComponent<Rigidbody>().isKinematic = true;
        item3dObject.GetComponent<BoxCollider>().enabled = false;


    }

    public void Init(Transform draggingParent)
    {
        _draggingParent = draggingParent;
        _originalParent = transform.parent;
    }

    // При начале перетаскивания ИКОНКА меняет парент, а её исходный парент запоминается
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            _originalParent = transform.parent;
            transform.parent = _draggingParent;
        }
    }

    // При зажатой кнопке мыши ИКОНКА ПЕРЕТАСКИВАЕТСЯ вместе с курсором
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = Input.mousePosition;
        }
    }

    // При отпускании кнопки над слотами ИКОНКА ПЕРЕХОДИТ В СЛОТ или дропается предмет
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
            { DragInSlot(storageWindow.slots[i]); return; }
        }
        if (InRectangle(inventoryWindow.rightHandSlot)) DragInSlot(inventoryWindow.rightHandSlot);
        else if (InRectangle(inventoryWindow.leftHandSlot)) DragInSlot(inventoryWindow.leftHandSlot);
        else Drop();
    }

    // Метод определяющий, что курсор на определенном 2д-объекте
    private bool InRectangle(RectTransform rectangle)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectangle, transform.position);
    }

    // Метод, меняющий местами 3д объекты и Item в их слотах
    void Swap3dObjectsAndItems(int firstIndex, Storage firstStorage, int secondIndex, Storage secondStorage)
    {
        GameObject firstObject = null;
        GameObject secondObject = null;
        Item firstItem = null;
        Item secondItem = null;
        
        // Определяем по индексу и хранилищу первый Item и его 3д-объект
        if (firstIndex < 100)
        {
            if (firstStorage == null)
            {
                firstObject = inventory.inventoryItemObjects[firstIndex];
                firstItem = inventory.inventoryItems[firstIndex];
            }
            else
            {
                firstObject = firstStorage.storageItemObjects[firstIndex];
                firstItem = firstStorage.storageItems[firstIndex];
            }
        }
        else
        {
            if (firstIndex == 100) { firstObject = inventory.ItemInLeftHand; firstItem = inventory.LeftHand; }
            if (firstIndex == 101) { firstObject = inventory.ItemInRightHand; firstItem = inventory.RightHand; }
        }
        // Определяем по индексу и хранилищу второй Item и его 3д-объект
        if (secondIndex < 100)
        {
            if (secondStorage == null)
            {
                secondObject = inventory.inventoryItemObjects[secondIndex];
                secondItem = inventory.inventoryItems[secondIndex];
            }
            else
            {
                secondObject = secondStorage.storageItemObjects[secondIndex];
                secondItem = secondStorage.storageItems[secondIndex];
            }
        }
        else
        {
            if (secondIndex == 100) { secondObject = inventory.ItemInLeftHand; secondItem = inventory.LeftHand; }
            if (secondIndex == 101) { secondObject = inventory.ItemInRightHand; secondItem = inventory.RightHand; }
        }

        // Замена в слот инвентаря
        if (firstIndex < 100 && firstStorage == null)
        {
            inventory.inventoryItemObjects[firstIndex] = secondObject;
            inventory.inventoryItems[firstIndex] = secondItem;
        }
        if (secondIndex < 100 && secondStorage == null)
        {
            inventory.inventoryItemObjects[secondIndex] = firstObject;
            inventory.inventoryItems[secondIndex] = firstItem;
        }

        // Замена в слот хранилища
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

        // Замена в слот левой руки
        if (firstIndex == 100 && firstStorage == null) { inventory.ItemInLeftHand = secondObject; inventory.LeftHand = secondItem; }
        if (secondIndex == 100 && secondStorage == null) { inventory.ItemInLeftHand = firstObject; inventory.LeftHand = firstItem; }

        // Замена в слот правой руки
        if (firstIndex == 101 && firstStorage == null) { inventory.ItemInRightHand = secondObject; inventory.RightHand = secondItem; }
        if (secondIndex == 101 && secondStorage == null) { inventory.ItemInRightHand = firstObject; inventory.RightHand = firstItem; }
    }

    private void DragInSlot(RectTransform slot)
    {
        // Определяем номер слота, куда перетащили и принадлежит ли слот текущему хранилищу
        int moveToIndex = slot.GetComponent<InventorySlot>().indexSlot;
        Storage toStorage = null;
        if (slot.GetComponent<InventorySlot>().isStorage) toStorage = storageWindow.targetStorage;

        if (isInRightHand) // Если из правой руки, то меняем ячейки 3д-объектов и Item местами в нужных списках
        {
            inventory.ItemInRightHand.SetActive(false);

            Swap3dObjectsAndItems(101, null, moveToIndex, toStorage);
        }
        else if (isInLeftHand) // Если из левой руки, то меняем ячейки 3д Объектов местами в нужных списках
        {
            inventory.ItemInLeftHand.SetActive(false);

            Swap3dObjectsAndItems(100, null, moveToIndex, toStorage);
        }
        else if (storage != null) // Если иконка из хранилища
        {
            Swap3dObjectsAndItems(index, storage, moveToIndex, toStorage);

            // Убираем из списка отрисовки хранилища и добавляем в список отрисовки инвентяря
            if (storage != toStorage)
            {
                inventoryWindow.drawnIcons.Add(gameObject);
                storageWindow.drawnIcons.Remove(gameObject);
                if (slot.childCount > 0) slot.GetChild(0).GetComponent<InventoryCell>().storage = storage;
            } 
        }
        else if (isInInventory)
        {
            Swap3dObjectsAndItems(index, null, moveToIndex, toStorage);

            // Убираем из списка отрисовки хранилища и добавляем в список отрисовки инвентяря
            if (storage != toStorage)
            {
                inventoryWindow.drawnIcons.Remove(gameObject);
                storageWindow.drawnIcons.Add(gameObject);
                if (slot.childCount > 0) slot.GetChild(0).GetComponent<InventoryCell>().storage = null;
                storage = toStorage;
            }
        }

        // Если в слоте уже есть иконка, то перемещаем ее 3д-объект в левую или правую руку
        if (moveToIndex == 100 && inventory.ItemInLeftHand != null)
        {
            inventory.ItemInLeftHand.SetActive(true);
            inventory.ItemInLeftHand.GetComponent<Rigidbody>().isKinematic = true;
            inventory.ItemInLeftHand.transform.parent = _leftHandParent;
            //inventory.ItemInLeftHand.transform.localPosition = new Vector3(0, 0, 0);
            //inventory.ItemInLeftHand.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if (moveToIndex == 101 && inventory.ItemInRightHand != null)
        {
            inventory.ItemInRightHand.SetActive(true);
            inventory.ItemInRightHand.GetComponent<Rigidbody>().isKinematic = true;
            inventory.ItemInRightHand.transform.parent = _rightHandParent;
            //inventory.ItemInRightHand.transform.localPosition = new Vector3(0, 0, 0);
            //inventory.ItemInRightHand.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        // Помещаем иконку в слот инвентаря и делаем его исходным
        transform.parent = slot;
        _originalParent = slot;
        storageWindow.Redraw();
        inventoryWindow.Redraw();  
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
