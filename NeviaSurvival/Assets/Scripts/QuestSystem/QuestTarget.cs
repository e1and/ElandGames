using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    public QuestTargetPlace thisTargetPlace;

    public Action targetDestroyedAction;

    private void OnDisable()
    {
        targetDestroyedAction?.Invoke();
    }

    private void OnDestroy()
    {
        targetDestroyedAction?.Invoke();
    }
}