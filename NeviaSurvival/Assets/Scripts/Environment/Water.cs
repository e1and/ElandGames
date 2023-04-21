using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
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
    }
}
