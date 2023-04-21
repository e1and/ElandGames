using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRandomizer : MonoBehaviour
{
    Links links;

    public List<Storage> storages = new List<Storage>();
    public List<Item> randomItems = new List<Item>();

    public int maxItems = 2;

    Storage[] allStorages;

    void Start()
    {
        links = FindObjectOfType<Links>();

        allStorages = links.containersParent.GetComponentsInChildren<Storage>();
        storages.AddRange(allStorages);

        for (int i = 0; i < links.containersParent.childCount; i++)
        {
            if (links.containersParent.GetChild(i).TryGetComponent(out Storage storage))
                storages.Add(storage);
        }
        StorageItemRandomizer();
    }

    public void StorageItemRandomizer()
    {
        for (int i = 0; i < storages.Count; i++)
        {
            int itemsCount = Random.Range(0, maxItems);
            for (int j = 0; j <= itemsCount; j++)
                storages[i].AddItem(randomItems[Random.Range(0, randomItems.Count)]);
        }
        links.itemSpawner.UniqueItemsRandomizer();
    }
}
