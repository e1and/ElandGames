using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour
{
    [Header("�����������")]
    public Item redMushroom;
    public Item rawPotato;

    [Header("����������")]
    public Item cookedMushroom;
    public Item burnedMushroom;
    public Item cookedPotato;
    public Item burnedPotato;
    public Item mushroomSoup;

    public List<Item> randomFood = new List<Item>();

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
