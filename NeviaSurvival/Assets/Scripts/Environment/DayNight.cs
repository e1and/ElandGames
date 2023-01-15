using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [SerializeField] TOD_Sky skyScript;
    public float hour;
    [SerializeField] GameObject sunLight;
    [SerializeField] GameObject moonLight;
    bool isDay;
    [SerializeField] TMP_Text timeIndicator;
    TimeSpan time;


    void Start()
    {
        if (hour > 4 && hour < 20)
        {
            isDay = true;
            sunLight.GetComponent<Light>().intensity = 1.5f;
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
        time = TimeSpan.FromHours(hour);
        timeIndicator.text = time.Hours.ToString("00") + ":" + time.Minutes.ToString("00");

        if (hour > 4 && hour < 18 && !isDay) StartCoroutine(StartDay());
        if (hour > 18 && isDay) StartCoroutine(StartNight());
    }

    IEnumerator StartDay()
    {
        isDay = true;
        moonLight.SetActive(true);
        while (hour < 18 && hour > 4 && moonLight.GetComponent<Light>().intensity > 0)
        {
            moonLight.GetComponent<Light>().intensity -= 0.001f;
            yield return null;
        }
        moonLight.SetActive(false);
        sunLight.SetActive(true);
        while (hour < 18 && hour > 4 && sunLight.GetComponent<Light>().intensity < 1.5f)
        {
            sunLight.GetComponent<Light>().intensity += 0.001f;
            yield return null;
        }
    }

    IEnumerator StartNight()
    {
        isDay = false;
        sunLight.SetActive(true);
        while (hour > 18 && sunLight.GetComponent<Light>().intensity > 0)
        {
            sunLight.GetComponent<Light>().intensity -= 0.001f;
            yield return null;
        }
        sunLight.SetActive(false);
        moonLight.SetActive(true);
        while (hour > 18 && moonLight.GetComponent<Light>().intensity < 1)
        {
            moonLight.GetComponent<Light>().intensity += 0.001f;
            yield return null;
        }
    }
}
