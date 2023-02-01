using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] MousePoint mousePoint;
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
                Item item = targetInventory.inventoryItems[i];
                DrawIcon(item, i);
            }
        }
        if (targetInventory.LeftHand != null) DrawIcon(targetInventory.LeftHand, 100);
        if (targetInventory.RightHand != null) DrawIcon(targetInventory.RightHand, 101);

        targetInventory.Recount();
    }

    void DrawIcon(Item item, int i)
    {
        RectTransform slot = null;

        if (i < 100) slot = slots[i];
        else if (i == 100) slot = leftHandSlot;
        else if (i == 101) slot = rightHandSlot;

        GameObject iconGameObject = Instantiate(inventoryCellTemplate, slot);
        InventoryCell iconScript = iconGameObject.GetComponent<InventoryCell>();

        iconGameObject.name = item.Name + " Icon";
        iconScript.Name.text = item.Name;
        iconScript.index = i;
        iconScript.Prefab = item.Prefab;
        iconScript.dropParent = _dropParent;
        iconGameObject.GetComponent<Image>().sprite = item.Icon;
        iconScript.Init(_draggingParent);
        iconScript.inventory = targetInventory;
        iconScript.inventoryWindow = this;
        iconScript.storageWindow = storageWindow;

        iconScript._rightHandParent = _rightHandParent;
        iconScript._leftHandParent = _leftHandParent;

        if (i < 100) iconScript.isInInventory = true;
        else if (i == 100) iconScript.isInLeftHand = true;
        else if (i == 101) iconScript.isInRightHand = true;

        drawnIcons.Add(iconGameObject);

        iconScript.item = item;
        iconGameObject.GetComponent<DescribeUI>().mousePoint = mousePoint;
        iconGameObject.GetComponent<ItemInfo>().itemName = item.Name;
        iconGameObject.GetComponent<ItemInfo>().itemDescription = item.Description;
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
}
