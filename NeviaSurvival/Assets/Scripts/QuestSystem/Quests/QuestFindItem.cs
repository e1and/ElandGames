using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFindItem : Quest
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
    }

    public override void SubscribeToEvents()
    {
        
    }

    public void CheckQuestItems()
    {
        
    }

    public override void UpdateQuestUnits()
    {
        base.UpdateQuestUnits();
    }

    private void OnDisable()
    {

    }

}

