using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObjects : MonoBehaviour
{
    public bool isConfigure;
    
    public List<Door> doors;
    public List<Lamp> lamps;
    public List<Storage> storages;

    public List<RandomElements> randomElements;
    public List<Destructable> destructableItems;
    public Transform existWorldItems;
    public Transform allWorldItems;
    public Transform playerBuildings;

    private void Start()
    {
        if (isConfigure)
        {
            var destructables = FindObjectsOfType<Destructable>();
            foreach (var item in destructables)
            {
                if (!destructableItems.Contains(item))
                    destructableItems.Add(item);
            }
        }
    }
}
