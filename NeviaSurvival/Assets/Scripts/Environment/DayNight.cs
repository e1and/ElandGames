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
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        if (hour > 4 && hour < 20)
        {
            isDay = true;
            sunLight.GetComponent<Light>().intensity = 1.5f;
            moonLight.GetComponent<Light>().intensity = 0;
            RenderSettings.ambientSkyColor = Color.white;
            RenderSettings.ambientEquatorColor = Color.white;
            RenderSettings.ambientGroundColor = new Color(150, 150, 150, 1);
            //RenderSettings.ambientIntensity = 0;
        }
        else
        {
            moonLight.GetComponent<Light>().intensity = 1.5f;
            sunLight.GetComponent<Light>().intensity = 0;
            RenderSettings.ambientSkyColor = Color.black;
            RenderSettings.ambientEquatorColor = Color.black;
            RenderSettings.ambientGroundColor = Color.black;
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
        float time = 0;
        if (hour > 9) time = 1;
        while (hour < 18 && hour > 4 && sunLight.GetComponent<Light>().intensity < 1.5f)
        {
            sunLight.GetComponent<Light>().intensity += 0.0001f;
            RenderSettings.ambientSkyColor = Color.Lerp(Color.black, Color.white, time);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.black, Color.white, time);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.black, Color.white, time);
            time += 0.0001f;
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
        float time = 0;
        while (hour > 18 && moonLight.GetComponent<Light>().intensity < 1.5f)
        {
            moonLight.GetComponent<Light>().intensity += 0.0001f;
            RenderSettings.ambientSkyColor = Color.Lerp(Color.white, Color.black, time);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.white, Color.black, time);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.white, Color.black, time);
            time += 0.0001f;
            yield return null;
        }
    }
}
