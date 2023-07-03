using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampTrigger : MonoBehaviour
{
    [SerializeField] Lamp lamp;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out NPC_Move enemy) && !enemy._isFearOfFire)
        {
            lamp.TurnOff();
        }
    }
}
