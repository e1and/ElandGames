using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] StorageWindow storageWindow;
    Storage storage;
    public RectTransform[] slots;
    [SerializeField] GameObject inventoryCellTemplate;
    [SerializeField] private Transform _draggingParent;
    public Transform _rightHandParent;
    public Transform _leftHandParent;
    [SerializeField] private Transform _dropParent;
    [SerializeField] Sprite emptySlotImage;

    public RectTransform rightHandSlot;
    public RectTransform leftHandSlot;



    [SerializeField] Item itemToAdd;

    public List<GameObject> drawnIcons = new List<GameObject>(); 

    private void Start()
    {
        targetInventory.onItemAdded += OnItemAdded;
        Redraw();
    }

    void OnItemAdded(Item obj) => Redraw();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) Redraw();
    }

    public void Redraw()
    {
        ClearDrawn();

        for (int i=0; i < targetInventory.inventoryItems.Count; i++)
        {
            if (targetInventory.inventoryItems[i] != null)
            {
                Debug.Log("Redraw");
                Item item = targetInventory.inventoryItems[i];
                var icon = Instantiate(inventoryCellTemplate, slots[i]);
                icon.name = item.Name + " Icon";
                icon.GetComponent<InventoryCell>().Name.text = item.Name;
                icon.GetComponent<InventoryCell>().index = i;
                icon.GetComponent<InventoryCell>().Prefab = item.Prefab;
                icon.GetComponent<InventoryCell>().dropParent = _dropParent;
                icon.GetComponent<Image>().sprite = item.Icon;
                icon.GetComponent<InventoryCell>().Init(_draggingParent);
                icon.GetComponent<InventoryCell>().inventory = targetInventory;
                icon.GetComponent<InventoryCell>().inventoryWindow = this;
                icon.GetComponent<InventoryCell>().storageWindow = storageWindow;

                icon.GetComponent<InventoryCell>()._rightHandParent = _rightHandParent;
                icon.GetComponent<InventoryCell>()._leftHandParent = _leftHandParent;
                icon.GetComponent<InventoryCell>().isInInventory = true; ;


                //icon.GetComponent<InventoryCell>().DropItem += () => Destroy(icon); 
                //icon.gameObject.transform.SetParent(itemsPanel);
                icon.GetComponent<InventoryCell>().Equip();
                drawnIcons.Add(icon);
            }
        }

        targetInventory.Recount();
    }

    void ClearDrawn()
    {
        for (var i = 0; i < drawnIcons.Count; i++)
        {
            if (drawnIcons[i] != null)
            if (!drawnIcons[i].GetComponent<InventoryCell>().isEquiped)
            Destroy(drawnIcons[i]);
        }
        drawnIcons.Clear();
    }
}
