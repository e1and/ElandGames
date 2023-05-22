using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDayNumber : MonoBehaviour
{
    Links links;
    void Awake()
    {
        links = FindObjectOfType<Links>();
    }


    void Update()
    {
        
    }

    public void Show()
    {
        dayColor.a = 0;
        links.dayNight.dayIndicator.color = dayColor;
        StopAllCoroutines();
        StartCoroutine(ShowThisDayNumber());
    }

    public Color dayColor;
    public IEnumerator ShowThisDayNumber()
    {
        dayColor.a = 0;

        Debug.Log("Show day number " + links.dayNight.thisDay);
        yield return new WaitForSeconds(3);

        dayColor = links.dayNight.dayIndicator.color;
        dayColor.a = 0;
        links.dayNight.dayIndicator.color = dayColor;

        links.dayNight.dayIndicator.text = "Δενό " + links.dayNight.thisDay;
        while (dayColor.a < 1)
        {
            //if (links.player.isDead)
                dayColor.a += 0.02f;
            links.dayNight.dayIndicator.color = dayColor;
            yield return null;
        }
        dayColor.a = 1;
        links.dayNight.dayIndicator.color = dayColor;

        yield return new WaitForSeconds(2);

        while (dayColor.a > 0)
        {
            dayColor.a -= 0.02f;
            links.dayNight.dayIndicator.color = dayColor;
            yield return null;
        }
        dayColor.a = 0;
        links.dayNight.dayIndicator.text = "";
    }
}
