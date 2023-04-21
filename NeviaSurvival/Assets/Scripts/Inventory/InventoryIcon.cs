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
    [Header("Принадлежность предмета какому-то слоту")]
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
    [Header("Состояния предмета")]
    public bool isEquiped;
    public bool isInInventory;

    private Transform draggingParent;
    public Transform originalParent;

    public Links links;

    //public Transform rightHandParent;
    //public Transform leftHandParent;
    //public Transform backpackParent;

    // В старте привязывем 3д объекты к иконкам соответсвующей ячейки инвентаря, экмипировки, хранилища
    // 100 - Left Hand, 101 - Right Hand, 102 - Backpack, 103 - Belt, 104 - Foots, 105 - Legs, 106 - Hands, 107 - Body, 
    // 108 - Shoulders, 109 - Head
    private void Start()
    {
        inventoryWindow = links.inventoryWindow;
        storageWindow = links.storageWindow;
        
        Name = GetComponentInChildren<Text>(); // Текстовое поле иконки находится внутри иконки

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

    // При начале перетаскивания ИКОНКА меняет парент, а её исходный парент запоминается
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            originalParent = transform.parent;
            transform.parent = draggingParent;
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
        else if (InRectangle(links.inventoryWindow.backpackSlot)) // Перемещение иконки в слот рюкзака
        {
            if (item.Type == ItemType.Bag) DragInSlot(links.inventoryWindow.backpackSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу повесить это на спину! Этот слот для рюкзака!");
            }
        }
        else if (InRectangle(links.inventoryWindow.beltSlot)) // Перемещение иконки в слот пояса
        {
            if (item.Type == ItemType.Belt) DragInSlot(links.inventoryWindow.beltSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу повесить это на пояс! Этот слот для ремня!");
            }
        }
        else if (InRectangle(links.inventoryWindow.feetSlot)) // Перемещение иконки в слот обуви
        {
            if (item.Type == ItemType.Feet) DragInSlot(links.inventoryWindow.feetSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на ступни! Этот слот для обуви!");
            }
        }
        else if (InRectangle(links.inventoryWindow.legsSlot)) // Перемещение иконки в слот ног
        {
            if (item.Type == ItemType.Legs) DragInSlot(links.inventoryWindow.legsSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на ноги! Этот слот для штанов!");
            }
        }
        else if (InRectangle(links.inventoryWindow.armsSlot)) // Перемещение иконки в слот рук
        {
            if (item.Type == ItemType.Hands) DragInSlot(links.inventoryWindow.armsSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на руки! Этот слот для наручей!");
            }
        }
        else if (InRectangle(links.inventoryWindow.bodySlot)) // Перемещение иконки в слот тела
        {
            if (item.Type == ItemType.Body) DragInSlot(links.inventoryWindow.bodySlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на тело! Этот слот для рубахи или доспеха!");
            }
        }
        else if (InRectangle(links.inventoryWindow.shouldersSlot)) // Перемещение иконки в слот плечей
        {
            if (item.Type == ItemType.Shoulders) DragInSlot(links.inventoryWindow.shouldersSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на плечи! Этот слот для плаща или наплечников!");
            }
        }
        else if (InRectangle(links.inventoryWindow.headSlot)) // Перемещение иконки в слот головы
        {
            if (item.Type == ItemType.Head) DragInSlot(links.inventoryWindow.headSlot);
            else
            {
                gameObject.transform.SetParent(originalParent);
                links.mousePoint.Comment("Не могу надеть это на голову! Этот слот для шапки!");
            }
        }
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
        // Определяем по индексу и хранилищу второй Item и его 3д-объект
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

        // Замена в слот инвентаря
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
        if (firstIndex == 100 && firstStorage == null) { inventoryWindow.LeftHandObject = secondObject; inventoryWindow.LeftHandItem = secondItem; }
        if (secondIndex == 100 && secondStorage == null) { inventoryWindow.LeftHandObject = firstObject; inventoryWindow.LeftHandItem = firstItem; }

        // Замена в слот правой руки
        if (firstIndex == 101 && firstStorage == null) { inventoryWindow.RightHandObject = secondObject; inventoryWindow.RightHandItem = secondItem; }
        if (secondIndex == 101 && secondStorage == null) { inventoryWindow.RightHandObject = firstObject; inventoryWindow.RightHandItem = firstItem; }

        // Замена в слот рюкзака
        if (firstIndex == 102 && firstStorage == null) { inventoryWindow.Backpack = secondObject; inventoryWindow.BackpackItem = secondItem; }
        if (secondIndex == 102 && secondStorage == null) { inventoryWindow.Backpack = firstObject; inventoryWindow.BackpackItem = firstItem; }

        // Замена в слот пояса
        if (firstIndex == 103 && firstStorage == null) { inventoryWindow.Belt = secondObject; inventoryWindow.BeltItem = secondItem; }
        if (secondIndex == 103 && secondStorage == null) { inventoryWindow.Belt = firstObject; inventoryWindow.BeltItem = firstItem; }

        // Замена в слот ступней
        if (firstIndex == 104 && firstStorage == null) { inventoryWindow.Feet = secondObject; inventoryWindow.FeetItem = secondItem; }
        if (secondIndex == 104 && secondStorage == null) { inventoryWindow.Feet = firstObject; inventoryWindow.FeetItem = firstItem; }

        // Замена в слот ног
        if (firstIndex == 105 && firstStorage == null) { inventoryWindow.Legs = secondObject; inventoryWindow.LegsItem = secondItem; }
        if (secondIndex == 105 && secondStorage == null) { inventoryWindow.Legs = firstObject; inventoryWindow.LegsItem = firstItem; }

        // Замена в слот рук
        if (firstIndex == 106 && firstStorage == null) { inventoryWindow.Arms = secondObject; inventoryWindow.ArmsItem = secondItem; }
        if (secondIndex == 106 && secondStorage == null) { inventoryWindow.Arms = firstObject; inventoryWindow.ArmsItem = firstItem; }

        // Замена в слот тела
        if (firstIndex == 107 && firstStorage == null) { inventoryWindow.Body = secondObject; inventoryWindow.BodyItem = secondItem; }
        if (secondIndex == 107 && secondStorage == null) { inventoryWindow.Body = firstObject; inventoryWindow.BodyItem = firstItem; }

        // Замена в слот плечей
        if (firstIndex == 108 && firstStorage == null) { inventoryWindow.Shoulders = secondObject; inventoryWindow.ShouldersItem = secondItem; }
        if (secondIndex == 108 && secondStorage == null) { inventoryWindow.Shoulders = firstObject; inventoryWindow.ShouldersItem = firstItem; }

        // Замена в слот головы
        if (firstIndex == 109 && firstStorage == null) { inventoryWindow.Head = secondObject; inventoryWindow.HeadItem = secondItem; }
        if (secondIndex == 109 && secondStorage == null) { inventoryWindow.Head = firstObject; inventoryWindow.HeadItem = firstItem; }

        inventoryWindow.Redraw();
    }

    private void DragInSlot(RectTransform slot)
    {
        // Определяем номер слота, куда перетащили и принадлежит ли слот текущему хранилищу
        int moveToIndex = slot.GetComponent<InventorySlot>().indexSlot;
        Storage toStorage = null;
        if (slot.GetComponent<InventorySlot>().isStorage) toStorage = storageWindow.targetStorage;

        Item itemInSlot = null;
        if (slot.childCount > 0) itemInSlot = slot.GetChild(0).GetComponent<InventoryIcon>().item;

        if (item.Type == ItemType.Bag && toStorage == null && moveToIndex < 16)
        {
            transform.parent = originalParent;
            links.mousePoint.Comment("Положить сумку в сумку нельзя!");
            storageWindow.Redraw();
            inventoryWindow.Redraw();
            return;
        }
        
        if (isInRightHand) // Если из правой руки, то меняем ячейки 3д-объектов и Item местами в нужных списках
        {
            inventoryWindow.RightHandObject.SetActive(false);

            Swap3dObjectsAndItems(101, null, moveToIndex, toStorage);
        }
        else if (isInLeftHand) // Если из левой руки, то меняем ячейки 3д Объектов местами в нужных списках
        {
            inventoryWindow.LeftHandObject.SetActive(false);

            Swap3dObjectsAndItems(100, null, moveToIndex, toStorage);
        }
        else if (isInBagSlot) // Если из слота рюкзака, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Bag) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть вместо сумки другой предмет не получится!");
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
                links.mousePoint.Comment("Засунуть сумку саму в себя? Как? И главное - зачем?!");
            }
        }
        else if (isInBeltSlot) // Если из слота пояса, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Belt) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть вместо пояса другой предмет не получится!");
                return;
            }

            inventoryWindow.Belt.SetActive(false);
            Swap3dObjectsAndItems(103, null, moveToIndex, toStorage);
        }
        else if (isInFeetSlot) // Если из слота ступней, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Feet) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть вместо обуви другой предмет не получится!");
                return;
            }

            inventoryWindow.Feet.SetActive(false);
            Swap3dObjectsAndItems(104, null, moveToIndex, toStorage);
        }
        else if (isInLegsSlot) // Если из слота ног, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Legs) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть вместо штанов другой предмет не получится!");
                return;
            }

            inventoryWindow.Legs.SetActive(false);
            Swap3dObjectsAndItems(105, null, moveToIndex, toStorage);
        }
        else if (isInHandsSlot) // Если из слота рук, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Hands) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть вместо наручей другой предмет не получится!");
                return;
            }

            inventoryWindow.Arms.SetActive(false);
            Swap3dObjectsAndItems(106, null, moveToIndex, toStorage);
        }
        else if (isInBodySlot) // Если из слота тела, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Body) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть на тело другой предмет не получится!");
                return;
            }

            inventoryWindow.Body.SetActive(false);
            Swap3dObjectsAndItems(107, null, moveToIndex, toStorage);
        }
        else if (isInShouldersSlot) // Если из слота плечей, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Shoulders) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть на плечи другой предмет не получится!");
                return;
            }

            inventoryWindow.Shoulders.SetActive(false);
            Swap3dObjectsAndItems(108, null, moveToIndex, toStorage);
        }
        else if (isInHeadSlot) // Если из слота головы, то меняем ячейки 3д Объектов местами в нужных списках
        {
            if (itemInSlot != null && itemInSlot.Type != ItemType.Head) // Если пытаемся заменить на неподходящий предмет
            {
                transform.parent = originalParent;
                links.mousePoint.Comment("Надеть на голову другой предмет не получится!");
                return;
            }

            inventoryWindow.Head.SetActive(false);
            Swap3dObjectsAndItems(109, null, moveToIndex, toStorage);
        }
        else if (storage != null) // Если иконка из хранилища
        {
            Swap3dObjectsAndItems(index, storage, moveToIndex, toStorage);

            // Убираем из списка отрисовки хранилища и добавляем в список отрисовки инвентяря
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
                links.mousePoint.Comment("Положить сумку в сумку не получится!");
                transform.parent = originalParent;
                return;
            }

            Swap3dObjectsAndItems(index, null, moveToIndex, toStorage);

            // Убираем из списка отрисовки хранилища и добавляем в список отрисовки инвентяря
            if (storage != toStorage)
            {
                inventoryWindow.drawnIcons.Remove(gameObject);
                storageWindow.drawnIcons.Add(gameObject);
                if (slot.childCount > 0) slot.GetChild(0).GetComponent<InventoryIcon>().storage = null;
                storage = toStorage;
            }
        }

        // Если в слоте уже есть иконка, то перемещаем ее 3д-объект в левую или правую руку
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

        // Помещаем иконку в слот инвентаря и делаем его исходным
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
