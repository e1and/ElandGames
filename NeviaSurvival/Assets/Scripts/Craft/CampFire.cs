using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(SphereCollider))]
public class Campfire : MonoBehaviour
{
    [SerializeField] float burningTime = 2;
    [SerializeField] TOD_Time time;
    [SerializeField] string burningTimeText;
    TimeSpan timeHours;
    [SerializeField] ParticleSystem fire;
    [SerializeField] GameObject warmTrigger;
    [SerializeField] int warmAmount = 1;
    [SerializeField] float warm;
    [SerializeField] float distance;

    [SerializeField] Vector3 campfirePos;
    [SerializeField] Vector3 spiderPos;
    [SerializeField] Vector3 newPos;

    public NavMeshObstacle obstacle;
    SphereCollider sphere;

    void Start()
    {
        time = FindObjectOfType<TOD_Time>();
        fire = GetComponentInChildren<ParticleSystem>();
        obstacle = GetComponent<NavMeshObstacle>();
        sphere = GetComponent<SphereCollider>();
    }


    void Update()
    {
        obstacle.radius = burningTime / 2;
        sphere.radius = burningTime / 1.9f;
        
        campfirePos = transform.position;
        
        if (burningTime > 0) burningTime -= Time.deltaTime * time.timeFactor / 3600f;
        else burningTime = 0;
        timeHours = TimeSpan.FromHours(burningTime);


        fire.startSize = 0.1f * burningTime;
        if (burningTime > 8)
        {
            fire.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            fire.startSize = 0.8f;
        }
        else
            fire.gameObject.transform.localScale = new Vector3(0.1f * burningTime + 0.4f, 0.1f * burningTime + 0.4f, 0.1f * burningTime + 0.4f);
        if (burningTime < 1)
        {
            fire.startSize += 0.2f;
        }



        if (burningTime <= 0)
        {
            burningTimeText = $"Костёр полностью догорел";
            fire.gameObject.SetActive(false);
        }
        else if (burningTime < 0.5f) burningTimeText = $"Костёр догорает";
        else if (burningTime < 1.5f && burningTime > 0.5f)
            burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} час";
        else if (burningTime > 1.5f && burningTime < 4.5f)
            burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} часа";
        else if (burningTime > 4.5f) burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} часов";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out NPC_Move npc) && npc._isFearOfFire && burningTime > 0)
        {
            Debug.Log("Enter Campfire Radius");
            //npc.StopMove();
            npc._isEscape = true;
            spiderPos = npc.gameObject.transform.position;
            newPos = ((npc.gameObject.transform.position - transform.position) + npc.gameObject.transform.position);
            npc.Escape(newPos);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            if (burningTime > 0)
            {
                player.isCold = false;
                player.isCampfire = true;
            }
            else 
            {
                player.isCampfire = false;
            }

            distance = Vector3.Distance(player.gameObject.transform.position, transform.position);
            if (distance != 0 && player.Cold < 100 && burningTime > 0)
            {
                warm += burningTime * warmAmount * Time.deltaTime / distance;
                if (warm > 1)
                { player.Cold++; warm = 0; }
            }
            if (burningTime <= 0) player.isCold = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            player.isCold = true;
            player.isCampfire = false;
        }
    }

}
