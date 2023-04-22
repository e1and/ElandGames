using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    Links links;
    public float TimeToSpawn = 24;
    public List<Item> items = new List<Item>();
    public List<GameObject> trees = new List<GameObject>();

    [Header("Уникальные предметы лута")]
    public List<Item> uniqueItems_loot = new List<Item>();

    [Header("Группы хранилищ по локациям")]
    public List<Transform> locations = new List<Transform>();

    [Header("Уникальные предметы для разных локаций")]
    public List<Item> uniqueItems_1lvl = new List<Item>();
    public List<Item> uniqueItems_2lvl = new List<Item>();
    public List<Item> uniqueItems_3lvl = new List<Item>();

    [Header("Спаунпоинты уникальных предметов для разных локаций")]
    public List<GameObject> spawnPoints_1lvl = new List<GameObject>();
    public List<GameObject> spawnPoints_2lvl = new List<GameObject>();
    public List<GameObject> spawnPoints_3lvl = new List<GameObject>();

    public List<GameObject> closedSpawnPoints = new List<GameObject>();
    public Transform closedSpawnPointParent;

    List<List<Item>> keys = new List<List<Item>>();
    List<List<GameObject>> spawnPoints = new List<List<GameObject>>();
    List<GameObject> allSpawnPoints = new List<GameObject>();

    private void Awake()
    {
        links = FindObjectOfType<Links>();
        keys.Add(uniqueItems_1lvl); keys.Add(uniqueItems_2lvl); keys.Add(uniqueItems_3lvl);
        spawnPoints.Add(spawnPoints_1lvl); spawnPoints.Add(spawnPoints_2lvl); spawnPoints.Add(spawnPoints_3lvl);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            allSpawnPoints.AddRange(spawnPoints[i]);
        }
        for (int i = 0; i < closedSpawnPointParent.childCount; i++)
        {
            allSpawnPoints.Add(closedSpawnPointParent.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        SpawnItemAroundTree();
    }

    public void SpawnItemAroundTree()
    {
        for (int i = 0; i < trees.Count; i++)
        {
            int randomItem = Random.Range(0, items.Count);
            GameObject newItem = Instantiate(items[randomItem].Prefab, links.spawnItemsParent);
            newItem.transform.Rotate(0, Random.Range(0, 360), 0);
            newItem.transform.position = trees[i].transform.position + newItem.transform.forward * Random.Range(4f, 6f) + newItem.transform.up * 2;
        }
    }

    public void UniqueItemsRandomizer()
    {
        // Рандомное расположение ключей по нужным локациям и незакрытым местам
        for (int i = 0; i < keys.Count; i++)
        {
            for (int y = 0; y < keys[i].Count; y++)
            {
                Debug.Log(i + " " + y + " " + keys[i][y]);
                int randomPlace = Random.Range(0, spawnPoints[i].Count);

                if (spawnPoints[i][randomPlace].TryGetComponent(out Storage storage))
                    storage.AddItem(keys[i][y]);
                else
                {
                    Debug.Log(keys[i][y].Prefab);
                    GameObject item = Instantiate(keys[i][y].Prefab, links.spawnItemsParent);
                    item.transform.position = spawnPoints[i][randomPlace].transform.position;
                }
            }
        }

        for (int i = 0; i < links.itemRandomizer.storages.Count; i++)
        {
            allSpawnPoints.Add(links.itemRandomizer.storages[i].gameObject);
        }
        
        // Рандомное расположение уникальных предметов по всем хранилищам и локациям
        for (int i = 0; i < uniqueItems_loot.Count; i++)
        {
            int randomPlace = Random.Range(0, allSpawnPoints.Count);

            if (allSpawnPoints[randomPlace].TryGetComponent(out Storage storage))
                storage.AddItem(uniqueItems_loot[i]);
            else
            {
                Debug.Log(uniqueItems_loot[i].Prefab);
                GameObject item = Instantiate(uniqueItems_loot[i].Prefab, links.spawnItemsParent);
                item.transform.position = allSpawnPoints[randomPlace].transform.position;
            }
        }
    }

    float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime * links.time.timeFactor / 3600;
        if (spawnTimer > TimeToSpawn) { SpawnItemAroundTree(); spawnTimer = 0; }
    }
}
