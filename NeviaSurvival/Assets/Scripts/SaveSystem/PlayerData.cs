using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int cold;
    public int food;
    public int energy;
    public int oxygen;
    public int stamina;

    public int maxHealth;
    public int maxCold;
    public int maxFood;
    public int maxEnergy;
    public int maxStamina;
    public int maxOxygen;

    public int maxCarryWeight;

    public int nightmares;
    public int survivalPoint;

    public bool isCold;
    public bool isCampfire;
    public bool isOnBed;
    public bool isLay;
    public bool isSit;
    public bool isSwim;
    public bool isUnderWater;
    public bool isRun;
    public bool isSleep;
    public bool isDungeon;

    public bool isControl;

    public float[] position;
    public float rotation;

    public float[] spawnPoint;

    public float saveTime;
    public int saveDay;
    public int saveMonth;
    public int saveYear;
    public int saveDayNumber;

    public List<string> questsID;
    public List <string> completedQuestsID;

    public List<bool> buildIconsState;

    public string bagID;
    public string[] itemID;
    public int[] itemDurability;

    public string[] clothesID;
    public int[] clothesDurability;

    public string rightHandID;
    public int rightHandDurability;
    public bool isRightHandBag;
    public string[] rightHandItemID;
    public int[] rightHandItemsDurability;


    public string leftHandID;
    public int leftHandDurability;
    public bool isLeftHandBag;
    public string[] leftHandItemID;
    public int[] leftHandItemsDurability;

    public bool[] isDoorLocked;
    public bool[] isDoorOpened;

    public bool[] isLampOn;

    public List<float[]> storagePosition;
    public List<float[]> storageRotation;
    public List<string[]> storagesItemId;
    public List<int[]> storagesItemDurability;

    public List<List<string[]>> storagesBagsItemsID;
    public List<List<int[]>> storagesBagsItemsDurability;

    public List<bool[]> isRandomElement;
    public List<bool> isDestructableItem;

    public string[] buildID;
    public List<float[]> buildPosition;
    public List<float[]> buildRotation;
    public int[] buildDurability;

    public string[] worldItemsID;
    public List<float[]> worldItemsPosition;
    public List<float[]> worldItemsRotation;
    public int[] worldItemsDurability;

    public string[] worldBagsID;
    public List<float[]> worldBagsPosition;
    public List<float[]> worldBagsRotation;
    public List<string[]> worldBagsItemsID;
    public List<int[]> worldBagsItemsDurability;

    public PlayerData (Player player)
    {
        health = player.Health;
        cold = player.Cold;
        food = player.Food;
        energy = player.Energy;

        oxygen = player.Oxygen;
        stamina = player.Stamina; 

        maxHealth = player.maxHealth;
        maxCold = player.maxCold;
        maxFood = player.maxFood;
        maxEnergy = player.maxEnergy;
        maxStamina = player.maxStamina;
        maxOxygen = player.maxOxygen;
        maxCarryWeight = player.maxCarryWeight;

        nightmares = player.nighmares;
        survivalPoint = player.skillPoint;

        isCold = player.isCold;
        isCampfire = player.isCampfire;
        isOnBed = player.isOnBed;
        isLay = player.isLay;
        isSit = player.isSit;
        isSwim = player.isSwim;
        isUnderWater = player.isUnderWater;
        isRun = player.isRun;
        isSleep = player.isSleep;
        isControl = player.isControl;

        position = new float[3];
        Vector3 playerPosition = player.transform.position;
        position[0] = playerPosition.x;
        position[1] = playerPosition.y;
        position[2] = playerPosition.z;

        Vector3 playerRotation = player.transform.rotation.eulerAngles;
        rotation = playerRotation.y;

        spawnPoint = new float[3];
        Vector3 lastSpawnPoint = player.spawnPoint.transform.position;
        spawnPoint[0] = lastSpawnPoint.x;
        spawnPoint[1] = lastSpawnPoint.y;
        spawnPoint[2] = lastSpawnPoint.z;

        saveTime = player.links.dayNight.hour;
        saveDay = player.links.cycle.Day;
        saveMonth = player.links.cycle.Month;
        saveYear = player.links.cycle.Year;
        saveDayNumber = player.links.dayNight.thisDay;

        questsID = new List<string>();
        completedQuestsID = new List<string>();
        foreach (QuestData quest in player.links.questHandler.takenQuestList)
        {
            questsID.Add(quest.id);
        }
        foreach (QuestData quest in player.links.questHandler.completedQuests)
        {
            completedQuestsID.Add(quest.id);
        }

        buildIconsState = new List<bool>();
        for (int i = 0; i < player.links.buildingHandler.allBuildIcons.Count; i++)
        {
            buildIconsState.Add(player.links.buildingHandler.allBuildIcons[i].activeSelf);
        }
        

        InventoryWindow inventory = player.inventoryWindow;

        if (inventory.BackpackItem != null)
        {
            bagID = inventory.BackpackItem.id;
            var bag = inventory.Backpack.GetComponent<Inventory>().inventoryItems;
            var objects = inventory.Backpack.GetComponent<Inventory>().inventoryItemObjects;
            int count = bag.Count;
            itemID = new string[count];
            itemDurability = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (bag[i] != null) itemID[i] = bag[i].id;

                if (objects[i] != null) itemDurability[i] = objects[i].GetComponent<ItemInfo>().durability;
            }
        }

        clothesID = new string[inventory.ClothesItems.Count];
        clothesDurability = new int[inventory.ClothesItems.Count];

        for (int i = 0; i < inventory.ClothesItems.Count; i++)
        {
            if (inventory.ClothesItems[i] != null)
            {
                clothesID[i] = inventory.ClothesItems[i].id;
                clothesDurability[i] = inventory.Clothes[i].GetComponent<ItemInfo>().durability;
            }
 
        }

        if (inventory.RightHandItem != null)
        {
            rightHandID = inventory.RightHandItem.id;
            rightHandDurability = inventory.RightHandObject.GetComponent<ItemInfo>().durability;
            isRightHandBag = inventory.RightHandObject.TryGetComponent(out Inventory inventory2);
        }

        if (isRightHandBag)
        {
            var bag = inventory.RightHandObject.GetComponent<Inventory>().inventoryItems;
            var objects = inventory.RightHandObject.GetComponent<Inventory>().inventoryItemObjects;
            int count = bag.Count;
            rightHandItemID = new string[count];
            rightHandItemsDurability = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (bag[i] != null) rightHandItemID[i] = bag[i].id;

                if (objects[i] != null)
                {
                    Debug.Log("Save durability in slot " + i + " = " + objects[i].GetComponent<ItemInfo>().durability);
                    rightHandItemsDurability[i] = objects[i].GetComponent<ItemInfo>().durability;
                }
            }
        }

        if (inventory.LeftHandItem != null)
        {
            leftHandID = inventory.LeftHandItem.id;
            leftHandDurability = inventory.LeftHandObject.GetComponent<ItemInfo>().durability;
            isLeftHandBag = inventory.LeftHandObject.TryGetComponent(out Inventory inventory3);
        }

        if (isLeftHandBag)
        {
            var bag = inventory.LeftHandObject.GetComponent<Inventory>().inventoryItems;
            var objects = inventory.LeftHandObject.GetComponent<Inventory>().inventoryItemObjects;
            int count = bag.Count;
            leftHandItemID = new string[count];
            leftHandItemsDurability = new int[count];
            for (int i = 0; i < count; i++)
            {
                if (bag[i] != null) leftHandItemID[i] = bag[i].id;

                if (objects[i] != null) leftHandItemsDurability[i] = objects[i].GetComponent<ItemInfo>().durability;
            }
        }

        isDoorLocked = new bool[player.links.saveObjects.doors.Count];
        isDoorOpened = new bool[player.links.saveObjects.doors.Count];

        for (int i = 0; i < player.links.saveObjects.doors.Count; i++)
        {
            isDoorLocked[i] = player.links.saveObjects.doors[i].isLocked;
            isDoorOpened[i] = player.links.saveObjects.doors[i].isOpen;
        }

        isLampOn = new bool[player.links.saveObjects.lamps.Count];

        for (int i = 0; i < player.links.saveObjects.lamps.Count; i++)
        {
            isLampOn[i] = player.links.saveObjects.lamps[i].isOn;
        }

        storagePosition = new List<float[]>();
        storageRotation = new List<float[]>();
        storagesItemId = new List<string[]>();
        storagesItemDurability = new List<int[]>();

        storagesBagsItemsID = new List<List<string[]>>();
        storagesBagsItemsDurability = new List<List<int[]>>();

        for (int i = 0; i < player.links.saveObjects.storages.Count; i++)
        {
            var vector3 = new float[3];
            vector3[0] = player.links.saveObjects.storages[i].transform.position.x;
            vector3[1] = player.links.saveObjects.storages[i].transform.position.y;
            vector3[2] = player.links.saveObjects.storages[i].transform.position.z;
            storagePosition.Add(vector3);

            var euler3 = new float[3];
            euler3[0] = player.links.saveObjects.storages[i].transform.eulerAngles.x;
            euler3[1] = player.links.saveObjects.storages[i].transform.eulerAngles.y;
            euler3[2] = player.links.saveObjects.storages[i].transform.eulerAngles.z;
            storageRotation.Add(euler3);

            var items = player.links.saveObjects.storages[i].storageItems;
            var objects = player.links.saveObjects.storages[i].storageItemObjects;
            int count = items.Count;

            var itemID = new string[count];
            var itemDurability = new int[count];

            var ItemsInBagInStorageID = new List<string[]>();
            var ItemsInBagInStorageDurability = new List<int[]>();

            for (int j = 0; j < count; j++)
            {
                if (items[j] != null) itemID[j] = items[j].id;

                var itemsInBagID = new string[16];
                var itemsInBagDurability = new int[16];

                if (objects[j] != null)
                {
                    itemDurability[j] = objects[j].GetComponent<ItemInfo>().durability;
                    if (objects[j].TryGetComponent(out Inventory bag))
                    {
                        var itemsInBagID2 = new string[bag.inventoryItems.Count];
                        var itemsInBagDurability2 = new int[bag.inventoryItems.Count];

                        for (int k = 0; k < bag.inventoryItems.Count; k++)
                        {
                            if (bag.inventoryItems[k] != null)
                            {
                                itemsInBagID2[k] = bag.inventoryItems[k].id;
                                if (bag.inventoryItemObjects[k] != null)
                                    itemsInBagDurability2[k] = bag.inventoryItemObjects[k].GetComponent<ItemInfo>().durability;
                            }
                        }

                        itemsInBagID = itemsInBagID2;
                        itemsInBagDurability = itemsInBagDurability2;
                    }

                    
                }
                ItemsInBagInStorageID.Add(itemsInBagID);
                ItemsInBagInStorageDurability.Add(itemsInBagDurability);
            }

            storagesItemId.Add(itemID);
            storagesItemDurability.Add(itemDurability);
            storagesBagsItemsID.Add(ItemsInBagInStorageID);
            storagesBagsItemsDurability.Add(ItemsInBagInStorageDurability);
        }

        isRandomElement = new List<bool[]>();

        for (int i = 0; i < player.links.saveObjects.randomElements.Count; i++)
        {
            var elements = new bool[player.links.saveObjects.randomElements[i].elements.Count];
            for (int j = 0; j < player.links.saveObjects.randomElements[i].elements.Count; j++)
            {
                elements[j] = player.links.saveObjects.randomElements[i].elements[j].activeSelf;
            }
            isRandomElement.Add(elements);
        }
        
        isDestructableItem = new List<bool>();

        for (int i = 0; i < player.links.saveObjects.destructableItems.Count; i++)
        {
            isDestructableItem.Add(player.links.saveObjects.destructableItems[i].gameObject.activeSelf);
        }

        List<GameObject> builds = new List<GameObject>(); 
        foreach (Build build in player.links.saveObjects.playerBuildings.transform.GetComponentsInChildren<Build>())
        {
            builds.Add(build.gameObject);
        }

        buildID = new string[builds.Count];
        buildPosition = new List<float[]>();
        buildRotation = new List<float[]>();
        buildDurability = new int[builds.Count];

        for (int i = 0; i < builds.Count; i++)
        {
            buildID[i] = builds[i].GetComponent<Build>().buildData.id;

            var vector3 = new float[3];
            vector3[0] = builds[i].transform.position.x;
            vector3[1] = builds[i].transform.position.y;
            vector3[2] = builds[i].transform.position.z;
            buildPosition.Add(vector3);

            var euler3 = new float[3];
            euler3[0] = builds[i].transform.eulerAngles.x;
            euler3[1] = builds[i].transform.eulerAngles.y;
            euler3[2] = builds[i].transform.eulerAngles.z;
            buildRotation.Add(euler3);

            buildDurability[i] = builds[i].GetComponent<ItemInfo>().durability;
        }

        List<ItemInfo> worldItems = new List<ItemInfo>();
        List<Inventory> worldBags = new List<Inventory>();
        foreach (ItemInfo item in player.links.saveObjects.allWorldItems.transform.GetComponentsInChildren<ItemInfo>())
        {
            if (item.gameObject.activeSelf)
            {
                if (item.TryGetComponent(out Inventory bag)) worldBags.Add(bag);
                else worldItems.Add(item);

                if (player.mousePoint.carryObject != null && !player.mousePoint.carryObject.TryGetComponent(out Storage storage))
                {
                    // Для предметов перетаскиваемых в 2 руках тоже нужно создать СО (пока это только кучи листьев).
                }
            }
        }

        worldItemsID = new string[worldItems.Count];
        worldItemsPosition = new List<float[]>();
        worldItemsRotation = new List<float[]>();
        worldItemsDurability = new int[worldItems.Count];

        for (int i = 0; i < worldItems.Count; i++)
        {
            worldItemsID[i] = worldItems[i].item.id;

            var vector3 = new float[3];
            vector3[0] = worldItems[i].transform.position.x;
            vector3[1] = worldItems[i].transform.position.y;
            vector3[2] = worldItems[i].transform.position.z;
            worldItemsPosition.Add(vector3);

            var euler3 = new float[3];
            euler3[0] = worldItems[i].transform.eulerAngles.x;
            
            euler3[1] = worldItems[i].transform.eulerAngles.y;
            euler3[2] = worldItems[i].transform.eulerAngles.z;
            worldItemsRotation.Add(euler3);

            worldItemsDurability[i] = worldItems[i].durability;
        }

        worldBagsID = new string[worldBags.Count];
        worldBagsPosition = new List<float[]>();
        worldBagsRotation = new List<float[]>();
        worldBagsItemsID = new List<string[]>();
        worldBagsItemsDurability = new List<int[]>();

        for (int i = 0; i < worldBags.Count; i++)
        {
            worldBagsID[i] = worldBags[i].GetComponent<ItemInfo>().item.id;

            var vector3 = new float[3];
            vector3[0] = worldBags[i].transform.position.x;
            vector3[1] = worldBags[i].transform.position.y;
            vector3[2] = worldBags[i].transform.position.z;
            worldBagsPosition.Add(vector3);

            var euler3 = new float[3];
            euler3[0] = worldBags[i].transform.eulerAngles.x;
            euler3[1] = worldBags[i].transform.eulerAngles.y;
            euler3[2] = worldBags[i].transform.eulerAngles.z;
            worldBagsRotation.Add(euler3);

            var items = new string[worldBags[i].inventoryItems.Count];
            var durabilities = new int[worldBags[i].inventoryItems.Count];

            for (int j = 0; j < worldBags[i].inventoryItems.Count; j++)
            {
                if (worldBags[i].inventoryItems[j] != null)
                {
                    items[j] = worldBags[i].inventoryItems[j].id;
                    if (worldBags[i].inventoryItemObjects[j] != null)
                        durabilities[j] = worldBags[i].inventoryItemObjects[j].GetComponent<ItemInfo>().durability;
                }
            }

            worldBagsItemsID.Add(items);
            worldBagsItemsDurability.Add(durabilities);
        }
        isDungeon = player.isDungeon;
    }
}
