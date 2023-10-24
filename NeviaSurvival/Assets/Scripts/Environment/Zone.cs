using System;
using System.Collections;
using System.Collections.Generic;
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
                areaFog.SetActive(false);
                miniMapAreaFog.SetActive(false);

                if (exploreXP > 0)
                {
                    player.ChangeXP(exploreXP);
                    links.ui.ShowTextInARow(links.ui.gainXPText, null,
                        "Открыта новая территория: +" + exploreXP + " опыта", "");
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

        /*titleColor.a = 0;
        
        titleColor = links.ui.areaTitle.color;
        locationColor = links.ui.locationTitle.color;
        titleColor.a = 0;
        locationColor.a = 0;
        links.ui.areaTitle.color = titleColor;
        links.ui.locationTitle.color = locationColor;

        links.ui.areaTitle.text = title;
        links.ui.locationTitle.text = links.ui.locationTitles[location];
        while (titleColor.a < 1)
        {
            titleColor.a += 0.02f;
            locationColor.a += 0.02f;
            links.ui.areaTitle.color = titleColor;
            links.ui.locationTitle.color = locationColor;
            yield return null;
        }
        titleColor.a = 1;
        locationColor.a = 1;
        links.ui.areaTitle.color = titleColor;
        links.ui.locationTitle.color = locationColor;

        yield return new WaitForSeconds(2);

        while (titleColor.a > 0)
        {
            titleColor.a -= 0.02f;
            locationColor.a -= 0.02f;
            links.ui.areaTitle.color = titleColor;
            links.ui.locationTitle.color = locationColor;
            yield return null;
        }
        titleColor.a = 0;
        locationColor.a = 0;
        links.ui.areaTitle.color = titleColor;
        links.ui.locationTitle.color = locationColor;*/
    }
}
