using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public GameObject areaFog;
    public GameObject miniMapAreaFog;
    Links links;

    public string title;
    public int location;
    public bool isExplored;
    public int exploreXP = 1;
    
    public Color titleColor;
    public Color locationColor;
    
    void Awake()
    {
        links = FindObjectOfType<Links>();
        titleColor = links.ui.areaTitle.color;
        locationColor = links.ui.locationTitle.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (!isExplored)
            {
                if (areaFog != null) areaFog.SetActive(false);
                if (miniMapAreaFog != null) miniMapAreaFog.SetActive(false);

                if (exploreXP > 0)
                {
                    player.ChangeXP(exploreXP, "Открыта новая территория:");
                    links.sounds.ExploreSound();
                }

                isExplored = true;
            }
            player.ChangeZone(this);
        }
    }

    public void Show()
    {
        titleColor.a = 0;

        StopAllCoroutines();
        StartCoroutine(ShowAreaTitle());
    }

    public IEnumerator ShowAreaTitle()
    {
        links.ui.ShowTextInARow(links.ui.areaTitle, links.ui.locationTitle, title, links.ui.locationTitles[location]);
        yield return null;
    }
}
