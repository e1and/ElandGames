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
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip closedDoorSound;
    
    Links links;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        links = mousePoint.links;
        CheckSaveList();
        ConfigureDoor();
    }

    public void ConfigureDoor()
    {
        if (isOpen)
        {
            if (isOpenMirror) animator.SetTrigger("OpenedMirror");
            else animator.SetTrigger("Opened");
            animator.SetBool("Open", true);
        }
    }

    void CheckSaveList()
    {
        if (!links.saveObjects.doors.Contains(this))
            Debug.LogError($"����� { gameObject.name } �� ��������� � ������ ����������� ��������!");
    }

    public void OpenClose()
    {
        if (isDungeon)
        {
            if (isExit) ExitDoor(); 
            else EnterDoor();
            audioSource.PlayOneShot(closeSound);
            
        }
        else
        {
            if (!isLocked && !isBlocked)
            {
                isOpen = !isOpen;
                
                if (isOpen) audioSource.PlayOneShot(openSound);
                else audioSource.PlayOneShot(closeSound);

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
                audioSource.PlayOneShot(closedDoorSound);
                gameObject.GetComponent<ItemInfo>().itemComment = "��������� ��� �� ���� ������ � ��� �� �������?";
                return false;
            }
            else return true;
        }
        else return false;
    }

    public void PlayKeySound()
    {
        audioSource.PlayOneShot(links.sounds.keyOpenDoor[Random.Range(0, links.sounds.keyOpenDoor.Length)]);
    }

    public void StopSound()
    {
        audioSource.Stop();
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
