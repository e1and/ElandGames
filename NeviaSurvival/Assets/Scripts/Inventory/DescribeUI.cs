using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescribeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MousePoint mousePoint;

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
            if (TryGetComponent(out InventoryIcon icon))
            {
                Debug.Log("111");
                mousePoint.itemNamePanelText.text = GetComponent<InventoryIcon>().item.Name;
                mousePoint.itemDescriptionPanelText.text = GetComponent<InventoryIcon>().item.Description;
                mousePoint.itemCommentPanelText.text = "";

                if (icon.item.isFood) mousePoint.itemActionPanelText.text = "E - съесть";
                else if (icon.item.Type == ItemType.Torch) mousePoint.itemActionPanelText.text = "E - зажечь/потушить";
                else if (icon.item.Type == ItemType.Scroll) mousePoint.itemActionPanelText.text = "E - прочитать";
                else if (icon.item.Type == ItemType.Key) mousePoint.itemActionPanelText.text = "ЛКМ на двери (удерж.)";
                else mousePoint.itemActionPanelText.text = "";

                mousePoint.pointedIcon = itemInfo;

            }
            else
            {
                mousePoint.itemNamePanelText.text = itemInfo.itemName;
                mousePoint.itemDescriptionPanelText.text = itemInfo.itemDescription;
                mousePoint.itemCommentPanelText.text = itemInfo.itemComment;
                mousePoint.pointedIcon = itemInfo;
                mousePoint.itemActionPanelText.text = "";
            }
        }
        else
        {
            mousePoint.isUIDescription = false;
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        mousePoint.isPointUI = false;
        mousePoint.isUIDescription = false;
        mousePoint.itemInfoPanel.SetActive(false);
        mousePoint.pointedIcon = null;
    }

}
