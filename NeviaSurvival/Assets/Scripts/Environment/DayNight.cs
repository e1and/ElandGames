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
    [SerializeField] bool isDay;
    [SerializeField] TMP_Text timeIndicator;
    TimeSpan time;
    public float temperature;
    public float dayTemperature = 25;
    public float nightTemperature = 0;
    public float coldTemperature = 17;
    public float thisDay = 1;
    [SerializeField] TMP_Text dayIndicator;


    void Start()
    {
        hour = skyScript.hour;

        dayIndicator.text = ""; 
        StartCoroutine(ThisDay());

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        if (hour > 4 && hour < 20)
        {
            Day();
        }
        else
        {
            Night();
        }
    }

    public void Day()
    {
        isDay = false;
        sunLight.SetActive(true);
        moonLight.SetActive(false);
        //sunLight.GetComponent<Light>().intensity = 1.5f;
        moonLight.GetComponent<Light>().intensity = 0;
        RenderSettings.ambientSkyColor = Color.white;
        RenderSettings.ambientEquatorColor = Color.white;
        RenderSettings.ambientGroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
    }

    public void Night()
    {
        isDay = true;
        sunLight.SetActive(false);
        moonLight.SetActive(true);
        //moonLight.GetComponent<Light>().intensity = 1.5f;
        sunLight.GetComponent<Light>().intensity = 0;
        RenderSettings.ambientSkyColor = Color.black;
        RenderSettings.ambientEquatorColor = Color.black;
        RenderSettings.ambientGroundColor = Color.black;
    }


    void Update()
    {
        hour = skyScript.hour;
        time = TimeSpan.FromHours(hour);
        timeIndicator.text = time.Hours.ToString("00") + ":" + time.Minutes.ToString("00");

        if (hour > 4 && hour < 18 && !isDay) { StartCoroutine(StartDay()); Debug.Log("Start Day"); }
        if (hour > 18 && isDay) { StartCoroutine(StartNight()); Debug.Log("Start Night"); }
    }

    public Color dayColor;
    IEnumerator ThisDay()
    {
        
        yield return new WaitForSeconds(3);
        
        dayColor = dayIndicator.color;
        dayColor.a = 0;
        dayIndicator.color = dayColor;

        dayIndicator.text = "День " + thisDay;
        while (dayColor.a < 1)
        {
            dayColor.a += 0.02f;
            dayIndicator.color = dayColor;
            yield return null;
        }
        dayColor.a = 1;
        dayIndicator.color = dayColor;

        yield return new WaitForSeconds(2);

        while (dayColor.a > 0)
        {
            dayColor.a -= 0.02f;
            dayIndicator.color = dayColor;
            yield return null;
        }
        dayIndicator.text = "";
    }

    IEnumerator StartDay()
    {
        thisDay++;
        StartCoroutine(ThisDay());
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
            if (temperature < dayTemperature)
            {
                temperature += 0.003f;
                Debug.Log("Теплеет");
            }
            RenderSettings.ambientSkyColor = Color.Lerp(Color.black, Color.white, time);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.black, Color.white, time);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.black, Color.white, time);
            time += 0.0003f;
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
            if (temperature > nightTemperature) temperature -= 0.001f;
            yield return null;
        }
        sunLight.SetActive(false);

        moonLight.SetActive(true);
        float time = 0;
        while (hour > 18 || hour < 4)
        {
            if (moonLight.GetComponent<Light>().intensity > 1.5f) break;
            moonLight.GetComponent<Light>().intensity += 0.0001f;
            if (temperature > nightTemperature) temperature -= 0.003f;
            RenderSettings.ambientSkyColor = Color.Lerp(Color.white, Color.black, time);
            RenderSettings.ambientEquatorColor = Color.Lerp(Color.white, Color.black, time);
            RenderSettings.ambientGroundColor = Color.Lerp(Color.white, Color.black, time);
            time += 0.0003f;
            yield return null;
        }
    }
}
