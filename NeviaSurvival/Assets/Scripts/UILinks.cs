using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILinks : MonoBehaviour
{
    public Text HealthIndicator;
    public Text ColdIndicator;
    public Text FoodIndicator;
    public Text WoodIndicator;
    public Text TemperatureIndicator;
    public TMP_Text timeIndicator;
    
    public Text EnergyIndicator;
    public Slider StaminaIndicator;
    public Slider OxygenIndicator;
    public Image progressIndicator;

    [Header("Окно статистики:")]
    public TMP_Text statsTimeIndicator;
    public Text nightmaresIndicator;
    public Text newSkillPointsIndicator;
    public GameObject upgradeButtons;
    [Space]
    public TMP_Text fpsIndicator;
    [Space]
    public Image ColdIcon;
    public Image WarmIcon;
    [Space]
    public ItemInfo healthStatusIcon;
    public ItemInfo coldStatusIcon;
    public ItemInfo warmStatusIcon;
    public ItemInfo foodStatusIcon;
    public ItemInfo energyStatusIcon;
    public ItemInfo temperatureStatusIcon;

    [Space]
    public GameObject inventoryPanel;
    public GameObject equipmentPanel;
    public GameObject statusPanel;
    public GameObject infoPanel;
    public GameObject helpPanel;
    public GameObject questPanel;
    public GameObject startPanel;
    public GameObject buildingPanel;
    public GameObject aboutPanel;
    public GameObject mainMenuPanel;
    public GameObject startMenuPanel;

    public GameObject pauseText;
    public GameObject thisDayIndicator;

    [Header("Сообщения-подсказки:")]
    public TMP_Text pressToStand;
    public TMP_Text pressToSleep;
    public TMP_Text pressToWakeUp;
    public TMP_Text pressToGrabGrass;


}
