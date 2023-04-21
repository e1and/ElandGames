using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestTrigger : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out RestZone restZone))
        {
            player.RestZone(restZone.RestPower);
        }
    }
}
