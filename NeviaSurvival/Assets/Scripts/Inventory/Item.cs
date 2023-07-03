using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string id;
    public string Description;
    public ItemType Type;
    public Sprite Icon;
    public GameObject Prefab;
    [Header("Параметры предмета:")]
    public float rarity;
    public float weight;
    public int durability;
    [Header("Съедобный ли это предмет:")]
    public bool isFood;
    public int foodValue;
    public int poisonValue;
    public bool isLiquid;
    [Header("Вид и параметры одежды:")]
    public float warmBonus;
    public int armor;
    public ClothType clothType;
}
