using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject DayTime;
    public bool isCold = true;
    public float Speed = 4f;
    public Vector3 moveVector;
    public int Health = 100;
    public int Cold = 100;
    public int Sticks = 0;
    public float deltaCold = 0;
    public Text HealthIndicator;
    public Text ColdIndicator;
    public GameObject spawnPoint;
    public GameObject PlayerFollowCamera;

    void Start()
    {
       Cursor.visible = true; 
    }

    void Update()
    {
        moveVector = Vector3.zero;
        moveVector = new Vector3(0, 0, Input.GetAxis("Vertical"));
        moveVector = transform.TransformDirection(moveVector.normalized) * Speed;

        if (Health <= 0) 
        {
            Death(); 
        }

        if (isCold)
        {
            deltaCold = deltaCold + 0.01f;
            if (deltaCold >= 1)
            {
                if (Cold > 0) Cold -= 1;
                else Health -= 1;

                deltaCold = 0f;
            }
        }

        HealthIndicator.text = "" + Health;
        ColdIndicator.text = "" + Cold;
        //DayTime.GetComponent<StickInvent>().stick

        if (Input.GetKeyDown(KeyCode.E)) Death();
    }

    void Death()
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = spawnPoint.transform.position;
        Health = 100;
        Cold = 100;
        GetComponent<CharacterController>().enabled = true;
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
