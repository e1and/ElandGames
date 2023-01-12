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

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void OpenClose()
    {
        if (key != null)
        {
            for (int i = 0; i < inventory.inventoryItems.Count; i++)
            {
                if (inventory.inventoryItems[i] != null && inventory.inventoryItems[i] == key)
                {
                    isLocked = false;
                    mousePoint.Comment("Ключ подошёл!");
                }
            }
        }
        if (!isLocked)
        {
            isOpen = !isOpen;

            animator.SetBool("Open", !animator.GetBool("Open"));
        }

    }
}
