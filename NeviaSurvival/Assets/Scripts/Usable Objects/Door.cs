using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public bool isBlocked;
    public bool isOpen;
    public bool isOpenMirror;
    public bool isDungeon;
    public bool isExit;
    public GameObject entrancePoint;
    Animator animator;
    public InventoryWindow inventoryWindow;
    public Item key;
    public MousePoint mousePoint;
    public float openingTime;
    public string doorInfo;
    public MusicZone musicZone;
    Links links;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        links = mousePoint.links;
    }

    public void OpenClose()
    {
        if (isDungeon)
        {
            if (isExit) ExitDoor(); 
            else EnterDoor();
            
        }
        else
        {
            if (!isLocked && !isBlocked)
            {
                isOpen = !isOpen;

                if (isOpenMirror) animator.SetBool("OpenMirror", !animator.GetBool("OpenMirror"));
                else animator.SetBool("Open", !animator.GetBool("Open"));
            }
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
                        if (isLocked) mousePoint.Comment("���-�� � ����� ��� ����!");
                        return false;
                    }
                }
            }
            if (inventoryWindow.LeftHandItem == key || inventoryWindow.RightHandItem == key)
            {
                if (isLocked)
                {
                    mousePoint.Comment("���� �������!");
                    if (inventoryWindow.LeftHandItem == key)
                        inventoryWindow.LeftHandObject.GetComponent<ItemInfo>().itemDescription = "���� ������ " + doorInfo;
                    else inventoryWindow.RightHandObject.GetComponent<ItemInfo>().itemDescription = "���� ������ " + doorInfo;
                    gameObject.GetComponent<ItemInfo>().itemComment = doorInfo;
                    inventoryWindow.Redraw();
                }
                return true;
            }

            if (isLocked)
            {
                mousePoint.Comment("����� �������. ����� ����!");
                gameObject.GetComponent<ItemInfo>().itemComment = "��������� ��� �� ���� ������ � ��� �� �������?";
                return false;
            }
            else return true;
        }
        else return false;
    }

    public void EnterDoor()
    {
        links.player.PlayerControl(false);
        mousePoint.Player.transform.position = entrancePoint.transform.position;
        links.dayNight.isDungeon = true;
        links.dayNight.Dungeon();
        links.player.PlayerControl(true);
    }

    public void ExitDoor()
    {
        links.player.PlayerControl(false); 
        mousePoint.Player.transform.position = entrancePoint.transform.position;
        if (musicZone != null) musicZone.ExitZone();
        links.dayNight.isDungeon = false;
        links.dayNight.SetDaySettings();
        links.player.PlayerControl(true);

    }
}
