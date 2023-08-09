using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveLoad : MonoBehaviour
{
    [SerializeField] Player player;

    public void SavePlayer()
    {
        BinarySavingSystem.SavePlayer(player);
    }

    public void LoadPlayer()
    {
        PlayerData data = BinarySavingSystem.LoadPlayer();

        player.Health = data.health;
        player.Cold = data.cold;
        player.Food = data.food;
        player.Energy = data.energy;

        player.Oxygen = data.oxygen;
        player.Stamina = data.stamina;

        player.maxHealth = data.maxHealth;
        player.maxCold = data.maxCold;
        player.maxFood = data.maxFood;
        player.maxEnergy = data.maxEnergy;
        player.maxStamina = data.maxStamina;
        player.maxOxygen = data.maxOxygen;
        player.maxCarryWeight = data.maxCarryWeight;

        player.nighmares = data.nightmares;
        player.survivalPoint = data.survivalPoint;
        player.UpdateSkillPoints();

        player.isCold = data.isCold;
        player.isCampfire = data.isCampfire;
        player.isOnBed = data.isOnBed;
        player.isLay = data.isLay;
        player.isSit = data.isSit;
        player.isSwim = data.isSwim;
        player.isUnderWater = data.isUnderWater;
        player.isRun = data.isRun;
        player.isSleep = data.isSleep;
        player.isControl = data.isControl;

        player.links.sky.Cycle.Hour = data.saveTime;
        player.links.sky.Cycle.Day = data.saveDay;
        player.links.sky.Cycle.Month = data.saveMonth;
        player.links.sky.Cycle.Year = data.saveYear;
        player.DayTime.thisDay = data.saveDayNumber;

        player.saveTime = data.saveTime;
        player.saveDay = data.saveDay;
        player.saveMonth = data.saveMonth;
        player.saveYear = data.saveYear;
        player.saveDayNumber = data.saveDayNumber;

        player.links.questHandler.questList.Clear();
        for (int i = 0; i < data.questsID.Count; i++)
        {
            player.links.questHandler.takenQuestList.Add(QuestById(data.questsID[i]));
        }

        player.links.questHandler.completedQuests.Clear();
        for (int i = 0; i < data.completedQuestsID.Count; i++)
        {
            player.links.questHandler.completedQuests.Add(QuestById(data.questsID[i]));
        }

        player.links.questWindow.QuestUpdate();

        for (int i = 0; i < player.links.buildingHandler.allBuildIcons.Count; i++)
        {
            if (data.buildIconsState.Count > i)
                player.links.buildingHandler.allBuildIcons[i].SetActive(data.buildIconsState[i]);
        }
        

        player.inventoryWindow.BackpackItem = ItemById(data.bagID);
        player.links.inventoryWindow.Redraw();

        // Ќужно ли это?
        player.ui.inventoryPanel.SetActive(true);

        StartCoroutine(LoadDelay(data));

        for (int i = 0; i < player.links.saveObjects.doors.Count; i++)
        {
            player.links.saveObjects.doors[i].isLocked = data.isDoorLocked[i];
            player.links.saveObjects.doors[i].isOpen = data.isDoorOpened[i];
            player.links.saveObjects.doors[i].ConfigureDoor();
        }

        if (data.isLampOn != null) // „тобы не ломать старые сохранени€ надо добавл€ть такую проверку к новым данным
        for (int i = 0; i < player.links.saveObjects.lamps.Count; i++)
        {
            player.links.saveObjects.lamps[i].isOn = data.isLampOn[i];
            player.links.saveObjects.lamps[i].ConfigureLamp();
        }

        for (int i = 0; i < player.links.saveObjects.storages.Count; i++)
        {
            player.links.saveObjects.storages[i].transform.position =
                new Vector3(data.storagePosition[i][0], data.storagePosition[i][1], data.storagePosition[i][2]);

            player.links.saveObjects.storages[i].transform.rotation =
                Quaternion.Euler(new Vector3(data.storageRotation[i][0], data.storageRotation[i][1], data.storageRotation[i][2]));

            var count = player.links.saveObjects.storages[i].storageItems.Count;
            player.links.saveObjects.storages[i].storageItems.Clear();

            for (int j = 0; j < count; j++)
            {
                player.links.saveObjects.storages[i].storageItems.Add(ItemById(data.storagesItemId[i][j]));
                if (player.links.saveObjects.storages[i].storageItems[j] != null)
                {
                    var newObject = Instantiate(player.links.saveObjects.storages[i].storageItems[j].Prefab, player.links.savedDropedItemsParent);
                    player.links.saveObjects.storages[i].storageItemObjects[j] = newObject;
                    newObject.GetComponent<ItemInfo>().durability = data.storagesItemDurability[i][j];
                    newObject.SetActive(false);

                    if (newObject.TryGetComponent(out Inventory bag))
                    {
                        for (int k = 0; k < bag.inventoryItems.Count; k++)
                        {
                            bag.inventoryItems[k] = ItemById(data.storagesBagsItemsID[i][j][k]);
                            if (bag.inventoryItems[k] != null)
                            {
                                var newObject2 = Instantiate(bag.inventoryItems[k].Prefab, player.links.savedDropedItemsParent);
                                bag.inventoryItemObjects[k] = newObject2;
                                newObject2.GetComponent<ItemInfo>().durability = data.storagesBagsItemsDurability[i][j][k];
                                newObject2.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < player.links.saveObjects.randomElements.Count; i++)
        {
            for (int j = 0; j < player.links.saveObjects.randomElements[i].elements.Count; j++)
            {
                player.links.saveObjects.randomElements[i].elements[j].SetActive(data.isRandomElement[i][j]);
            }
        }

        for (int i = 0; i < player.links.saveObjects.destructableItems.Count; i++)
        {
            player.links.saveObjects.destructableItems[i].gameObject.SetActive(data.isDestructableItem[i]);
        }

        for (int i = 0; i < data.buildPosition.Count; i++)
        {
            BuildData build = BuildById(data.buildID[i]);
            GameObject building = Instantiate(build.Prefab, player.links.playerBuildings);
            building.transform.position = new Vector3(data.buildPosition[i][0], data.buildPosition[i][1], data.buildPosition[i][2]);
            building.transform.rotation = 
                Quaternion.Euler(new Vector3(data.buildRotation[i][0], data.buildRotation[i][1], data.buildRotation[i][2]));
            building.GetComponent<ItemInfo>().durability = data.buildDurability[i];
        }

        foreach (ItemInfo item in player.links.saveObjects.existWorldItems.transform.GetComponentsInChildren<ItemInfo>())
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < data.worldItemsPosition.Count; i++)
        {
            GameObject item = Instantiate(ItemById(data.worldItemsID[i]).Prefab, player.links.savedDropedItemsParent);
            item.transform.position = new Vector3(data.worldItemsPosition[i][0], data.worldItemsPosition[i][1], data.worldItemsPosition[i][2]);
            item.transform.rotation =
                Quaternion.Euler(new Vector3(data.worldItemsRotation[i][0], data.worldItemsRotation[i][1], data.worldItemsRotation[i][2]));
            item.GetComponent<ItemInfo>().durability = data.worldItemsDurability[i];
        }

        for (int i = 0; i < data.worldBagsPosition.Count; i++)
        {
            GameObject bag = Instantiate(ItemById(data.worldBagsID[i]).Prefab, player.links.savedDropedItemsParent);
            bag.transform.position = new Vector3(data.worldBagsPosition[i][0], data.worldBagsPosition[i][1], data.worldBagsPosition[i][2]);
            bag.transform.rotation =
                Quaternion.Euler(new Vector3(data.worldBagsRotation[i][0], data.worldBagsRotation[i][1], data.worldBagsRotation[i][2]));

            Inventory inventory = bag.GetComponent<Inventory>();
            for (int j = 0; j < inventory.inventoryItems.Count; j++)
            {
                inventory.inventoryItems[j] = ItemById(data.worldBagsItemsID[i][j]);
                if (inventory.inventoryItems[j] != null)
                {
                    GameObject newObject = Instantiate(inventory.inventoryItems[j].Prefab, player.links.savedDropedItemsParent);
                    inventory.inventoryItemObjects[j] = newObject;
                    newObject.GetComponent<ItemInfo>().durability = data.worldBagsItemsDurability[i][j];
                    newObject.SetActive(false);
                }
            }
        }

        player.isDungeon = data.isDungeon;
        player.Light();
    }

    IEnumerator LoadDelay(PlayerData data)
    {
        yield return null;
        player.inventoryWindow.LoadInventory(data);
        
        player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        player.transform.rotation = Quaternion.Euler(Vector3.zero);
        player.transform.Rotate(0, data.rotation, 0);

        player.spawnPoint.transform.position = new Vector3(data.spawnPoint[0], data.spawnPoint[1], data.spawnPoint[2]);
        
        player.gameObject.SetActive(true);
        player.links.ScottyCamera.gameObject.SetActive(false);
        player.cameraController.RotateCameraToPlayer(false);
    }

    Item ItemById(string id)
    {
        if (id == null) return null;
        else return player.inventoryWindow.ItemsList[id];
    }

    BuildData BuildById(string id)
    {
        if (id == null) return null;
        else return player.links.buildingHandler.BuildList[id];
    }

    QuestData QuestById(string id)
    {
        if (id == null) return null;
        else return player.links.questWindow.allQuestList[id];
    }
}
