using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    public StepSound stepSound;
    private void OnTriggerEnter(Collider other)
    {
        stepSound.Step();
    }
}
