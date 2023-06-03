using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomElements : MonoBehaviour
{
    public List<GameObject> elements;

    void Start()
    {
        foreach (GameObject element in elements)
        {
            if (element.TryGetComponent(out ElementVariants elementVariants))
            {
                elementVariants.variants[Random.Range(0, elementVariants.variants.Count)].SetActive(false);
            }
            else if (Random.Range(0, 2) < 1) element.SetActive(false); 
        }
    }
}
