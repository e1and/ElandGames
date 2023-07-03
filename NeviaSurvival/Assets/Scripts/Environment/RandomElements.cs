using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomElements : MonoBehaviour
{
    public List<GameObject> elements;
    GameObject currentElement;

    void Start()
    {
        foreach (GameObject element in elements)
        {
            if (element.TryGetComponent(out ElementVariants elementVariants))
            {
                currentElement = elementVariants.variants[Random.Range(0, elementVariants.variants.Count)];
                if (currentElement.activeSelf) currentElement.SetActive(false);
                else currentElement.SetActive(true);
            }
            else if (Random.Range(0, 2) < 1)
            {
                if (element.activeSelf) element.SetActive(false);
                else element.SetActive(true);
            }
        }
    }
}
