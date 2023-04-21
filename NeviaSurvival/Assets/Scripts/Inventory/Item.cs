using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Description;
    public ItemType Type;
    public Sprite Icon;
    public GameObject Prefab;
    [Header("Параметры предмета:")]
    public float weight;
    [Header("Съедобный ли это предмет:")]
    public bool isFood;
    public int foodValue;
    [Header("Вид и параметры одежды:")]
    public float warmBonus;
    public ClothType clothType;
}
