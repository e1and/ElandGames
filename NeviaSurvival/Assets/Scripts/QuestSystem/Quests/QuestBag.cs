public class QuestBag : Quest
{
    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }
    
    public override void UpdateQuestUnits()
    {
        if (questUI != null) questUI.questUnitsText.text = QuestUnitsDone + " / " + questData.questUnits;
    }

    public override void SubscribeToEvents()
    {
        CheckBag();
    }

    public void CheckBag()
    {
         questHandler.links.inventoryWindow.BagEquipedAction += QuestUnitDone;
    }
    
    public override void QuestUnitDone()
    {
        if (!isComplete)
        {
            QuestUnitsDone++;
            UpdateQuestUnits();
            if (questUI != null) CheckQuestCondition();
        }
    }

    private void OnDisable()
    {
        questHandler.links.inventoryWindow.BagEquipedAction -= QuestUnitDone;
    }
}

