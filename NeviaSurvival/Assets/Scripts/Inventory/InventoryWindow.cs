using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType 
{
    Bag = 102,
    Belt = 103,
    Feet = 104,
    Legs = 105,
    Hands = 106,
    Body = 107,
    Shoulders = 108,
    Head = 109,

    Food = 200,
    Wood, 
    Tool, 
    Key, 
    Cauldron, 
    Scroll,
    GrassStack,
    Torch,
    Axe,
    Cloth
}

public class InventoryWindow : MonoBehaviour
{
    public List<Item> allItems;
    public Dictionary<string, Item> ItemsList;

    public Inventory inventory;
    public RectTransform[] slots;
    public GameObject inventoryCellTemplate;
    [SerializeField] Sprite emptySlotImage;

    [Header("3д паренты для позиционпирования вещей на персонаже")]
    public Transform rightHandParent;
    public Transform leftHandParent;
    public Transform backpackParent;
    public Transform headParent;
    public Transform dropParent;

    [Header("Парент для перетаскивания иконок")]
    public Transform draggingParent;

    [Header("UI слоты для предметов")]
    public RectTransform leftHandSlot;   // 100
    public RectTransform rightHandSlot;  // 101
    public RectTransform backpackSlot;   // 102
    public RectTransform beltSlot;       // 103
    public RectTransform feetSlot;       // 104
    public RectTransform legsSlot;       // 105
    public RectTransform armsSlot;      // 106
    public RectTransform bodySlot;       // 107
    public RectTransform shouldersSlot;  // 108
    public RectTransform headSlot;       // 109

    [Header("Объекты предметов в слотах и их Item")]
    public Item LeftHandItem;
    public GameObject LeftHandObject;
    public Item RightHandItem;
    public GameObject RightHandObject;
    public Item BackpackItem;
    public GameObject Backpack;
    public Item BeltItem;
    public GameObject Belt;
    public Item FeetItem;
    public GameObject Feet;
    public Item LegsItem;
    public GameObject Legs;
    public Item ArmsItem;
    public GameObject Arms;
    public Item BodyItem;
    public GameObject Body;
    public Item ShouldersItem;
    public GameObject Shoulders;
    public Item HeadItem;
    public GameObject Head;

    public Weapon rightHandWeapon;
    public Weapon leftHandWeapon;

    public List<GameObject> Clothes = new List<GameObject>(7);
    public List<Item> ClothesItems = new List<Item>(7);

    [SerializeField] Item itemToAdd;

    public List<GameObject> drawnIcons = new List<GameObject>();

    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();

        ItemsList = new Dictionary<string, Item>();
        allItems.AddRange(Resources.LoadAll<Item>("ScriptableObjects"));
        foreach (Item item in allItems)
        {
            if (ItemsList.ContainsKey(item.id) || item.id.Length == 0) item.id = item.name.ToLower().Replace(" ", "");
            ItemsList.Add(item.id, item);
        }


        if (Backpack != null) inventory = Backpack.GetComponent<Inventory>();
        else inventory = null;
    }
    private void Start()
    {
        if (inventory != null) inventory.onItemAdded += OnItemAdded;
        Redraw();
        //if (links.player.isStart) links.ui.inventoryPanel.SetActive(false);
        links.ui.equipmentPanel.SetActive(false);

    }

    public int ItemCount(string id)
    {
        int itemCount = 0;
        if (inventory != null)
            foreach (Item item in inventory.inventoryItems)
            {
                if (item != null && item.id == id) itemCount++;
            }

        if (RightHandItem != null && RightHandItem.id == id) itemCount++;
        if (LeftHandItem != null && LeftHandItem.id == id) itemCount++;

        if (LeftHandItem != null && LeftHandItem.Type == ItemType.Bag)
        {
            foreach (Item item in LeftHandObject.GetComponent<Inventory>().inventoryItems)
            {
                if (item != null && item.id == id) itemCount++;
            }
        }
        Debug.Log(itemCount);
        return itemCount;
    }

    void OnItemAdded(Item obj) => Redraw();

    Item ItemById(string id)
    {
        if (id == null) return null;
        else return ItemsList[id];
    }

    public void LoadInventory(PlayerData data)
    {
        if (Backpack != null)
        {
            inventory = Backpack.GetComponent<Inventory>();
            Backpack.SetActive(true);

            for (int i = 0; i < inventory.inventoryItems.Count; i++)
            {
                inventory.inventoryItems[i] = ItemById(data.itemID[i]);
            }
            
        }
        else inventory = null;

        BeltItem = ItemById(data.clothesID[0]);
        if (BeltItem != null)
        {
            Belt = Instantiate(BeltItem.Prefab);
            Belt.GetComponent<ItemInfo>().durability = data.clothesDurability[0];
        }

        FeetItem = ItemById(data.clothesID[1]);
        if (FeetItem != null)
        {
            Feet = Instantiate(FeetItem.Prefab);
            Feet.GetComponent<ItemInfo>().durability = data.clothesDurability[1];
        }

        LegsItem = ItemById(data.clothesID[2]);
        if (LegsItem != null)
        {
            Legs = Instantiate(LegsItem.Prefab);
            Legs.GetComponent<ItemInfo>().durability = data.clothesDurability[2];
        }

        ArmsItem = ItemById(data.clothesID[3]);
        if (ArmsItem != null)
        {
            Arms = Instantiate(ArmsItem.Prefab);
            Arms.GetComponent<ItemInfo>().durability = data.clothesDurability[3];
        }

        BodyItem = ItemById(data.clothesID[4]);
        if (BodyItem != null)
        {
            Body = Instantiate(BodyItem.Prefab);
            Body.GetComponent<ItemInfo>().durability = data.clothesDurability[4];
        }

        ShouldersItem = ItemById(data.clothesID[5]);
        if (ShouldersItem != null)
        {
            Shoulders = Instantiate(ShouldersItem.Prefab);
            Shoulders.GetComponent<ItemInfo>().durability = data.clothesDurability[5];
        }

        HeadItem = ItemById(data.clothesID[6]);
        if (HeadItem != null)
        {
            Head = Instantiate(HeadItem.Prefab);
            Head.GetComponent<ItemInfo>().durability = data.clothesDurability[6];
        }

        UpdateClothesVisual();

        RightHandItem = ItemById(data.rightHandID);
        if (RightHandItem != null)
        {
            RightHandObject = Instantiate(RightHandItem.Prefab);
            RightHandObject.GetComponent<ItemInfo>().durability = data.rightHandDurability;
            RightHandObject.SetActive(true);
        }
        
        LeftHandItem = ItemById(data.leftHandID);
        if (LeftHandItem != null)
        {
            LeftHandObject = Instantiate(LeftHandItem.Prefab);
            LeftHandObject.GetComponent<ItemInfo>().durability = data.leftHandDurability;
            LeftHandObject.SetActive(true);
        }

        if (data.isRightHandBag && RightHandObject.TryGetComponent(out Inventory rightBag))
        {
            for (int i = 0; i < rightBag.inventoryItems.Count; i++)
            {
                rightBag.inventoryItems[i] = ItemById(data.rightHandItemID[i]);
                if (rightBag.inventoryItems[i] != null)
                rightBag.inventoryItemObjects[i] = Instantiate(rightBag.inventoryItems[i].Prefab);
            }
        }

        if (data.isLeftHandBag && LeftHandObject.TryGetComponent(out Inventory leftBag))
        {
            for (int i = 0; i < leftBag.inventoryItems.Count; i++)
            {
                leftBag.inventoryItems[i] = ItemById(data.leftHandItemID[i]);
                if (leftBag.inventoryItems[i] != null)
                    leftBag.inventoryItemObjects[i] = Instantiate(leftBag.inventoryItems[i].Prefab);
            }
        }

        Redraw();

        StartCoroutine(LoadDelay(data));
    }
    
    IEnumerator LoadDelay(PlayerData data)
    {
        yield return null;
        if (inventory != null)
        {
            for (int i = 0; i < inventory.inventoryItems.Count; i++)
            {
                if (inventory.inventoryItemObjects[i] != null)
                {
                    inventory.inventoryItemObjects[i].GetComponent<ItemInfo>().durability = data.itemDurability[i];
                    inventory.inventoryItemObjects[i].SetActive(false);
                }
            }
        }

        if (data.isRightHandBag && RightHandObject.TryGetComponent(out Inventory rightBag))
        {
            for (int i = 0; i < rightBag.inventoryItems.Count; i++)
            {
                if (rightBag.inventoryItemObjects[i] != null)
                {

                    rightBag.inventoryItemObjects[i].GetComponent<ItemInfo>().durability = data.rightHandItemsDurability[i];
                    rightBag.inventoryItemObjects[i].SetActive(false);
                }
            }
        }

        if (data.isLeftHandBag && LeftHandObject.TryGetComponent(out Inventory leftBag))
        {
            for (int i = 0; i < leftBag.inventoryItems.Count; i++)
            {
                if (leftBag.inventoryItemObjects[i] != null)
                {
                    leftBag.inventoryItemObjects[i].GetComponent<ItemInfo>().durability = data.leftHandItemsDurability[i];
                    leftBag.inventoryItemObjects[i].SetActive(false);
                }
            }
        }

        for (int i = 0; i < Clothes.Count; i++)
        {
            if (Clothes[i] != null)
            {
                Clothes[i].GetComponent<ItemInfo>().durability = data.clothesDurability[i];
                Clothes[i].SetActive(true);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) { Redraw(); links.storageWindow.Redraw(); Debug.Log("Redraw"); }

    }

    public void Redraw()
    {
        ClearDrawn();
        links.questWindow.QuestItemsRecount();
        UpdateClothes();
        RecountWood();

        if (RightHandObject != null && RightHandObject.TryGetComponent(out Weapon weapon))
        { 
            rightHandWeapon = weapon; 
            weapon.isRightHand = true; 
        }
        else rightHandWeapon = null;

        if (LeftHandObject != null && LeftHandObject.TryGetComponent(out Weapon weapon2))
        {
            leftHandWeapon = weapon2;
            weapon2.isRightHand = false;
        }
        else leftHandWeapon = null;

        if (Backpack != null)
        {
            inventory = Backpack.GetComponent<Inventory>();
            if (!links.saveInventory.bags.Contains(inventory))
            {
                links.saveInventory.bags.Add(inventory);
                links.saveInventory.AddNewSaveBag(inventory);
            }
        }
        else inventory = null;

        int slotsCount = 0;
        if (inventory != null) slotsCount = inventory.size;
        
        for (int i = 0; i < 16; i++)
        {
            if (i < slotsCount) { slots[i].gameObject.SetActive(true); }
            else slots[i].gameObject.SetActive(false);
        }

        if (inventory != null)
            for (int i=0; i < inventory.inventoryItems.Count; i++)
            {
                if (inventory.inventoryItems[i] != null)
                {
                    Item item = inventory.inventoryItems[i];
                    DrawIcon(item, i);
                }
            }
        if (LeftHandItem != null) DrawIcon(LeftHandItem, 100);
        if (RightHandItem != null) DrawIcon(RightHandItem, 101);
        if (BackpackItem != null) DrawIcon(BackpackItem, 102);
        if (BeltItem != null) DrawIcon(BeltItem, 103);
        if (FeetItem != null) DrawIcon(FeetItem, 104);
        if (LegsItem != null) { DrawIcon(LegsItem, 105);}
        if (ArmsItem != null) DrawIcon(ArmsItem, 106);
        if (BodyItem != null) DrawIcon(BodyItem, 107);
        if (ShouldersItem != null) DrawIcon(ShouldersItem, 108);
        if (HeadItem != null) DrawIcon(HeadItem, 109);

        if (inventory != null) inventory.Recount();
    }

    void DrawIcon(Item item, int i)
    {
        RectTransform slot = null;

        if (i < 100) slot = slots[i];
        else if (i == 100) slot = leftHandSlot;
        else if (i == 101) slot = rightHandSlot;
        else if (i == 102) slot = backpackSlot;
        else if (i == 103) slot = beltSlot;
        else if (i == 104) slot = feetSlot;
        else if (i == 105) slot = legsSlot;
        else if (i == 106) slot = armsSlot;
        else if (i == 107) slot = bodySlot;
        else if (i == 108) slot = shouldersSlot;
        else if (i == 109) slot = headSlot;

        GameObject iconGameObject = Instantiate(inventoryCellTemplate, slot);
        InventoryIcon iconScript = iconGameObject.GetComponent<InventoryIcon>();

        iconGameObject.name = item.Name + " Icon";
        iconGameObject.GetComponent<Image>().sprite = item.Icon;

        iconScript.Name.text = item.Name;
        iconScript.index = i;
        iconScript.Prefab = item.Prefab;
        iconScript.Init(draggingParent);
        iconScript.links = links;

        if (i < 100 && iconScript.storage == null)
        {
            iconScript.isInInventory = true;
            iconScript.isEquiped = false;
        }
        else if (i == 100)
        {
            iconScript.isInLeftHand = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 101) 
        {
            iconScript.isInRightHand = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 102) 
        {
            iconScript.isInBagSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 103) 
        {
            iconScript.isInBeltSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 104) 
        {
            iconScript.isInFeetSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 105) 
        {
            iconScript.isInLegsSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 106) 
        {
            iconScript.isInHandsSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 107)
        {
            iconScript.isInBodySlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 108)
        {
            iconScript.isInShouldersSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else if (i == 109)
        {
            iconScript.isInHeadSlot = true;
            iconScript.isEquiped = true;
            iconScript.isInInventory = false;
        }
        else
        {
            iconScript.isEquiped = false;
            iconScript.isInInventory = false;
        }

        drawnIcons.Add(iconGameObject);

        iconScript.item = item;
        iconGameObject.GetComponent<DescribeUI>().mousePoint = links.mousePoint;
        iconGameObject.GetComponent<ItemInfo>().itemName = item.Name;
        iconGameObject.GetComponent<ItemInfo>().itemDescription = item.Description;
        iconGameObject.GetComponent<ItemInfo>().isCollectible = true;
        
        if (i == 100 && LeftHandObject != null)
        {
            iconGameObject.GetComponent<ItemInfo>().itemName = LeftHandObject.GetComponent<ItemInfo>().itemName;
            iconGameObject.GetComponent<ItemInfo>().itemDescription = LeftHandObject.GetComponent<ItemInfo>().itemDescription;
        }
        if (i == 101 && RightHandObject != null)
        {
            iconGameObject.GetComponent<ItemInfo>().itemName = RightHandObject.GetComponent<ItemInfo>().itemName;
            iconGameObject.GetComponent<ItemInfo>().itemDescription = RightHandObject.GetComponent<ItemInfo>().itemDescription;
        }

        iconGameObject.GetComponent<ItemInfo>().item = item;
        iconGameObject.GetComponent<ItemInfo>().type = item.Type;
        iconGameObject.GetComponent<ItemInfo>().weight = item.weight;
        iconGameObject.GetComponent<ItemInfo>().durability = item.durability;
    }

    void ClearDrawn()
    {
        for (var i = 0; i < drawnIcons.Count; i++)
        {
            if (drawnIcons[i] != null)
            Destroy(drawnIcons[i]);
        }
        drawnIcons.Clear();
    }

    public void RecountWood()
    {
        int woodAmount = 0;
        if (RightHandItem != null && RightHandItem.Type == ItemType.Wood)
        {
            woodAmount++;
        }
        if (LeftHandItem != null && LeftHandItem.Type == ItemType.Wood)
        {
            woodAmount++;
        }
        if (links.inventoryWindow.inventory != null)
            for (int i = 0; i < links.inventoryWindow.inventory.inventoryItems.Count; i++)
            {
                if (links.inventoryWindow.inventory.inventoryItems[i] != null)
                    if (links.inventoryWindow.inventory.inventoryItems[i].Type == ItemType.Wood)
                    {
                        woodAmount++;
                    }
            }
        links.player.Wood = woodAmount;
    }

    public void UpdateClothes()
    {
        Clothes[0] = Belt; ClothesItems[0] = BeltItem;
        Clothes[1] = Feet; ClothesItems[1] = FeetItem;
        Clothes[2] = Legs; ClothesItems[2] = LegsItem;
        Clothes[3] = Arms; ClothesItems[3] = ArmsItem;
        Clothes[4] = Body; ClothesItems[4] = BodyItem;
        Clothes[5] = Shoulders; ClothesItems[5] = ShouldersItem;
        Clothes[6] = Head; ClothesItems[6] = HeadItem;

        UpdateClothesWarm();
        UpdateClothesVisual();
        UpdateClothesArmor();
    }

    public void UpdateClothesWarm()
    {
        links.player.clothesTemperature = 0;
        for (int i = 0; i < Clothes.Count; i++)
        {
            if (ClothesItems[i] != null) links.player.clothesTemperature += ClothesItems[i].warmBonus
                    * Clothes[i].GetComponent<ItemInfo>().durability * 0.01f;
        }
    }

    float _armor;
    public void UpdateClothesArmor()
    {
        _armor = 0;
        existClothes.Clear();
        for (int i = 0; i < Clothes.Count; i++)
        {
            if (ClothesItems[i] != null)
            {
                _armor += ClothesItems[i].armor
                      * Clothes[i].GetComponent<ItemInfo>().durability * 0.01f;
                existClothes.Add(Clothes[i]);
            }
        }
        links.player.armor = (int)Mathf.Round(_armor);
        links.ui.armorIndicator.text = _armor.ToString("0.0");
    }

    public List<GameObject> existClothes;

    public void ArmorRandomDamage()
    {
        if (existClothes.Count > 0)
        {
            existClothes[Random.Range(0, existClothes.Count)].GetComponent<ItemInfo>().durability -= Random.Range(1, 10);
            UpdateClothesArmor();
        }
    }
    
    public void WeaponRandomDamage(Weapon weapon)
    {
        weapon.GetComponent<ItemInfo>().durability -= Random.Range(0, 3);
        Debug.LogError("weapon damage");
        if (weapon.GetComponent<ItemInfo>().durability <= 0)
        {
            weapon.gameObject.SetActive(false);
            if (weapon.isRightHand)
            {
                rightHandWeapon = null;
                if (rightHandSlot.childCount > 0) Destroy(rightHandSlot.GetChild(0).gameObject);
                Destroy(RightHandObject);
            }
            else
            {
                leftHandWeapon = null;
                if (leftHandSlot.childCount > 0) Destroy(leftHandSlot.GetChild(0).gameObject);
                Destroy(LeftHandObject);
            }
            links.mousePoint.Comment("Оружие сломалось!");
        }
    }

    public void UpdateClothesVisual()
    {
        // Ремень
        if (ClothesItems[0] != null && ClothesItems[0].clothType == ClothType.Belt)
        {
            belt.SetActive(true);
        }
        else
        {
            belt.SetActive(false);
        }

        // Ботинки
        if (ClothesItems[1] != null && ClothesItems[1].clothType == ClothType.Boots)
        {
            boots.SetActive(true);
        }
        else
        {
            boots.SetActive(false);
        }

        // Штаны
        if (ClothesItems[2] != null && ClothesItems[2].clothType == ClothType.ShortPants)
        {
            playerLegs.SetActive(false);
            playerLowLegs.SetActive(true);
            shortPants.SetActive(true);
            underPants.SetActive(false);
        }
        else
        {
            playerLegs.SetActive(true);
            playerLowLegs.SetActive(false);
            shortPants.SetActive(false);
            underPants.SetActive(true);
        }

        // Перчатки
        if (ClothesItems[3] != null && ClothesItems[3].clothType == ClothType.Glowes)
        {
            glowes.SetActive(true);
        }
        else
        {
            glowes.SetActive(false);
        }

        // Броня
        if (ClothesItems[4] != null && ClothesItems[4].clothType == ClothType.LeatherPlate)
        {
            leatherPlate.SetActive(true);
            body.SetActive(false);
        }
        else
        {
            body.SetActive(true);
            leatherPlate.SetActive(false);
        }

        // Наплечники
        if (ClothesItems[5] != null && ClothesItems[5].clothType == ClothType.Shoulders)
        {
            shoulders.SetActive(true);
        }
        else
        {
            shoulders.SetActive(false);
        }

        // Шлем
        if (ClothesItems[6] != null && ClothesItems[6].clothType == ClothType.VikingHelmet)
        {
            vikingHelmet.SetActive(true);
        }
        else
        {
            vikingHelmet.SetActive(false);
        }
    }

    public GameObject playerLegs;
    public GameObject playerLowLegs;
    public GameObject shortPants;
    public GameObject underPants;
    public GameObject belt;
    public GameObject boots;
    public GameObject glowes;
    public GameObject leatherPlate;
    public GameObject body;
    public GameObject shoulders;
    public GameObject vikingHelmet;
}
public enum ClothType
{
    Belt = 0,
    Boots = 1,
    ShortPants = 2,
    Glowes = 3,
    LeatherPlate = 4,
    Shoulders = 5,
    VikingHelmet = 6
}

