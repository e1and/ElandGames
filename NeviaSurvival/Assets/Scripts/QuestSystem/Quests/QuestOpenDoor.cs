using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QuestOpenDoor : Quest
{
    public Door questDoor;

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

    public async void CheckDoorState()
    {
        await UniTask.DelayFrame(2);
        if (questDoor != null)
        {
            questDoor.doorOpenedAction += QuestUnitDone;
        }
        else Debug.LogError("Не подписан на событие двери");
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

