using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescribeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MousePoint mousePoint;
    public Links links;

    private void Start()
    {
        links = FindObjectOfType<Links>();
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
            if (TryGetComponent(out InventoryIcon icon))
            {
                mousePoint.itemDurabilityPanelText.enabled = true;
                mousePoint.itemWeightPanelText.enabled = true;


                mousePoint.itemNamePanelText.text = itemInfo.itemName;
                mousePoint.itemDescriptionPanelText.text = itemInfo.itemDescription;
                mousePoint.itemWeightPanelText.text = itemInfo.weight + "��";
                if (icon.item3dObject != null)
                {
                    mousePoint.itemDurabilityPanelText.text = icon.item3dObject.GetComponent<ItemInfo>().durability + "%";
                }
                else
                mousePoint.itemDurabilityPanelText.text = itemInfo.durability + "%";

                mousePoint.itemCommentPanelText.text = "";

                if (icon.item.isFood) mousePoint.itemActionPanelText.text = "E - ������";
                else if (icon.item.Type == ItemType.Torch)
                {
                    mousePoint.itemActionPanelText.text = "E - ������/��������";
                    if (icon.item3dObject.GetComponent<Torchlight>().burningTime == 0)
                        mousePoint.itemActionPanelText.text = "��������� ��������";
                }
                else if (icon.item.Type == ItemType.Scroll) mousePoint.itemActionPanelText.text = "E - ���������";
                else if (icon.item.Type == ItemType.Key) mousePoint.itemActionPanelText.text = "��� �� ����� (�����.)";
                else if (icon.item.Type == ItemType.Cauldron && icon.item3dObject.TryGetComponent(out Cauldron cauldron))
                {
                    if (links.player.isAbleToCollectWater && icon.TryGetComponent(out ItemInfo info))
                    {
                        if (!cauldron.isWater)
                        {
                            mousePoint.itemActionPanelText.text = "E - �������, Q - ���������";
                            info.itemName = cauldron.cauldronName;
                            info.itemDescription = "�������� �������. ��� ���� ����������� ����������.";
                            links.mousePoint.itemNamePanelText.text = info.itemName;
                            links.mousePoint.itemDescriptionPanelText.text = info.itemDescription;
                        }
                        else
                        {
                            mousePoint.itemActionPanelText.text = "E - �������, Q - ������";
                            info.itemName = cauldron.cauldronName + " " + cauldron.withWaterName;
                            info.itemDescription = "������ � �� ����� ���-������ �������!";
                            links.mousePoint.itemNamePanelText.text = info.itemName;
                            links.mousePoint.itemDescriptionPanelText.text = info.itemDescription;
                        }
                    }
                    else if (!cauldron.isWater) mousePoint.itemActionPanelText.text = "E - �������";
                    else mousePoint.itemActionPanelText.text = "E - �������, Q - ������";
                }
                else mousePoint.itemActionPanelText.text = itemInfo.itemComment;

                mousePoint.pointedIcon = itemInfo;

            }
            else
            {
                mousePoint.itemDurabilityPanelText.enabled = false;
                mousePoint.itemWeightPanelText.enabled = false;

                mousePoint.itemNamePanelText.text = itemInfo.itemName;
                mousePoint.itemDescriptionPanelText.text = itemInfo.itemDescription;
                mousePoint.itemActionPanelText.text = itemInfo.itemComment;
                mousePoint.pointedIcon = itemInfo;
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
