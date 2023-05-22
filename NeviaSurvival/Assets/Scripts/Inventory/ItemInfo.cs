using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    public string itemDescription;
    public string itemComment;

    public int durability;
    public float weight;

    public bool isUnique;
    public bool isUsable;
    public bool isOpenable;
    public bool isCollectible;
    public bool isCarrying;
    public bool isFirePlace;
    public bool isTorch;
    public bool isBed;
    [Space]
    public bool isMovable;
    public Vector3 savePosition;
    public Quaternion saveRotation;
    public bool isSaveItemActive;
    [Space]

    public Vector3 carryPosition;
    public Quaternion carryRotation;
    [Space]
    [Space]
    [Space]

    public Item item;
    public ItemType type;


}
