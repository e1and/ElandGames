using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject DayTime;
    public int Health = 100;
    public int Cold = 100;
    public int Sticks = 0;
    public float deltaCold = 0;
    public Text HealthIndicator;
    public Text ColdIndicator;

    void Start()
    {
       Cursor.visible = true; 
    }

    void Update()
    {
        if (Health <= 0) {
            Debug.Log("1"); 
        }

        deltaCold = deltaCold + 0.01f;
        if (deltaCold >= 1) 
        { if (Cold > 0) { Cold -= 1; deltaCold = 0f; }
            else Health -= 1; deltaCold = 0f;
        }
        HealthIndicator.text = "" + Health;
        ColdIndicator.text = "" + Cold;
        //DayTime.GetComponent<StickInvent>().stick


    }
}
