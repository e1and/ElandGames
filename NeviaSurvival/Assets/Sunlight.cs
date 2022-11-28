using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunlight : MonoBehaviour
{
    [SerializeField] TOD_Sky skyScript;
    public float hour;
    

    void Start()
    {
        
    }


    void Update()
    {
        hour = skyScript.hour;
        if (hour > 21 && GetComponent<Light>().intensity > 0) GetComponent<Light>().intensity -= 0.001f;
        if (hour < 21 && hour > 5 && GetComponent<Light>().intensity < 1) GetComponent<Light>().intensity += 0.001f;
    }
}
