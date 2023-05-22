using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Состояния игрока")]
    public float feelingTemperature;
    public float clothesTemperature;
    public float buildingTemperature;
    public float torchTemperature;
    public float campfireTemperature;
    public bool isCold;
    public bool isCampfire;
    public bool isOnBed;
    public bool isLay;
    public bool isSit;
    public bool isSwim;
    public bool isUnderWater;
    public bool isRun;
    public bool isSleep;
    public bool isStart = true;
    [Header("Ограничения игрока")]
    public float runCoolDown;
    public bool isAbleJump;
    public bool isAbleRun;
    public bool isAbleCarry;
    public bool isAbleToSleep;
    public bool isAbleToGrabGrass;
    public bool isAbleToCollectWater;
    public bool isDead;
    public bool isControl;
    [Header("Текущие параметры игрока")]
    public int Health = 100;
    public int Cold = 100;
    public int Food = 100;
    public int Energy = 100;
    public int Stamina = 100;
    public int Oxygen = 100;
    public int Wood = 0;
    [Header("Развиваемые параметры")]
    public int maxHealth = 100;
    public int maxCold = 100;
    public int maxFood = 100;
    public int maxEnergy = 100;
    public int maxStamina = 100;
    public int maxOxygen = 100;
    public int staminaForJump = 10;
    public int runningSkill = 1;
    public int swimmingSkill = 1;
    public int maxCarryWeight = 150;
    [Header("Скорость восстановления параментров")]
    public int restoreEnergy = 1;  // Скорость восстановления энергии
    [Header("Очки прокачки")]
    public int survivalPoint;
    [Header("Статистика")]
    public int nighmares = 0;
    [Space]
    public UILinks ui;
    public Links links;
    [Space]
    public GameObject spawnPoint;
    public GameObject sleepPlace;
    public Vector3 deathPlace;
    public GameObject PlayerFollowCamera;
    public GameObject sleepCamera;

    float deltaWarm;
    float deltaHealth;
    float deltaEnergy;
    float deltaFood;

    float freezeFactor = 1;
    float tirednessFactor = 1;
    float hungerFactor = 1;
    float carryFactor;
    float runFactor;
    float swimfactor;
    float sleepFactor = 1;

    TOD_Sky time;

    [Header("Сохраняемые параметры")]
    public float saveTime;
    public int saveDay;
    public int savetMonth;
    public int saveYear;
    public float saveDayNumber;
    public int saveHealth;
    public int saveCold;
    public int saveFood;
    public int saveEnergy;

    public Animator animator;
    public MousePoint mousePoint;
    public Cinemachine.CameraController cameraController;

    public DayNight DayTime;
    public InventoryWindow inventoryWindow;
    public GameObject grassStack;



    void Start()
    {
        links = FindObjectOfType<Links>();

        Cursor.visible = true;
        time = DayTime.gameObject.GetComponent<TOD_Sky>();
        saveTime = links.cycle.Hour;
        saveDay = links.cycle.Day;
        savetMonth = links.cycle.Month;
        saveYear = links.cycle.Year;
        saveDayNumber = DayTime.thisDay;
        saveHealth = Health;
        saveCold = Cold;
        saveFood = Food;
        saveEnergy = Energy;

        ui.StaminaIndicator.value = Stamina;

        animator = GetComponent<Animator>();
        mousePoint = GetComponent<MousePoint>();
        gameObject.SetActive(false);
    }

    float fpsTime;
    void FPSmeter()
    {
        fpsTime += Time.deltaTime;
        if (fpsTime > 0.25f)
        {
            ui.fpsIndicator.text = (int)(1.0f / Time.deltaTime) + " fps";
            fpsTime = 0;
        }
    }

    float restTimer;
    float gameMinute;
    void Update()
    {
        FPSmeter();
        StaminaUpdate();
        OxygenUpdate();

        if (!isSleep && !isUnderWater && !isDead)
        {
            HealthMessages();
            StarveMessages();
            ColdMessages();
            EnergyMessages();
        }

        StatusUpdate();

        gameMinute = links.time.timeFactor / 60;

        // Ограничения при усталости
        if (Energy < 30) isAbleJump = false; else isAbleJump = true;
        if (Energy < 20) isAbleCarry = false; else isAbleCarry = true;
        if (Energy < 10) isAbleRun = false; else if (runCoolDown <= 0) isAbleRun = true;

        // Восстановление энергии лежа
        if (isLay)
        {
            restTimer += Time.deltaTime * gameMinute / 4;       // Умножаем на коэффициент игрового времени
            if (restTimer > restoreEnergy && Energy <= 35)  // Вместо реальных минут используем игровые
            {
                Energy += 1;
                restTimer = 0;
            }
        }

        // Бонус тепла от горящего факела
        torchTemperature = 0;
        if (inventoryWindow.RightHandItem != null && inventoryWindow.RightHandItem.Type == ItemType.Torch)
        {    
            if (inventoryWindow.RightHandObject.TryGetComponent(out Torchlight torch) && torch.isBurn)
                torchTemperature += 2 * torch.IntensityLight;
        }
        if (inventoryWindow.LeftHandItem != null && inventoryWindow.LeftHandItem.Type == ItemType.Torch)
        { 
            if (inventoryWindow.LeftHandObject.TryGetComponent(out Torchlight torch) && torch.isBurn)
                torchTemperature += 2 * torch.IntensityLight;
        }

        // Температура по ощущениям
        if (isSwim)
        {
            feelingTemperature = DayTime.temperature - 5;
            if (feelingTemperature < 2) feelingTemperature = 2;
            if (feelingTemperature > 15) feelingTemperature = 15;
        }
        else
        feelingTemperature = DayTime.temperature + clothesTemperature + buildingTemperature + torchTemperature + campfireTemperature;

        // Замерзание при температуре меньше coldTemperature градусов или при плавании когда закончится стамина
        if (feelingTemperature < DayTime.freezeTemperature) isCold = true;
        else isCold = false;

        // Постепенное согревание при температуре больше freezeTemperature градусов
        if (feelingTemperature > DayTime.freezeTemperature && !isSwim)
        {
            if (Cold < maxCold)
            {
                deltaWarm += Time.deltaTime * (feelingTemperature - DayTime.freezeTemperature) * 0.02f * gameMinute;
                if (deltaWarm > 1)
                { Cold++; deltaWarm = 0; }
            }
        }
        else
        {
            if (Cold > 0)  // Потеря тепла при низких температурах
            {
                
                deltaWarm += Time.deltaTime * (DayTime.freezeTemperature - feelingTemperature) * gameMinute * 0.2f;
                if (deltaWarm > 1)
                {
                    Cold--;
                    deltaWarm = 0;
                }
            }
            else  // Потеря здоровья от замерзания
            {
                deltaHealth -= Time.deltaTime * (DayTime.freezeTemperature - feelingTemperature) * gameMinute * 0.02f;
                if (deltaHealth < -1)
                { Health--; deltaHealth = 0; }
            }
        }

        // Потеря здоровья при перегреве
        if (feelingTemperature > DayTime.hotTemperature)
        {
            deltaHealth -= Time.deltaTime * (feelingTemperature - DayTime.hotTemperature) * gameMinute * 0.05f;
            if (deltaHealth < -1)
            { 
                Health--; deltaHealth = 0;
                if (!isDead) mousePoint.Comment("Как же жарко!");
            }
            
        }

        // Расход энергии
        if (Energy > 0 && !isOnBed)
        {
            deltaEnergy += Time.deltaTime * 0.04f * hungerFactor * freezeFactor * runFactor * swimfactor * carryFactor * gameMinute;
            if (deltaEnergy > 1)
            {
                Energy--;
                deltaEnergy = 0;
                if (Energy < 10) animator.SetBool("isTired", true);
                else animator.SetBool("isTired", false);
            }
        }
        
        // Множители влияющие на скорость траты энергии
        if (mousePoint.isCarry) carryFactor = 1 + mousePoint.carryWeight * 10 / 100; else carryFactor = 1;
        if (isRun) runFactor = 5; else runFactor = 1;
        if (isSwim) swimfactor = 3; else swimfactor = 1;

        if (Cold < 20) freezeFactor = 1.5f; else freezeFactor = 1;
        if (Energy < 20) tirednessFactor = 1.5f; else tirednessFactor = 1;
        if (Food < 20) hungerFactor = 1.5f; else hungerFactor = 1;

        //Расход сытости
        if (Food > 0)
        {
            deltaFood += Time.deltaTime * 0.1f * freezeFactor * tirednessFactor * gameMinute;
            if (deltaFood > 1)
            {
                Food--;
                deltaFood = 0;
            }
        }

        if (Health <= 0)
        {
            Death();
        }

        // Переключение иконок при состоянии замерзания/согревания
        if (isCold)
        {
            ui.ColdIcon.enabled = true;
            ui.WarmIcon.enabled = false;
        }
        else
        {
            ui.ColdIcon.enabled = false;
            ui.WarmIcon.enabled = true;
        }

        ui.HealthIndicator.text = "" + Health;
        ui.ColdIndicator.text = "" + Cold;
        ui.FoodIndicator.text = "" + Food;
        ui.EnergyIndicator.text = "" + Energy;
        ui.WoodIndicator.text = "" + Wood;
        ui.TemperatureIndicator.text = Mathf.Round(feelingTemperature) + "°C";

        if (Input.GetKeyDown(KeyCode.P)) Death();

        if (Input.GetKeyDown(KeyCode.Q) && (isLay || isSit) && !isSleep)
        {
            GetUp();
        }

        if (Input.GetKeyDown(KeyCode.X) && !isSit && !isSleep)
        {
            Sit();
        }

        if (isOnBed && !isSleep && Input.GetKeyDown(KeyCode.R))
        {
            if (isAbleToSleep) StartCoroutine(Sleep());
            else if (Energy >= maxEnergy) { links.mousePoint.Comment("Я слишком полон энергии, чтобы спать!"); }
            else if (Cold == 0) { links.mousePoint.Comment("Мне слишком холодно, чтобы уснуть!"); }
            else if (Food == 0) { links.mousePoint.Comment("На пустой желудок я не усну!"); }
        }

        // Ограничения сна
        if (Energy < maxEnergy && Food > 0 && Cold > 0) isAbleToSleep = true; else isAbleToSleep = false;

    }
    
    public IEnumerator CollectGrass()
    {
        float collectingProgress = 0;
        float timeToCollectGrass = 5;
        ui.progressIndicator.transform.parent.gameObject.SetActive(true);
        PlayerControl(false);
        while (collectingProgress < timeToCollectGrass)
        {
            collectingProgress += Time.deltaTime;
            Debug.Log("Collecting");
            ui.progressIndicator.fillAmount = collectingProgress / timeToCollectGrass;
            yield return null;
        }
        animator.SetBool("CollectGrass", false);
        ui.progressIndicator.fillAmount = 0;
        GameObject grass = Instantiate(grassStack);
        mousePoint.Carry(grass.GetComponent<ItemInfo>());
        ui.progressIndicator.transform.parent.gameObject.SetActive(false);
        PlayerControl(true);
    }
    
    public void PlayerControl(bool isOn)
    {
        if (isOn)
        {
            isControl = true;
            GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            isControl = false;
            GetComponent<CharacterController>().enabled = false;
        }
    }

    public void GetUp()
    {
        animator.SetTrigger("GetUp");
        if (Energy >= 10) animator.SetBool("isTired", false);
        isLay = false;
        isSit = false;
        isOnBed = false;
        ui.pressToStand.gameObject.SetActive(false);
        ui.pressToSleep.gameObject.SetActive(false);
        PlayerControl(true);
        isDead = false;
    }

    public void Lay()
    {
        if (mousePoint.carryObject != null) mousePoint.CarryRelease();
        animator.ResetTrigger("GetUp");
        animator.SetTrigger("Lay");
        PlayerControl(false);
        GrassGrab(false);
        isLay = true;
        ui.pressToStand.gameObject.SetActive(true);
    }

    public void Sit()
    {
        if (mousePoint.carryObject != null) mousePoint.CarryRelease();
        animator.SetTrigger("Sit");
        PlayerControl(false);
        GrassGrab(false);
        isSit = true;
        ui.pressToStand.gameObject.SetActive(true);
    }

    public IEnumerator Sleep()
    {
        PlayerControl(false);
        isSleep = true;
        sleepCamera.SetActive(true);
        Time.timeScale = 10;
        ui.pressToWakeUp.gameObject.SetActive(true);
        ui.pressToStand.gameObject.SetActive(false);
        ui.pressToSleep.gameObject.SetActive(false);

        float sleepTime = 0;
        float awakeTime = Random.Range(8f, 12f);

        bool isGetUp = false;

        while (true && isSleep)
        {   
            sleepFactor = 3;
            if (Input.GetKeyDown(KeyCode.Space)) break;
            if (Energy == maxEnergy) { links.mousePoint.Comment("Не могу спать, когда во мне столько энергии - надо чем-то заняться!"); break; }
            if (Food == 0) { links.mousePoint.Comment("Не могу нормально спать, когда в желудке так пусто!"); GetUp(); isGetUp = true; break; }
            if (Cold == 0) { links.mousePoint.Comment("Слишком холодно, чтобы дальше спать!"); GetUp(); isGetUp = true; break; }
            sleepTime += Time.deltaTime * links.time.timeFactor / 3600;
            if (sleepTime > awakeTime) { links.mousePoint.Comment($"Я выспался! Кажется я проспал часов {Mathf.Round(awakeTime)}!"); break; }

            // Восстановление здоровья во время сна при достаточной сытости
            if (Food >= 20)
            {
                deltaHealth += Time.deltaTime * 0.5f;
                if (deltaHealth > 1 && Health < maxHealth)
                {
                    Health++;
                    deltaHealth = 0;
                }
            }
            yield return null;
        }

        sleepFactor = 1;
        isSleep = false;
        sleepCamera.SetActive(false);
        Time.timeScale = 1;
        ui.pressToWakeUp.gameObject.SetActive(false);
        if (!isGetUp)
        {
            ui.pressToStand.gameObject.SetActive(true);
            ui.pressToSleep.gameObject.SetActive(true);
        }
        SaveGame();
        spawnPoint.transform.position = sleepPlace.transform.position;
        spawnPoint.transform.rotation = sleepPlace.transform.rotation;
    }

    void SaveGame()
    {
        saveTime = links.dayNight.hour;
        saveDay = links.cycle.Day;
        savetMonth = links.cycle.Month;
        saveYear = links.cycle.Year;
        saveDayNumber = links.dayNight.thisDay;
        saveHealth = Health;
        saveCold = Cold;
        saveFood = Food;
        saveEnergy = Energy;
        links.saveInventory.SaveItems();
    }

    float staminaTimer;
    bool isFreezeInWater;
    void StaminaUpdate()
    { 
        if (Stamina == maxStamina) ui.StaminaIndicator.gameObject.SetActive(false);
        else ui.StaminaIndicator.gameObject.SetActive(true);

        if (isRun)
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer > 0.03f * (10 + runningSkill) && Stamina > 0)
            {
                Stamina--;
                staminaTimer = 0;
                if (Stamina == 0)
                {
                    runCoolDown = 2;
                }
            }
        }
        else if (isSwim)
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer > 0.02f * (10 + swimmingSkill) && Stamina > 0)
            {
                isFreezeInWater = false;
                Stamina--;
                staminaTimer = 0;
            }
            //if (isSwim && Stamina == 0 && staminaTimer > 1f) // Если при плавании заканчивается стамина, то тратится тепло, затем здоровье
            //{
            //    if (Cold > 0)
            //    {
            //        Cold--;
            //        isCold = true;
            //        if (!isFreezeInWater) mousePoint.Comment("Холодно! Долго в такой воде не поплаваешь!");
            //        isFreezeInWater = true;
            //    }
            //    else
            //    {
            //        Health--;
            //        deltaHealth = -0.1f;
            //    }
            //    staminaTimer = 0;
            //}
        }
        else if (Stamina < maxStamina)
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer > 0.01f * (10 + runningSkill + swimmingSkill) && Stamina < maxStamina)
            {
                Stamina++;
                staminaTimer = 0;
            }            
        }
        if (!isRun && (runCoolDown > 0 || runCoolDown < -0.1f)) runCoolDown -= Time.deltaTime;
        ui.StaminaIndicator.value = Stamina;
    }

    float oxygenTimer = 0;
    void OxygenUpdate()
    {   
        if (isUnderWater)
        {
            oxygenTimer += Time.deltaTime * 5;
            if (Oxygen > 0 && oxygenTimer > 1)
            {
                Oxygen--;
                oxygenTimer = 0;
            }
            if (Oxygen == 0 && oxygenTimer > 1)
            {
                Health--;
                deltaHealth = -0.1f;
                oxygenTimer = 0;
            }
            ui.OxygenIndicator.gameObject.SetActive(true);
        }
        else
        {
            oxygenTimer += Time.deltaTime * 30;
            if (Oxygen < maxOxygen && oxygenTimer > 1)
            {
                Oxygen++;
                oxygenTimer = 0;
            }
        }
        ui.OxygenIndicator.value = Oxygen;
        if (Oxygen == maxOxygen) ui.OxygenIndicator.gameObject.SetActive(false);
    }

    public void Death()
    {
        if (mousePoint.carryObject != null) mousePoint.CarryRelease();
        PlayerControl(false);
        deathPlace = transform.position;
        transform.position = spawnPoint.transform.position;

        campfireTemperature = 0;
        isDead = true;
        if (!isStart)
        {
            nighmares++;
            ui.nightmaresIndicator.text = "" + nighmares;
        }
        else
        {
            links.questWindow.FollowingQuestPanel.SetActive(true);
            links.saveInventory.SaveContainers();
            isStart = false;
        }

        time.Cycle.Hour = saveTime;
        time.Cycle.Day = saveDay;
        time.Cycle.Month = savetMonth;
        time.Cycle.Year = saveYear;
        DayTime.thisDay = saveDayNumber;
        if (saveHealth < 20) Health = 20; else Health = saveHealth;
        if (saveCold < 20) Cold = 20; else Cold = saveCold;
        Food = saveFood;
        Energy = saveEnergy;
        links.saveInventory.LoadItems();

        if (saveTime >= 6 && saveTime < 18) DayTime.Day(); else DayTime.Night();

        links.dayNight.ShowDay();

        isLay = true;
        isSleep = false;

        animator.SetTrigger("Lay");

        transform.eulerAngles = new Vector3(0, 50, 0);
        cameraController.RotateCameraToPlayer(true);

        ui.pressToStand.gameObject.SetActive(true);
        ui.pressToSleep.gameObject.SetActive(false);

        mousePoint.IconHighLight.SetActive(false);

        inventoryWindow.RecountWood();

        links.mousePoint.Comment("Присниться же такое!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            Health -= damage.SingleDamage();
        }
    }

    public void RestZone(int restZonePower)
    {
        if (isLay)
        {
            isOnBed = true;
            Debug.Log("Rest");
            deltaEnergy += Time.deltaTime * 0.003f * restZonePower * sleepFactor * links.time.timeFactor / 60;
            if (deltaEnergy > 1 && Energy < maxEnergy)
            {
                Energy++;
                deltaEnergy = 0;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out RestZone restZone))
        {
            isOnBed = false;
        }
    }


    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.TryGetComponent(out Damage damage))
    //    {
    //        Health -= damage.damage;
    //    }
    //}

    public void GrassGrab(bool isGrab)
    {
        if (isGrab)
        {
            ui.pressToGrabGrass.gameObject.SetActive(true);
            isAbleToGrabGrass = true;
        }
        else
        {
            ui.pressToGrabGrass.gameObject.SetActive(false);
            isAbleToGrabGrass = false;
        }
    }

    int healthAmountForMessage = 20;
    void HealthMessages()
    {
            if (Health == 20 && healthAmountForMessage == Health)
            {
                mousePoint.Comment("Что-то мне не хорошо!");
                healthAmountForMessage = 10;
            }
            if (Health == 10 && healthAmountForMessage == Health)
            {
                mousePoint.Comment("Очень плохо себя чувствую!");
                healthAmountForMessage = maxHealth;
            }
            if (Health == maxHealth && healthAmountForMessage == Health)
            {
                mousePoint.Comment("Самочувствие как будто заново родился!");
                healthAmountForMessage = 20;
            }
    }

    int foodAmountForMessage = 30;
    void StarveMessages()
    {
        if (Food == 30 && foodAmountForMessage == Food)
        {
            mousePoint.Comment("Перекусить бы чего-нибудь");
            foodAmountForMessage = 15;
        }
        if (Food == 15 && foodAmountForMessage == Food)
        {
            mousePoint.Comment("Как же хочется есть!");
            foodAmountForMessage = 0;
        }
        if (Food == 0 && foodAmountForMessage == Food)
        {
            mousePoint.Comment("В животе совсем пусто! Я теряю силы!");
            foodAmountForMessage = 30;
        }
    }

    int warmAmountForMessage = 100;
    int coldAmountForMessage = 40;
    void ColdMessages()
    {
        if (Cold == maxCold && warmAmountForMessage == Cold)
        {
            if (isCampfire) mousePoint.Comment("Я полностью согрелся!");
            warmAmountForMessage = 50;
        }
        if (Cold == 50 && warmAmountForMessage == Cold)
        {
            mousePoint.Comment("Прохладно!");
            warmAmountForMessage = maxCold;
        }

        if (Cold == 40 && coldAmountForMessage == Cold)
        {
            mousePoint.Comment("Холодает!");
            coldAmountForMessage = 20;
        }
        if (Cold == 20 && coldAmountForMessage == Cold)
        {
            mousePoint.Comment("Что-то холодновато! Надо бы согреться!");
            coldAmountForMessage = 0;
        }
        if (Cold == 0 && coldAmountForMessage == Cold)
        {
            mousePoint.Comment("Я совсем замёрз! Холод забирает мои жизненные силы!");
            coldAmountForMessage = 40;
        }
    }

    int energyAmountForMessage = 30;
    void EnergyMessages()
    {
        if (Energy == 30 && energyAmountForMessage == Energy)
        {
            mousePoint.Comment("Что-то я подустал! Неплохо бы найти место для отдыха!");
            energyAmountForMessage = 25;
        }
        if (Energy == 25 && energyAmountForMessage == Energy)
        {
            mousePoint.Comment("Усталость меня одолевает!");
            energyAmountForMessage = 20;
        }
        if (Energy == 20 && energyAmountForMessage == Energy)
        {
            mousePoint.Comment("Ноги уже еле держат!");
            energyAmountForMessage = 0;
        }
        if (Energy == 0 && energyAmountForMessage == Energy)
        {
            mousePoint.Comment("Сил совсем нет! Готов упасть прямо здесь!");
            energyAmountForMessage = 30;
        }
    }

    public string healthStatus;
    public string coldStatus;
    public string foodStatus;
    public string energyStatus;
    public string temperatureStatus;

    void StatusUpdate()
    {
        if (Health >= 90) healthStatus = "Отличное самочувствие";
        else if (Health < 90 && Health >= 60) healthStatus = "Нормальное самочувствие";
        else if (Health < 60 && Health >= 30) healthStatus = "Самочувствие средней паршивости";
        else healthStatus = "Очень плохо";
        ui.healthStatusIcon.itemDescription = healthStatus;
        ui.healthStatusIcon.itemComment = Health + " из " + maxHealth;

        if (Cold >= 90) coldStatus = "Очень тепло";
        else if (Cold < 90 && Cold >= 60) coldStatus = "Комфортно";
        else if (Cold < 60 && Cold >= 30) coldStatus = "Прохладно";
        else if (Cold < 30 && Cold >= 1) coldStatus = "Очень холодно";
        else coldStatus = "Переохлаждение";
        ui.coldStatusIcon.itemDescription = coldStatus;
        ui.coldStatusIcon.itemComment = Cold + " из " + maxCold;

        if (Food >= 80) foodStatus = "Полная сытость";
        else if (Food < 80 && Food > 20) foodStatus = "Легкий голод";
        else foodStatus = "Истощение";
        ui.foodStatusIcon.itemDescription = foodStatus;
        ui.foodStatusIcon.itemComment = Food + " из " + maxFood;

        if (Energy >= 80) energyStatus = "Полон сил";
        else if (Energy < 80 && Energy >= 40) energyStatus = "Легкая усталость";
        else if (Energy < 40 && Energy >= 20) energyStatus = "Сильная усталость";
        else energyStatus = "Изнемождение";
        ui.energyStatusIcon.itemDescription = energyStatus;
        ui.energyStatusIcon.itemComment = Energy + " из " + maxEnergy;

        ui.temperatureStatusIcon.itemDescription = "Температура воздуха: " + Mathf.Round(DayTime.temperature);
        

    }

    public void UpdateSkillPoints()
    {
        ui.newSkillPointsIndicator.text = "" + survivalPoint;
        if (survivalPoint > 0) ui.upgradeButtons.SetActive(true);
        else ui.upgradeButtons.SetActive(false);

        if (survivalPoint > 0) ui.newSkillPointsSign.SetActive(true);
        else ui.newSkillPointsSign.SetActive(false);
    }

    public void AddSkillPoint(int skillIndex)
    {
        if (skillIndex == 0) maxHealth++;
        if (skillIndex == 1) maxCold++;
        if (skillIndex == 2) maxFood++;
        if (skillIndex == 3) maxEnergy++;
        survivalPoint--;
        UpdateSkillPoints();
    }
}


