using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject DayTime;
    public float Speed = 4f;
    public Vector3 moveVector;
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
        moveVector = Vector3.zero;
        moveVector = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveVector = transform.TransformDirection(moveVector.normalized) * Speed;

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
