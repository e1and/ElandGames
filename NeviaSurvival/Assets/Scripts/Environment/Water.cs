using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    Links links;
    private void Start()
    {
        links = FindObjectOfType<Links>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Swimming swimming))
        {
            swimming.SwimmingStateSwitcher(true);
            swimming.waterY = other.gameObject.transform.position.y;
        }
        if (other.TryGetComponent(out OxygenDetector oxygen))
        {
            oxygen.GetComponentInParent<Player>().isUnderWater = true;
        }
        if (other.TryGetComponent(out WaterDetector water))
        {
            water.GetComponentInParent<Player>().isAbleToCollectWater = true;
            if (links.mousePoint.pointedIcon != null && links.mousePoint.pointedIcon.item.Type == ItemType.Cauldron)
                links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Swimming swimming))
        {
            swimming.SwimmingStateSwitcher(false);
        }
        if (other.TryGetComponent(out OxygenDetector oxygen))
        {
            oxygen.GetComponentInParent<Player>().isUnderWater = false;
        }
        if (other.TryGetComponent(out WaterDetector water))
        {
            water.GetComponentInParent<Player>().isAbleToCollectWater = false;
            if (links.mousePoint.pointedIcon != null && links.mousePoint.pointedIcon.item.Type == ItemType.Cauldron)
                links.mousePoint.pointedIcon.gameObject.GetComponent<DescribeUI>().EnterUI();
        }
    }
}
