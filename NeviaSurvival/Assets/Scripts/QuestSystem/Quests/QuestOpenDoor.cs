using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOpenDoor : Quest
{
    Door questDoor;

    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
        if (questTarget.TryGetComponent(out Door door)) questDoor = door;
    }

    public override void SubscribeToEvents()
    {
        CheckDoorState();
    }

    public void CheckDoorState()
    {
        if (questDoor != null) questDoor.doorOpenedAction += QuestUnitDone;
    }

    public override void UpdateQuestUnits()
    {
        base.UpdateQuestUnits();
    }

    private void OnDisable()
    {
        if (questDoor != null) questDoor.doorOpenedAction -= QuestUnitDone;
    }

}

