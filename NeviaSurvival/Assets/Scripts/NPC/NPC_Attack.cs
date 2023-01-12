using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Attack : MonoBehaviour
{
    public GameObject damageTrigger;
    
    public void Damage(int a)
    {
        if (a == 1) damageTrigger.SetActive(true);
        else damageTrigger.SetActive(false);
    }
}
