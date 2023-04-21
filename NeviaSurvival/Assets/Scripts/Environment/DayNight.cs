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
    [SerializeField] GameObject sun;
    [SerializeField] GameObject moon;

    [SerializeField] bool isDay;
    TimeSpan time;
    public float temperature;
    public float dayTemperature = 25;
    public float nightTemperature = 0;
    public float freezeTemperature = 17;
    public float thisDay = 1;
    [SerializeField] TMP_Text dayIndicator;
    [SerializeField] Material daySkyBoxMaterial;
    [SerializeField] Material nightSkyBoxMaterial;
    [SerializeField] Color nightFogColor;
    [SerializeField] Color dayFogColor;
    [SerializeField] Color sunsetColor;
    [Space]
    [SerializeField] Color nightAmbientColor;
    [SerializeField] Color nightAmbientColor2;
    [SerializeField] Color nightAmbientColor3;
    [Space]
    [SerializeField] Color dayAmbientColor;
    [SerializeField] Color dayAmbientColor2;
    [SerializeField] Color dayAmbientColor3;

    [SerializeField] float moonLightIntensity = 1;
    [SerializeField] float sunLightIntensity= 1.3f;

    [SerializeField] int startDayTime = 6;
    [SerializeField] int startNightTime = 18;
    [SerializeField] float skyExposure = 0.53f;
    [SerializeField] float sunRisePos;
    [SerializeField] float sunSetPos;
    [SerializeField] float fullExposurePos;

    Links links;


    void Start()
    {
        links = FindObjectOfType<Links>();
        
        hour = skyScript.hour;

        dayIndicator.text = "";
        if (hour < 5.5f || hour > 6f) StartCoroutine(ShowThisDayNumber());



        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        if (hour > startDayTime && hour < startNightTime)
        {
            RenderSettings.skybox = daySkyBoxMaterial;
            RenderSettings.skybox.SetFloat("_Exposure", skyExposure);
            isDay = true;
            Day();
        }
        else
        {
            RenderSettings.skybox = nightSkyBoxMaterial;
            RenderSettings.skybox.SetFloat("_Exposure", skyExposure);
            isDay = false;
            Night();
        }
        if (hour < 6) 
            thisDay = 1;
    }

    public void Day()
    {
        
        //sunLight.SetActive(true);
        //moonLight.SetActive(false);
        sunLight.GetComponent<Light>().intensity = 1.3f;
        moonLight.GetComponent<Light>().intensity = 0;
        RenderSettings.ambientSkyColor = dayAmbientColor;
        RenderSettings.ambientEquatorColor = dayAmbientColor2;
        RenderSettings.ambientGroundColor = dayAmbientColor3;
    }

    public void Night()
    {
        
       //sunLight.SetActive(false);
        //moonLight.SetActive(true);
        moonLight.GetComponent<Light>().intensity = 1f;
        sunLight.GetComponent<Light>().intensity = 0;
        RenderSettings.ambientSkyColor = nightAmbientColor;
        RenderSettings.ambientEquatorColor = nightAmbientColor2;
        RenderSettings.ambientGroundColor = nightAmbientColor3;
    }

    public float step;
    public float timestep;
    public float timestepMoon;
    float exposureStep;
    float time1;
    void Update()
    {
        hour = skyScript.hour;
        time = TimeSpan.FromHours(hour);
        links.ui.timeIndicator.text = time.Hours.ToString("00") + ":" + time.Minutes.ToString("00");
        links.ui.statsTimeIndicator.text = links.ui.timeIndicator.text;

        step = (sunRisePos - sunSetPos) / 1000;

        timestep = (sun.transform.position.y - sunSetPos) * step;
        timestepMoon = (moon.transform.position.y - sunSetPos) * step;

        if (timestep > 1) timestep = 1; if (timestep < 0) timestep = 0f;
        if (timestepMoon > 1) timestepMoon = 1; if (timestepMoon < 0) timestepMoon = 0;

        sunLight.GetComponent<Light>().intensity = timestep * sunLightIntensity;
        moonLight.GetComponent<Light>().intensity = timestepMoon * moonLightIntensity;

        skyScript.Night.ColorMultiplier = timestepMoon / 2 + 0.5f;

        RenderSettings.reflectionIntensity = timestep / 2;

        RenderSettings.ambientSkyColor = Color.Lerp(nightAmbientColor, dayAmbientColor, timestep);
        RenderSettings.ambientEquatorColor = Color.Lerp(nightAmbientColor2, dayAmbientColor2, timestep);
        RenderSettings.ambientGroundColor = Color.Lerp(nightAmbientColor3, dayAmbientColor3, timestep);
        RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, timestep);

        if (timestep > 0.2) { RenderSettings.skybox = daySkyBoxMaterial; }
        else { RenderSettings.skybox = nightSkyBoxMaterial; }

        exposureStep = (sun.transform.position.y - sunSetPos) * (fullExposurePos - sunSetPos) / 1000;

        if (exposureStep > 1) exposureStep = 1;
        if (exposureStep < 0.2f) exposureStep = 0.2f; if (timestepMoon < 0.2f) timestepMoon = 0.2f;
        daySkyBoxMaterial.SetFloat("_Exposure", skyExposure * exposureStep - 0.2f);
        nightSkyBoxMaterial.SetFloat("_Exposure", skyExposure * timestepMoon - 0.2f);




        if (hour > startDayTime && hour < startNightTime && !isDay) { StartCoroutine(StartDay()); isDay = true; Debug.Log("Start Day"); }
        if (hour > startNightTime && isDay) { StartCoroutine(StartNight()); isDay = false; Debug.Log("Start Night"); }
    }

    public Color dayColor;
    public IEnumerator ShowThisDayNumber()
    {
        dayColor.a = 0;

        Debug.Log("day number " + thisDay);
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
        dayColor.a = 0;
        dayIndicator.text = "";
    }

    public Coroutine showDayNumber;

    IEnumerator StartDay()
    {
        isDay = true;
        if (links.music.music.clip != links.music.dayMusic) links.music.DayMusic();
        if (!links.player.isDead) 
        { 
            thisDay++; StartCoroutine(ShowThisDayNumber());
            if (thisDay == 2) links.questWindow.QuestDone(links.questWindow.Survive1Day);
        }
        else links.player.isDead = false;

        //if (showDayNumber != null) StopCoroutine(showDayNumber);



        //moonLight.SetActive(false);

        //sunLight.SetActive(true);
        links.ui.temperatureStatusIcon.itemComment = "Теплеет";
        while (hour < startNightTime && hour > startDayTime && temperature < dayTemperature)
        {

            temperature += 0.003f;
     
            yield return null;
        }
        links.ui.temperatureStatusIcon.itemComment = "Тепло";
    }

    IEnumerator StartNight()
    {
        isDay = false;
        if (links.music.music.clip != links.music.nightMusic) links.music.NightMusic();

        //sunLight.SetActive(false);

        //moonLight.SetActive(true);

        links.ui.temperatureStatusIcon.itemComment = "Холодает";
        while ((hour > startNightTime || hour < startDayTime) && temperature > nightTemperature)
        {
            temperature -= 0.003f;

            yield return null;
        }
        links.ui.temperatureStatusIcon.itemComment = "Ночной холод";

    }
}
