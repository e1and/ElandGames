using System.Collections;
using System.Collections.Generic;
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
    Axe
}

public class InventoryWindow : MonoBehaviour
{
    public Inventory inventory;
    public RectTransform[] slots;
    public GameObject inventoryCellTemplate;
    [SerializeField] Sprite emptySlotImage;

    [Header("3д паренты для позиционпирования вещей на персонаже")]
    public Transform rightHandParent;
    public Transform leftHandParent;
    public Transform backpackParent;
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

    public List<GameObject> Clothes = new List<GameObject>(7);
    public List<Item> ClothesItems = new List<Item>(7);

    [Space]
    //public Player player;

    [SerializeField] Item itemToAdd;

    public List<GameObject> drawnIcons = new List<GameObject>();

    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();

        if (Backpack != null) inventory = Backpack.GetComponent<Inventory>();
        else inventory = null;
    }
    private void Start()
    {
        if (inventory != null) inventory.onItemAdded += OnItemAdded;
        Redraw();
        gameObject.SetActive(false);
        links.ui.equipmentPanel.SetActive(false);

    }

    void OnItemAdded(Item obj) => Redraw();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) { Redraw(); links.storageWindow.Redraw(); Debug.Log("Redraw"); }

    }

    public void Redraw()
    {
        ClearDrawn();
        links.questWindow.QuestItemsReCount();
        UpdateClothes();
        RecountWood();

        if (Backpack != null)
        {
            inventory = Backpack.GetComponent<Inventory>();
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

        if (i < 100) iconScript.isInInventory = true;
        else if (i == 100) iconScript.isInLeftHand = true;
        else if (i == 101) iconScript.isInRightHand = true;
        else if (i == 102) iconScript.isInBagSlot = true;
        else if (i == 103) iconScript.isInBeltSlot = true;
        else if (i == 104) iconScript.isInFeetSlot = true;
        else if (i == 105) iconScript.isInLegsSlot = true;
        else if (i == 106) iconScript.isInHandsSlot = true;
        else if (i == 107) iconScript.isInBodySlot = true;
        else if (i == 108) iconScript.isInShouldersSlot = true;
        else if (i == 109) iconScript.isInHeadSlot = true;

        drawnIcons.Add(iconGameObject);

        iconScript.item = item;
        iconGameObject.GetComponent<DescribeUI>().mousePoint = links.mousePoint;
        iconGameObject.GetComponent<ItemInfo>().itemName = item.Name;
        iconGameObject.GetComponent<ItemInfo>().itemDescription = item.Description;

        iconGameObject.GetComponent<ItemInfo>().item = item;
        iconGameObject.GetComponent<ItemInfo>().type = item.Type;
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

        links.player.clothesTemperature = 0;
        for (int i = 0; i < Clothes.Count; i++)
        {
            if (ClothesItems[i] != null) links.player.clothesTemperature += ClothesItems[i].warmBonus
                    * Clothes[i].GetComponent<ItemInfo>().durability * 0.01f;
        }
        UpdateClothesVisual();
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

