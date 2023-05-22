using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(SphereCollider))]
public class Campfire : MonoBehaviour
{
    [Header("Время горения костра в игровых часах")]
    public float burningTime = 2;
    public string burningTimeText;

    [Header("Ссылки на компоненты")]
    [SerializeField] TOD_Time time;
    
    TimeSpan timeHours;
    [Space(10)]
    [Header("Ссылки на партиклы огня и дыма")]
    [SerializeField] ParticleSystem fire;
    ParticleSystem.MainModule fireMain;
    ParticleSystem.MainModule smokeMain;
    Color smokeColor;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem embers;
    [SerializeField] ParticleSystem embersSmall;
    [SerializeField] ParticleSystem glow;
    [SerializeField] GameObject fireLight;
    [Space(10)]
    [Header("Дрова")]
    [SerializeField] GameObject[] fireWoods;
    [Space(10)]
    [Header("Материал и текстуры состояния костра")]
    [SerializeField] Material Shader;
    [SerializeField] Texture burningTexture;
    [SerializeField] Texture extinctTexture;
    MeshRenderer[] meshRenderer;
    [Space(10)]
    [Header("Функции согревания и отпугивания")]
    [SerializeField] GameObject warmTrigger;
    [SerializeField] int warmAmount = 1;
    [SerializeField] float warm;
    [SerializeField] float distance;

    [SerializeField] Vector3 campfirePos;
    [SerializeField] Vector3 spiderPos;
    [SerializeField] Vector3 newPos;

    public NavMeshObstacle obstacle;
    SphereCollider sphere;
    ItemInfo info;
    AudioSource audioSource;
    float volume;

    public Transform cauldronPlace;
    public Container cauldron;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        volume = audioSource.volume;
        time = FindObjectOfType<TOD_Time>();
        //fire = GetComponentInChildren<ParticleSystem>();
        obstacle = GetComponent<NavMeshObstacle>();
        sphere = warmTrigger.GetComponent<SphereCollider>();
        fireMain = fire.main;
        smokeMain = smoke.main;
        smokeColor = smokeMain.startColor.color;
        info = GetComponent<ItemInfo>();
        for (int i = 0; i < fireWoods.Length; i++)
        {
            if (burningTime < i) fireWoods[i].SetActive(false);
        }

        meshRenderer = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material = new Material(Shader);
            meshRenderer[i].material.mainTexture = burningTexture;
        }
        StartCoroutine(burningWoodPerHour());
    }

    public float smokeTime = 0;
    public float colorA;

    IEnumerator burningWoodPerHour()
    {
        audioSource.enabled = true;
        audioSource.volume = volume;
        
        fireLight.SetActive(true);
        while (burningTime > 8) { yield return null; }
        fireWoods[7].SetActive(false);

        while (burningTime > 7) { yield return null; }
        fireWoods[6].SetActive(false);

        while (burningTime > 6) { yield return null; }
        fireWoods[5].SetActive(false);

        while (burningTime > 5) { yield return null; }
        fireWoods[4].SetActive(false);

        while (burningTime > 4) { yield return null; }
        fireWoods[3].SetActive(false);

        while (burningTime > 3) { yield return null; }
        fireWoods[2].SetActive(false);

        while (burningTime > 2) { yield return null; }
        fireWoods[1].SetActive(false);

        while (burningTime > 1) 
        {
            
            yield return null; 
        
        }
        embers.gameObject.SetActive(false);
        //fire.gameObject.SetActive(false);
        fireWoods[0].SetActive(false);

        while (burningTime > 0) 
        {
            audioSource.volume -= 0.0001f;
            yield return null; 
        }
        embers.gameObject.SetActive(false);
        fire.gameObject.SetActive(false);
        glow.gameObject.SetActive(false);
        embersSmall.gameObject.SetActive(false);
        fireLight.SetActive(false);
        audioSource.enabled = false;

        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material = new Material(Shader);
            meshRenderer[i].material.mainTexture = extinctTexture;
        }
        smokeTime = 0;
        while (smokeColor.a > 0)
        {
            smokeTime += Time.deltaTime;
            smokeColor.a -= 0.001f;
            smokeMain.startColor = smokeColor;
            colorA = smokeColor.a;
            yield return null;
        }
        //smoke.gameObject.SetActive(false);
    }


    void Update()
    {
        obstacle.radius = burningTime / 2;
        sphere.radius = burningTime / 1.9f + 1;
        
        campfirePos = transform.position;
        
        if (burningTime > 0) burningTime -= Time.deltaTime * time.timeFactor / 3600f;
        else burningTime = 0;
        timeHours = TimeSpan.FromHours(burningTime);

        fire.gameObject.transform.localScale = new Vector3(0.2f + 0.01f * burningTime, 0.2f + 0.01f * burningTime, 0.2f + 0.01f * burningTime);
        
        if (burningTime >= 0) fire.startLifetime = 0.3f + 0.1f * burningTime;
        else fire.startLifetime = 0;

        if (burningTime < 2) fire.startSize = burningTime;

        if (burningTime > 8)
        {
            fire.startLifetime = 0.8f;
        }

        if (burningTime <= 0)
        {
            burningTimeText = $"Костёр полностью догорел";
        }
        else if (burningTime < 0.5f) burningTimeText = $"Костёр догорает";
        else if (burningTime < 1.5f && burningTime > 0.5f)
            burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} час";
        else if (burningTime > 1.5f && burningTime < 4.5f)
            burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} часа";
        else if (burningTime > 4.5f) burningTimeText = $"Гореть будет примерно {Mathf.Round(burningTime)} часов";
        info.itemDescription = burningTimeText;
    }

    public void AddFireWood()
    {
        burningTime++;

        smokeColor.a = 1;
        smokeMain.startColor = smokeColor;

        for (int i = 0; i < fireWoods.Length; i++)
        {
            if (burningTime > i) fireWoods[i].SetActive(true);
        }

        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material = new Material(Shader);
            meshRenderer[i].material.mainTexture = burningTexture;
        }

        embers.gameObject.SetActive(true);
        fire.gameObject.SetActive(true);
        glow.gameObject.SetActive(true);
        embersSmall.gameObject.SetActive(true);
        smoke.gameObject.SetActive(true);

        fireMain.startSize = 2;

        StartCoroutine(burningWoodPerHour());
    }

    public void TriggerEnter(Collider other)
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

    public void TriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerWarmCollider col))
        {
            
            if (burningTime > 0)
            {
                //player.isCold = false;
                col.player.isCampfire = true;
                col.player.campfireTemperature = burningTime * 10 / distance;
            }
            else 
            {
                col.player.isCampfire = false;
                col.player.campfireTemperature = 0;
            }

            distance = Vector3.Distance(col.player.gameObject.transform.position, transform.position);
            if (distance != 0 && col.player.feelingTemperature > col.player.DayTime.freezeTemperature && col.player.Cold < col.player.maxCold)
            {
                warm += burningTime * warmAmount * Time.deltaTime / distance;
                if (warm > 1)
                {
                    col.player.Cold++; warm = 0; }
            }
        }
    }

    public void TriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerWarmCollider col))
        {
            col.player.isCampfire = false;
            col.player.campfireTemperature = 0;
        }
    }

}
