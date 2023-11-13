using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
    public Slider CookingIndicator;
    public Slider BurningIndicator;
    public Image progressIndicator;

    public Slider XPIndicator;

    public TMP_Text areaTitle;
    public TMP_Text locationTitle;

    public List<string> locationTitles;
    
    [Header("ќкно карты:")]
    
    public TMP_Text mapAreaTitle;
    public TMP_Text mapLocationTitle;

    [Header("ќкно статистики:")] 
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text gainXPText;
    public TMP_Text gainXPStringText;
    public TMP_Text statsTimeIndicator;
    public Text nightmaresIndicator;
    public Text newSkillPointsIndicator;
    public Text armorIndicator;
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
    public GameObject mapPanel;

    public GameObject savingGameText;
    public GameObject pauseText;
    public GameObject thisDayIndicator;
    public GameObject questCompleteSign;
    public TMP_Text activeQuestCount;
    public GameObject newSkillPointsSign;

    [Header("—ообщени€-подсказки:")]
    public TMP_Text pressToStand;
    public TMP_Text pressToSleep;
    public TMP_Text pressToWakeUp;
    public TMP_Text pressToGrabGrass;
    public TMP_Text pressToDrop;

    [Header("Ёффекты")] public RawImage freezingEffect;
    public Color freezingImageColor;


    private List<Task> tasks = new List<Task>();

    public async void ShowTextInARow(TMP_Text textBlock, TMP_Text textBlock2, string text, string text2)
    {
        Task previuosTask = null;
        if (tasks.Count > 0) previuosTask = tasks[^1];
        
        Task task = ShowText(textBlock, textBlock2, text, text2, previuosTask);
        
        tasks.Add(task);
        await task;
        tasks.Remove(task);
    }
    
    public async Task ShowText(TMP_Text textBlock, TMP_Text textBlock2, string text, string text2, Task task)
    {
        if (task != null) await task;
        
        Color tempColor = textBlock.color;
        tempColor.a = 0;
        textBlock.color = tempColor;
        textBlock.text = text;

        Color tempColor2 = Color.black;
        if (textBlock2 != null)
        {
            tempColor2 = textBlock2.color;
            tempColor2.a = 0;
            textBlock2.color = tempColor2;
            textBlock2.text = text2;
        }
        
        while (tempColor.a < 1)
        {
            tempColor.a += 0.04f;
            textBlock.color = tempColor;
            if (textBlock2 != null)
            {
                tempColor2.a += 0.04f;
                textBlock2.color = tempColor2;
            }

            await UniTask.DelayFrame(1);
        }
        
        tempColor.a = 1;
        textBlock.color = tempColor;
        if (textBlock2 != null)
        {
            tempColor2.a = 1;
            textBlock2.color = tempColor2;
        }

        await UniTask.Delay(3000);

        while (tempColor.a > 0)
        {
            tempColor.a -= 0.04f;
            textBlock.color = tempColor;

            if (textBlock2 != null)
            {
                tempColor2.a -= 0.04f;
                textBlock2.color = tempColor2;
            }

            await UniTask.DelayFrame(1);
        }
        tempColor.a = 0;
        textBlock.color = tempColor;
        
        if (textBlock2 != null)
        {
            tempColor2.a = 0;
            textBlock2.color = tempColor2;
        }
    }
    
    private List<Task> XPTasks = new List<Task>();

    public async void ShowXpInARow(int XP, string text)
    {
        Task previuosTask = null;
        if (XPTasks.Count > 0) previuosTask = XPTasks[^1];
        
        Task task = ShowXP(XP, text, previuosTask);
        
        XPTasks.Add(task);
        await task;
        XPTasks.Remove(task);
    }
    
    public async Task ShowXP(int XP, string text, Task task)
    {
        if (task != null) await task;
        
        Color xpColor;
        Color stringColor;

        xpColor = gainXPText.color;
        stringColor = gainXPStringText.color;
        xpColor.a = 0;
        stringColor.a = 0;
        gainXPText.color = xpColor;
        gainXPStringText.color = stringColor;

        gainXPText.text = "+" + XP + " опыта";
        gainXPStringText.text = text;
        
        while (xpColor.a < 1)
        {
            xpColor.a += 0.02f;
            stringColor.a += 0.02f;
            gainXPText.color = xpColor;
            gainXPStringText.color = stringColor;
            await UniTask.DelayFrame(1);
        }
        xpColor.a = 1;
        stringColor.a = 1;
        gainXPText.color = xpColor;
        gainXPStringText.color = stringColor;

        await UniTask.Delay(3000);

        while (xpColor.a > 0)
        {
            xpColor.a -= 0.02f;
            stringColor.a -= 0.02f;
            gainXPText.color = xpColor;
            gainXPStringText.color = stringColor;
            await UniTask.DelayFrame(1);
        }
        xpColor.a = 0;
        stringColor.a = 0;
        gainXPText.color = xpColor;
        gainXPStringText.color = stringColor;
    }
}
