using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Attack : MonoBehaviour
{
    public Damage attackTrigger;
    
    public void Damage(int a)
    {
        if (a == 1) attackTrigger.box.enabled = true;
        else attackTrigger.box.enabled = false;
    }
}
