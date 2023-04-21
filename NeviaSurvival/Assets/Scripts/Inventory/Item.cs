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
    [Header("��������� ��������:")]
    public float weight;
    [Header("��������� �� ��� �������:")]
    public bool isFood;
    public int foodValue;
    [Header("��� � ��������� ������:")]
    public float warmBonus;
    public ClothType clothType;
}
