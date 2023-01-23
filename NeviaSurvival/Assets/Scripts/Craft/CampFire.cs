using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        time = FindObjectOfType<TOD_Time>();
        fire = GetComponentInChildren<ParticleSystem>();
    }


    void Update()
    {
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
            burningTimeText = $"����� ��������� �������";
            fire.gameObject.SetActive(false);
        }
        else if (burningTime < 0.5f) burningTimeText = $"����� ��������";
        else if (burningTime < 1.5f && burningTime > 0.5f)
            burningTimeText = $"������ ����� �������� {Mathf.Round(burningTime)} ���";
        else if (burningTime > 1.5f && burningTime < 4.5f)
            burningTimeText = $"������ ����� �������� {Mathf.Round(burningTime)} ����";
        else if (burningTime > 4.5f) burningTimeText = $"������ ����� �������� {Mathf.Round(burningTime)} �����";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out NPC_Move npc) && npc._isFearOfFire)
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
                player.isCold = false;

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
        }
    }

}
