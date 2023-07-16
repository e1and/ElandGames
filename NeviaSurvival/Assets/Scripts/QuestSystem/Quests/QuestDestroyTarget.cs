using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDestroyTarget : Quest
{
    QuestTarget _questTarget;

    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = 1;
        UpdateQuestUnits();
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
        _questTarget = questTarget.GetComponent<QuestTarget>();
    }

    public override void SubscribeToEvents()
    {
        CheckQuestProps();
    }

    public void CheckQuestProps()
    {
        _questTarget.targetDestroyedAction += QuestUnitDone;
    }

    public override void UpdateQuestUnits()
    {
        questUI.questUnitsText.text = QuestUnitsDone + " / " + QuestUnitsNeed;
    }

    private void OnDisable()
    {

            _questTarget.targetDestroyedAction -= QuestUnitDone;
    }

}

