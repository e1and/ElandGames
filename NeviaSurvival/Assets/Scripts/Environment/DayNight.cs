using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [SerializeField] TOD_Sky skyScript;
    public float hour;
    [SerializeField] GameObject sunLight;
    [SerializeField] GameObject moonLight;
    bool isDay;


    void Start()
    {
        if (hour > 4 && hour < 20)
        {
            isDay = true;
            sunLight.GetComponent<Light>().intensity = 1;
            moonLight.GetComponent<Light>().intensity = 0;
        }
        else
        {
            moonLight.GetComponent<Light>().intensity = 1;
            sunLight.GetComponent<Light>().intensity = 0;
        }
    }


    void Update()
    {
        hour = skyScript.hour;
        if (hour > 4 && hour < 20 && !isDay) StartCoroutine(StartDay());
        if (hour > 20 && isDay) StartCoroutine(StartNight());
    }

    IEnumerator StartDay()
    {
        isDay = true;
        moonLight.SetActive(true);
        moonLight.GetComponent<Light>().intensity = 1;
        while (hour < 20 && hour > 4 && moonLight.GetComponent<Light>().intensity > 0)
        {
            moonLight.GetComponent<Light>().intensity -= 0.001f;
            yield return null;
        }
        moonLight.SetActive(false);
        sunLight.SetActive(true);
        while (hour < 20 && hour > 4 && sunLight.GetComponent<Light>().intensity < 1)
        {
            sunLight.GetComponent<Light>().intensity += 0.001f;
            yield return null;
        }
    }

    IEnumerator StartNight()
    {
        isDay = false;
        sunLight.SetActive(true);
        sunLight.GetComponent<Light>().intensity = 1;
        while (hour > 20 && sunLight.GetComponent<Light>().intensity > 0)
        {
            sunLight.GetComponent<Light>().intensity -= 0.001f;
            yield return null;
        }
        sunLight.SetActive(false);
        moonLight.SetActive(true);
        while (hour > 20 && moonLight.GetComponent<Light>().intensity < 1)
        {
            moonLight.GetComponent<Light>().intensity += 0.001f;
            yield return null;
        }
    }
}
