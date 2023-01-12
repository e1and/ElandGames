using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescribeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public MousePoint mousePoint;

    void Start()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    { Debug.Log("Function UI");
    
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterUI();
    }
    public void EnterUI()
    {
        mousePoint.isPointUI = true;
        if (TryGetComponent(out ItemInfo itemInfo))
        {
            mousePoint.isUIDescription = true;
            if (TryGetComponent(out InventoryCell component))
            {
                mousePoint.itemNamePanelText.text = GetComponent<InventoryCell>().item.Name;
                mousePoint.itemDescriptionPanelText.text = GetComponent<InventoryCell>().item.Description;
            }
            else
            {
                mousePoint.itemNamePanelText.text = itemInfo.itemName;
                mousePoint.itemDescriptionPanelText.text = itemInfo.itemDescription;
            }
        }
        else mousePoint.isUIDescription = false;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (TryGetComponent(out InventoryWindow panel) || TryGetComponent(out StorageWindow storage))
        mousePoint.isPointUI = false;
        mousePoint.isUIDescription = false;
        mousePoint.itemInfoPanel.SetActive(false);
    }

}
