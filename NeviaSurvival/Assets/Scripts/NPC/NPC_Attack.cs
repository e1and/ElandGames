using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Attack : MonoBehaviour
{
    public Damage damageTrigger;
    
    public void Damage(int a)
    {
        if (a == 1) damageTrigger.box.enabled = true;
        else damageTrigger.box.enabled = false;
    }
}
