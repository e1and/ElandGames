using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmZone : MonoBehaviour
{
    public int warmBonus = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            player.buildingTemperature = warmBonus;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            player.buildingTemperature -= warmBonus;
    }
}
