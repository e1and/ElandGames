using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireDamage : MonoBehaviour
{
    public Campfire campfire;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    float timer;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            Debug.Log("Fire!");
            timer += Time.deltaTime * campfire.burningTime * 30;
            if (timer > 2)
            {
                player.Health--;
                player.mousePoint.Comment("ААА! Как горячо!!!");
                timer = 0;
            }
        }
    }
}
