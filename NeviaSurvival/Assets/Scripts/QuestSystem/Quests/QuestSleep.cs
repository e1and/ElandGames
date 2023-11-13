public class QuestSleep : Quest
{
    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }
    
    public override void UpdateQuestUnits()
    {
        if (questUI != null) questUI.questUnitsText.text = QuestUnitsDone + " / " + questData.questUnits + " часов";
    }

    public override void SubscribeToEvents()
    {
        CheckSleepTime();
    }

    public void CheckSleepTime()
    {
         questHandler.player.SleepForHourAction += QuestUnitDone;
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
        questHandler.player.SleepForHourAction -= QuestUnitDone;
    }
}

