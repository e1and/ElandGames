using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public bool isOpen;
    Animator animator;
    public Inventory inventory;
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

            animator.SetBool("Open", !animator.GetBool("Open"));
        }
    }

    public bool SearchingKey()
    {
        if (key != null)
        {
            for (int i = 0; i < inventory.inventoryItems.Count; i++)
            {
                if (inventory.inventoryItems[i] != null && inventory.inventoryItems[i] == key)
                {
                    if (isLocked) mousePoint.Comment("Ключ подошёл!");
                    return true;
                }
            }
            if (isLocked)
            {
                mousePoint.Comment("Дверь заперта!");
                return false;
            }
            else return false;
        }
        else return false;
    }
}
