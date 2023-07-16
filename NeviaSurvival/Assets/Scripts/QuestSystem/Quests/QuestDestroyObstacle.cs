using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDestroyObstacle : Quest
{
    CampHandler questCamp;

    public override void SetQuestUnitsNeed()
    {
        if (questCamp != null)
        {
            QuestUnitsNeed = questCamp.PropsCount();
            UpdateQuestUnits();
        }
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
        if (questTarget.TryGetComponent(out CampHandler camp)) questCamp = camp;
    }

    public override void SubscribeToEvents()
    {
        CheckQuestProps();
    }

    public void CheckQuestProps()
    {
        if (questCamp != null) questCamp.propDestroyedAction += QuestUnitDone;
    }

    public override void UpdateQuestUnits()
    {
        questUI.questUnitsText.text = QuestUnitsDone + " / " + QuestUnitsNeed;
    }

    private void OnDisable()
    {
        if (questCamp != null && questData.questType == QuestType.DestroyObstacle)
            questCamp.propDestroyedAction -= QuestUnitDone;
    }

}

