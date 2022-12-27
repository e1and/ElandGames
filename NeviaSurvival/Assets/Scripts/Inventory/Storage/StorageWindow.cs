using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageWindow : MonoBehaviour
{
    public Storage targetStorage;
    public Player Player;
    public RectTransform[] slots;
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryWindow inventoryWindow;
    [SerializeField] GameObject inventoryCellTemplate;
    [SerializeField] private Transform _rightHandParent;
    [SerializeField] private Transform _leftHandParent;
    [SerializeField] private Transform _draggingParent;
    [SerializeField] private Transform _dropParent;
    [SerializeField] Sprite emptySlotImage;
    [SerializeField] TMP_Text storageTitle;

    [SerializeField] Item itemToAdd;

    public List<GameObject> drawnIcons = new List<GameObject>();

    private void Awake()
    {
        targetStorage.onStorageItemAdded += OnItemAdded;
        targetStorage.onOpenStorage += RedrawStorage;
        RedrawStorage();
        gameObject.SetActive(false);
    }

    void OnItemAdded(Item obj) => RedrawStorage();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) targetStorage.AddItem(itemToAdd);
    }

    public void RedrawStorage()
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
                Debug.Log("Redraw Storage");
                Item item = targetStorage.storageItems[i];
                var icon = Instantiate(inventoryCellTemplate, slots[i]);
                icon.name = item.Name + " Icon";
                icon.GetComponent<InventoryCell>().Name.text = item.Name;
                icon.GetComponent<InventoryCell>().index = i;
                icon.GetComponent<InventoryCell>().Prefab = item.Prefab;
                icon.GetComponent<InventoryCell>().dropParent = _dropParent;
                icon.GetComponent<Image>().sprite = item.Icon;
                icon.GetComponent<InventoryCell>().Init(_draggingParent);
                icon.GetComponent<InventoryCell>().inventory = inventory;
                icon.GetComponent<InventoryCell>().inventoryWindow = inventoryWindow;
                icon.GetComponent<InventoryCell>().storageWindow = this;
                icon.GetComponent<InventoryCell>().storage = targetStorage;
                icon.GetComponent<InventoryCell>()._rightHandParent = _rightHandParent;
                icon.GetComponent<InventoryCell>()._leftHandParent = _leftHandParent;
                drawnIcons.Add(icon);
            }
        }

        targetStorage.Recount();
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
