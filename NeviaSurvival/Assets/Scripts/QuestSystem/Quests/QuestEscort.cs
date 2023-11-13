using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class QuestEscort : Quest
{

    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
    }

    public override void SubscribeToEvents()
    {
        CheckQuestDistance();
    }

    public async void CheckQuestDistance()
    {
        await UniTask.DelayFrame(1);
        if (questGiver.TryGetComponent(out NPC_Move npc)) npc.EscortFinishedEvent += QuestUnitDone;
    }

    public override void UpdateQuestUnits()
    {
        if (questUI != null) questUI.questUnitsText.text = "";
    }

    private void OnDisable()
    {
        if (questGiver.TryGetComponent(out NPC_Move npc)) npc.EscortFinishedEvent -= QuestUnitDone;
    }

}

