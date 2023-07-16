using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestExplore : Quest
{
    public override void SetQuestUnitsNeed()
    {
        questUI.questUnitsText.text = "Explore target!";
    }

    public override void SetQuestTarget(GameObject target)
    {
        base.SetQuestTarget(target);
    }

    public override void SubscribeToEvents()
    {
        
    }

    public override void CheckQuestCondition()
    {
        base.CheckQuestCondition();
        if (isComplete)
        {
            questUI.questUnitsText.text = "Return to " + questGiver.dialogueInterractor.npcName;
        }
    }

    private void OnDisable()
    {

    }

}

