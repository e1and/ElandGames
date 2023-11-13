using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInventory : MonoBehaviour
{
    Links links;
    InventoryWindow slots;


    public List<Item> SaveSlots = new List<Item>(10);
    public List<GameObject> SaveSlotObjects = new List<GameObject>(10);
    public GameObject SaveBackpack;
    public GameObject SaveLeftHandObject;
    public GameObject SaveRightHandObject;
    public GameObject SaveCarryObject;

    void Start()
    {
        links = FindObjectOfType<Links>();
        slots = links.inventoryWindow;
        FindContainers();
        FindBags();
        FindItemsOnLocations();
        SaveItemPositions();
    }
    public void SaveItems()
    {
        SaveContainers();
        
        SaveSlots[0] = slots.LeftHandItem;
        SaveSlotObjects[0] = slots.LeftHandObject;

        SaveSlots[1] = slots.RightHandItem;
        SaveSlotObjects[1] = slots.RightHandObject;

        SaveSlots[2] = slots.BackpackItem;
        SaveSlotObjects[2] = slots.Backpack;

        for (int i = 0; i < 7; i++)
        {
            SaveSlots[i+3] = slots.ClothesItems[i];
            SaveSlotObjects[i+3] = slots.Clothes[i];
        }

        int count = slots.dropParent.childCount;
        for (int i = 0; i < count; i++)
        {
            if (slots.dropParent.GetChild(0).gameObject.TryGetComponent(out ItemInfo info))
            {
                info.savePosition = slots.dropParent.GetChild(0).transform.position;
                if (slots.dropParent.GetChild(0).gameObject.activeSelf) info.isSaveItemActive = true;
            }
            slots.dropParent.GetChild(0).transform.parent = links.savedDropedItemsParent;
        }

        SaveItemPositions();
        SaveBuildings();
    }

    public void LoadItems()
    {
        LoadContainers();
        LoadItemPositions();
        LoadBags();
        DeleteUnsavedBuildings();

        for (int i = 0; i < slots.dropParent.childCount; i++)
        {
            // Удаляем брошенные предметы, которые не были сохранены, кроме уникальных
            if (slots.dropParent.GetChild(i).gameObject.TryGetComponent(out ItemInfo info) && !info.isSaveItemActive)
            slots.dropParent.GetChild(i).gameObject.SetActive(false);
            if (info.isSaveItemActive)
            {
                info.gameObject.transform.position = info.savePosition + new Vector3(0, 1, 0);
                info.gameObject.transform.parent = links.savedDropedItemsParent;
            }
        }

        // Если предмет в слоте изменился, то новый предмет дропается на месте смерти
        if (SaveSlotObjects[2] != slots.Backpack)
        {
            if (slots.BackpackItem != null) DropUnsavedItem(slots.backpackSlot);
        }
        // Если слоте есть сохраненный предмет, то при загрузке ставим его в слот, в случае рюкзака загружаем и его слоты
        if (SaveSlots[2] != null)
        {
            slots.BackpackItem = SaveSlots[2];
            slots.Backpack = SaveSlotObjects[2];
        }

        if (SaveSlotObjects[0] != slots.LeftHandObject)
        {
            if (slots.LeftHandItem != null) DropUnsavedItem(slots.leftHandSlot);
        }
        if (SaveSlotObjects[0] != null)
        {
            slots.LeftHandItem = SaveSlots[0];
            slots.LeftHandObject = SaveSlotObjects[0];
        }

        if (SaveSlotObjects[1] != slots.RightHandObject)
        {
            if (slots.RightHandItem != null) DropUnsavedItem(slots.rightHandSlot);
        }
        if (SaveSlotObjects[1] != null)
        {
            slots.RightHandObject = SaveSlotObjects[1];
            slots.RightHandItem = SaveSlots[1];
        }      

        if (slots.Belt != SaveSlotObjects[3])
        { 
            if (slots.BeltItem != null) DropUnsavedItem(slots.beltSlot);
            slots.BeltItem = SaveSlots[3];
            slots.Belt = SaveSlotObjects[3];
        }

        if (slots.Feet != SaveSlotObjects[4])
        {
            if (slots.FeetItem != null) DropUnsavedItem(slots.feetSlot);
            slots.FeetItem = SaveSlots[4];
            slots.Feet = SaveSlotObjects[4];
        }

        if (slots.Legs != SaveSlotObjects[5])
        {
            if (slots.LegsItem != null) DropUnsavedItem(slots.legsSlot);
            slots.LegsItem = SaveSlots[5];
            slots.Legs = SaveSlotObjects[5];
        }

        if (slots.Arms != SaveSlotObjects[6])
        {
            if (slots.ArmsItem != null) DropUnsavedItem(slots.armsSlot);
            slots.ArmsItem = SaveSlots[6];
            slots.Arms = SaveSlotObjects[6];
        }

        if (slots.Body != SaveSlotObjects[7])
        {
            if (slots.BodyItem != null) DropUnsavedItem(slots.bodySlot);
            slots.BodyItem = SaveSlots[7];
            slots.Body = SaveSlotObjects[7];
        }

        if (slots.Shoulders != SaveSlotObjects[8])
        {
            if (slots.BeltItem != null) DropUnsavedItem(slots.shouldersSlot);
            slots.ShouldersItem = SaveSlots[8];
            slots.Shoulders = SaveSlotObjects[8];
        }

        if (slots.Head != SaveSlotObjects[9])
        {
            if (slots.HeadItem != null) DropUnsavedItem(slots.headSlot);
            slots.HeadItem = SaveSlots[9];
            slots.Head = SaveSlotObjects[9];
        }

        // Отображает нужные элементы одежды и пересчитывает их бонусы, перерисовывает все иконки и связывает их с объектами
        slots.Redraw();
    }

    void DropUnsavedItem(RectTransform slot)
    {
        // Дроп предмета на месте смерти (уже не нужно, но может пригодится)
        GameObject item = slot.GetChild(0).GetComponent<InventoryIcon>().item3dObject;
        slot.GetChild(0).GetComponent<InventoryIcon>().Drop();
        item.transform.position = links.player.deathPlace + new Vector3(Random.Range(0.2f, 1f), 1f, Random.Range(0.2f, 1f));
    }

    public List<Storage> storages = new List<Storage>();
    public List<List<Item>> saveStorageItems = new List<List<Item>>();
    public List<List<GameObject>> saveStorageItemObjects = new List<List<GameObject>>();

    void FindContainers()
    {
        // Поиск всех хранилищ на сцене
        Storage[] allStorages = FindObjectsOfType<Storage>();

        for (int i = 0; i < allStorages.Length; i++)
        {
                storages.Add(allStorages[i]);
                saveStorageItems.Add(allStorages[i].storageItems);
                saveStorageItemObjects.Add(allStorages[i].storageItemObjects);
        }
    }

    public List<Inventory> bags = new List<Inventory>();
    public List<List<Item>> saveBagItems = new List<List<Item>>();
    public List<List<GameObject>> saveBagItemObjects = new List<List<GameObject>>();

    void FindBags()
    {
        // Поиск всех сумок на уровне
        Inventory[] bagsArray = FindObjectsOfType<Inventory>();

        for (int i = 0; i < bagsArray.Length; i++)
        {
            if (!bags.Contains(bagsArray[i]))
            {
                bags.Add(bagsArray[i]);
                saveBagItems.Add(bagsArray[i].inventoryItems);
                saveBagItemObjects.Add(bagsArray[i].inventoryItemObjects);
            }
        }
        bagItems2 = bags[0].inventoryItems;
        bagItems2Objects = bags[0].inventoryItemObjects;
        SaveBags();
    }

    // Для отслеживания работы хранилищ (можно удалить)
    public List<Item> bagItems2 = new List<Item>();
    public List<GameObject> bagItems2Objects = new List<GameObject>();

    public void SaveBags()
    {
        // Сохранение содержимого всех сумок
        for (int i = 0; i < bags.Count; i++)
        {
            saveBagItems[i] = new List<Item>(bags[i].inventoryItems);
            saveBagItemObjects[i] = new List<GameObject>(bags[i].inventoryItemObjects);
        }
    }

    public void AddNewSaveBag(Inventory bag)
    {
        saveBagItems.Add(new List<Item>(bag.inventoryItems));
        saveBagItemObjects.Add(new List<GameObject>(bag.inventoryItemObjects));
    }    

    public void SaveContainers()
    {
        // Сохранение содержимого всех хранилищ
        for (int i = 0; i < storages.Count; i++)
        {
                saveStorageItems[i] = new List<Item>(storages[i].storageItems);
                saveStorageItemObjects[i] = new List<GameObject>(storages[i].storageItemObjects);
        }

        storageItems2 = saveStorageItems[0];
        storageItems2Objects = saveStorageItemObjects[0];
    }

    // Для отслеживания работы хранилищ (можно удалить)
    public List<Item> storageItems2 = new List<Item>();
    public List<GameObject> storageItems2Objects = new List<GameObject>();
    public List<Item> storageItems3 = new List<Item>();

    void LoadContainers()
    {
        // Загрузка содержимого всех хранилищ
        for (int i = 0; i < storages.Count; i++)
        {
            for (int j = 0; j < storages[i].storageItems.Count; j++)
            {
                storages[i].storageItems[j] = saveStorageItems[i][j];
                storages[i].storageItemObjects[j] = saveStorageItemObjects[i][j];
            }
        }
    }

    void LoadBags()
    {
        // Загрузка сохраненного содержимого всех сумок
        for (int i = 0; i < bags.Count; i++)
        {
            for (int j = 0; j < bags[i].inventoryItems.Count; j++)
            {
                bags[i].inventoryItems[j] = saveBagItems[i][j];
                bags[i].inventoryItemObjects[j] = saveBagItemObjects[i][j];
            }
        }
    }

    public List<GameObject> items = new List<GameObject>();

    void FindItemsOnLocations()
    {
        // Поиск всех предметов на уровне, чье положение будет сохраняться
        for (int i = 0; i < links.itemsOnLocationsParent.childCount; i++)
        {
            items.Add(links.itemsOnLocationsParent.GetChild(i).gameObject);
        }
    }

    void SaveItemPositions()
    {
        // Сохранение начальных положений всех предметов на уровне (предметы должны быть в паренте Items)
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].TryGetComponent(out ItemInfo info))
            {
                info.savePosition = items[i].transform.position;
                if (info.gameObject.activeSelf) info.isSaveItemActive = true;
            }
        }

        // Сохранение положения всех перемещенных предметов
        for (int i = 0; i < links.savedDropedItemsParent.childCount; i++)
        {
            if (links.savedDropedItemsParent.GetChild(i).gameObject.TryGetComponent(out ItemInfo info) && info.isMovable)
            {
                info.savePosition = links.savedDropedItemsParent.GetChild(i).gameObject.transform.position;
                if (info.gameObject.activeSelf) info.isSaveItemActive = true;
            }
        }

        // Сохранение положения всех перемещаемых хранилищ (сундуки, бочки)
        for (int i = 0; i < storages.Count; i++)
        {
            if (storages[i].gameObject.TryGetComponent(out ItemInfo info) && info.isMovable)
            {
                if (storages[i].gameObject.transform.position != info.savePosition)
                    info.savePosition = storages[i].gameObject.transform.position;

                if (storages[i].gameObject.transform.rotation != info.saveRotation)
                    info.saveRotation = storages[i].gameObject.transform.rotation;

                if (info.gameObject.activeSelf) info.isSaveItemActive = true;
            }
        }
    }

    void LoadItemPositions()
    {
        // Загрузка сохраненных положений всех предметов на уровне
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].TryGetComponent(out ItemInfo info) && info.isMovable)
            {
                if (items[i].transform.position != info.savePosition)
                items[i].transform.position = info.savePosition + new Vector3(0, 1, 0);

                if (info.isSaveItemActive) info.gameObject.SetActive(true);
                if (info.gameObject.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
                if (info.gameObject.TryGetComponent(out BoxCollider collider)) collider.enabled = true;
                if (info.gameObject.TryGetComponent(out MeshCollider mesh)) mesh.enabled = true;
            }
        }

        // Загрузка положений всех перемещенных и сохраненных предметов на уровне
        for (int i = 0; i < links.savedDropedItemsParent.childCount; i++)
        {
            if (links.savedDropedItemsParent.GetChild(i).gameObject.TryGetComponent(out ItemInfo info) && info.isMovable)
            {
                if (links.savedDropedItemsParent.GetChild(i).gameObject.transform.position != info.savePosition)
                    links.savedDropedItemsParent.GetChild(i).gameObject.transform.position =
                info.savePosition + new Vector3(0, 1, 0);

                if (info.isSaveItemActive) info.gameObject.SetActive(true);
                if (info.gameObject.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
                if (info.gameObject.TryGetComponent(out BoxCollider collider)) collider.enabled = true;
                if (info.gameObject.TryGetComponent(out MeshCollider mesh)) mesh.enabled = true;
            }
        }

        // Загрузка сохраненных положений всех больших перемещаемых предметов
        for (int i = 0; i < storages.Count; i++)
        { 
            if (storages[i].gameObject.TryGetComponent(out ItemInfo info) && info.isMovable)
            {
                if (storages[i].gameObject.transform.position != info.savePosition) 
                    storages[i].gameObject.transform.position = info.savePosition + new Vector3(0, 1, 0);

                if (storages[i].gameObject.transform.rotation != info.saveRotation)
                    storages[i].gameObject.transform.rotation = info.saveRotation;
            }      
        }
    }

    void SaveBuildings()
    {
        for (int i = 0; i < links.playerBuildings.childCount; i++)
        {
            links.playerBuildings.GetChild(i).gameObject.GetComponent<ItemInfo>().savePosition =
                links.playerBuildings.GetChild(i).gameObject.transform.position;
        }
    }

    void DeleteUnsavedBuildings()
    {
        for (int i = 0; i < links.playerBuildings.childCount; i++)
        {
            if (links.playerBuildings.GetChild(i).gameObject.GetComponent<ItemInfo>().savePosition == Vector3.zero)
                Destroy(links.playerBuildings.GetChild(i).gameObject);
        }
    }
}
