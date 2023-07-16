using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageWindow : MonoBehaviour
{
    public Storage targetStorage;
    public RectTransform[] slots;
    [SerializeField] Sprite emptySlotImage;
    public TMP_Text storageTitle;

    [SerializeField] Item itemToAdd;

    public List<GameObject> drawnIcons = new List<GameObject>();
    [Space]

    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();     
        Redraw();
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    void OnOpenStorage()
    {
        targetStorage.onStorageItemAdded += OnItemAdded;
        targetStorage.onOpenStorage += Redraw;
    }

    void OnItemAdded(Item obj) => Redraw();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) targetStorage.AddItem(itemToAdd);
    }

    public void Redraw()
    {
        ClearDrawn();
        storageTitle.text = targetStorage.gameObject.GetComponent<ItemInfo>().itemName;

        for (int i = 0; i < 9; i++)
        {
            if (i < targetStorage.size) { slots[i].gameObject.SetActive(true); }
            else slots[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < targetStorage.storageItems.Count; i++)
        {
            if (targetStorage.storageItems[i] != null)
            {
                Item item = targetStorage.storageItems[i];
                var icon = Instantiate(links.inventoryWindow.inventoryCellTemplate, slots[i]);
                
                icon.name = item.Name + " Icon";
                icon.GetComponent<DescribeUI>().mousePoint = links.mousePoint;
                Debug.Log(icon.GetComponent<InventoryIcon>() + " & " + item);
                

                icon.GetComponent<InventoryIcon>().index = i;
                icon.GetComponent<InventoryIcon>().Prefab = item.Prefab;
                icon.GetComponent<Image>().sprite = item.Icon;
                icon.GetComponent<InventoryIcon>().Init(links.inventoryWindow.draggingParent);
                icon.GetComponent<InventoryIcon>().links = links;
                icon.GetComponent<InventoryIcon>().storage = targetStorage;
                
                drawnIcons.Add(icon);

                icon.GetComponent<InventoryIcon>().item = item;
                icon.GetComponent<ItemInfo>().item = item;
                icon.GetComponent<ItemInfo>().itemName = item.Name;
                if (targetStorage.storageItemObjects[i] != null)
                    icon.GetComponent<ItemInfo>().itemName = targetStorage.storageItemObjects[i].GetComponent<ItemInfo>().itemName;
                icon.GetComponent<ItemInfo>().itemDescription = item.Description;
                icon.GetComponent<InventoryIcon>().Name.text = item.Name;
                icon.GetComponent<ItemInfo>().isCollectible = true;
            }
        }

        targetStorage.Recount();
        if (targetStorage.isSmallCauldron) targetStorage.gameObject.GetComponent<Cauldron>().UpdateCauldron();
    }

    void ClearDrawn()
    {      
        for (var i = 0; i < drawnIcons.Count; i++)
        {
            if (drawnIcons[i] != null) Destroy(drawnIcons[i]);
        }
        drawnIcons.Clear();
    }
}
