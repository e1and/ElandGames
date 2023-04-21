using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public bool isOpen;
    public bool isOpenMirror;
    Animator animator;
    public InventoryWindow inventoryWindow;
    public Item key;
    public MousePoint mousePoint;
    public float openingTime;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void OpenClose()
    {
        if (!isLocked)
        {
            isOpen = !isOpen;

            if (isOpenMirror) animator.SetBool("OpenMirror", !animator.GetBool("OpenMirror"));
            else animator.SetBool("Open", !animator.GetBool("Open"));
        }
    }

    public bool SearchingKey()
    {
        if (key != null)
        {
            if (inventoryWindow.inventory != null)
            {
                for (int i = 0; i < inventoryWindow.inventory.inventoryItems.Count; i++)
                {
                    if (inventoryWindow.inventory.inventoryItems[i] != null && inventoryWindow.inventory.inventoryItems[i] == key)
                    {
                        if (isLocked) mousePoint.Comment("√де-то в сумке был ключ!");
                        return false;
                    }
                }
            }
            if (inventoryWindow.LeftHandItem == key || inventoryWindow.RightHandItem == key)
            {
                if (isLocked) mousePoint.Comment(" люч подошЄл!");
                return true;
            }

            if (isLocked)
            {
                mousePoint.Comment("ƒверь заперта!");
                return false;
            }
            else return true;
        }
        else return false;
    }
}
