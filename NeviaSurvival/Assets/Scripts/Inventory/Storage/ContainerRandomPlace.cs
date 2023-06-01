using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerRandomPlace : MonoBehaviour
{
    public List<Storage> storages;
    public List<Transform> places;
    public List<int> placeNumbers;

    int randomIndex;

    void Start()
    {
        randomIndex = Random.Range(0, places.Count);

        foreach (Storage storage in storages)
        {
            if (storages.Count > places.Count)
            {
                Debug.Log("Can't ContainerRandomPlace - need more PLACES!");
                break;
            }

            while (placeNumbers.Contains(randomIndex))
            {
                randomIndex = Random.Range(0, places.Count);
            }
            
            storage.transform.position = places[randomIndex].position;
            storage.transform.rotation = places[randomIndex].rotation;
            placeNumbers.Add(randomIndex);
        }
    }
}
