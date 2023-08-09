public class QuestSurviveDays : Quest
{
    public override void SetQuestUnitsNeed()
    {
        QuestUnitsNeed = questData.questUnits;
        UpdateQuestUnits();
    }
    
    public override void UpdateQuestUnits()
    {
        if (questUI != null) questUI.questUnitsText.text = questHandler.links.dayNight.thisDay - 1 + " / " + questData.questUnits;
    }

    public override void SubscribeToEvents()
    {
        CheckDays();
    }

    public void CheckDays()
    {
         questHandler.links.dayNight.NewDayAction += QuestUnitDone;
    }
    
    public override void QuestUnitDone()
    {
        if (!isComplete)
        {
            QuestUnitsDone = questHandler.links.dayNight.thisDay - 1;
            UpdateQuestUnits();
            if (questUI != null) CheckQuestCondition();
        }
    }

    private void OnDisable()
    {
        questHandler.links.dayNight.NewDayAction -= QuestUnitDone;
    }
}

