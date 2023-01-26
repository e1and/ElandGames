using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public DayNight DayTime;
    
    public bool isCold = true;
    public bool isCampfire;
    public int Health = 100;
    public int Cold = 100;
    public int Food = 100;
    public int Sticks = 0;
    [Space]
    public Text HealthIndicator;
    public Text ColdIndicator;
    public Text FoodIndicator;
    public Text StickIndicator;
    public Text TemperatureIndicator;
    [Space]
    public Image ColdIcon;
    public Image WarmIcon;
    [Space]
    public GameObject spawnPoint;
    public GameObject PlayerFollowCamera;

    float deltaWarm;
    float deltaHealth;

    TOD_Sky time;
    public float saveTime;
    public int saveDay;
    public int savetMonth;
    public int saveYear;
    public float saveThisDay;

    public Animator animator;

    void Start()
    {
        Cursor.visible = true;
        time = DayTime.gameObject.GetComponent<TOD_Sky>();
        saveDay = time.Cycle.Day;
        savetMonth = time.Cycle.Month;
        saveYear = time.Cycle.Year;
        saveThisDay = DayTime.thisDay;

    }

    void Update()
    {
        // Замерзание при температуре меньше coldTemperature градусов
        if (DayTime.temperature < DayTime.coldTemperature && !isCampfire) isCold = true;
        else isCold = false;

        // Постепенное согревание при температуре больше coldTemperature градусов
        if (DayTime.temperature > DayTime.coldTemperature)
        {
            if (Cold < 100)
            {
                deltaWarm += Time.deltaTime * (DayTime.temperature - DayTime.coldTemperature) * 0.1f;
                if (deltaWarm > 1)
                { Cold++; deltaWarm = 0; }
            }
        }
        else
        {
            if (Cold > 0 && !isCampfire)
            {
                deltaWarm += Time.deltaTime * (DayTime.coldTemperature - DayTime.temperature) * 0.1f;
                if (deltaWarm > 1)
                { Cold--; deltaWarm = 0; }
            }
            else if (!isCampfire)
            {
                deltaHealth += Time.deltaTime * (DayTime.coldTemperature - DayTime.temperature) * 0.1f;
                if (deltaHealth > 1)
                { Health--; deltaHealth = 0; }
            }
        }

        if (Health <= 0)
        {
            Death();
        }

        if (isCold)
        {
            ColdIcon.enabled = true;
            WarmIcon.enabled = false;
        }
        else
        {
            ColdIcon.enabled = false;
            WarmIcon.enabled = true;
        }

        HealthIndicator.text = "" + Health;
        ColdIndicator.text = "" + Cold;
        FoodIndicator.text = "" + Food;
        StickIndicator.text = "" + Sticks;
        TemperatureIndicator.text = Mathf.Round(DayTime.temperature) + "C";
        //DayTime.GetComponent<StickInvent>().stick

        if (Input.GetKeyDown(KeyCode.E)) Death();
        if (Input.GetKeyDown(KeyCode.Q)) animator.SetTrigger("GetUp");
    }

    void Death()
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = spawnPoint.transform.position;
        Health = 20;
        Cold = 20;
        Food = 20;
        Sticks = 0;
        GetComponent<CharacterController>().enabled = true;

        DayTime.Day();
        time.Cycle.Hour = 4;
        time.Cycle.Day = saveDay;
        time.Cycle.Month = savetMonth;
        time.Cycle.Year = saveYear;
        DayTime.thisDay = saveThisDay;

        animator.SetTrigger("Death");
        transform.eulerAngles = new Vector3(0, -90, 0);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Damage damage))
        {
            Health -= damage.damage;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.TryGetComponent(out Damage damage))
    //    {
    //        Health -= damage.damage;
    //    }
    //}
}

