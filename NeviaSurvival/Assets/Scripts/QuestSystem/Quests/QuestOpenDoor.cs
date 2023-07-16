using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestOpenDoor : Quest
{
    CampHandler questCamp;

    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
        if (questTarget.TryGetComponent(out CampHandler camp)) questCamp = camp;
    }

    public override void SubscribeToEvents()
    {
        CheckQuestEnemies();
    }

    public void CheckQuestEnemies()
    {
        if (questCamp != null) questCamp.itemFoundAction += QuestUnitDone;
    }

    public override void UpdateQuestUnits()
    {
        base.UpdateQuestUnits();
    }

    private void OnDisable()
    {
        if (questCamp != null && questData.questType == QuestType.Enemies)
            questCamp.itemFoundAction -= QuestUnitDone;
    }

}

