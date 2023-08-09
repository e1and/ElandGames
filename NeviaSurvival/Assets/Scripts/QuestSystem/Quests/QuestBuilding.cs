public class QuestBuilding : Quest
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
        CheckBuilding();
    }

    public void CheckBuilding()
    {
        switch (questData.buildingType)
        {
            case BuildingType.Campfire:
                questHandler.links.buildingHandler.BuildCampfireAction += QuestUnitDone;
                break;
            case BuildingType.GrassBed:
                questHandler.links.buildingHandler.BuildGrassBedAction += QuestUnitDone;
                break;
            case BuildingType.Fence:
                questHandler.links.buildingHandler.BuildFenceAction += QuestUnitDone;
                break;
        }
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
        switch (questData.buildingType)
        {
            case BuildingType.Campfire:
                questHandler.links.buildingHandler.BuildCampfireAction -= QuestUnitDone;
                break;
            case BuildingType.GrassBed:
                questHandler.links.buildingHandler.BuildGrassBedAction -= QuestUnitDone;
                break;
            case BuildingType.Fence:
                questHandler.links.buildingHandler.BuildFenceAction -= QuestUnitDone;
                break;
        }
    }
}

