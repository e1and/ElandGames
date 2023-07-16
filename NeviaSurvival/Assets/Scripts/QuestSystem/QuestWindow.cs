using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    public Action onCollectItem;

    public List<QuestData> allQuests = new List<QuestData>();
    public Dictionary<string, QuestData> allQuestList = new Dictionary<string, QuestData>();

    public Quest OpenedQuest;
    public Quest FollowingQuest;
    
    public List<Quest> questBlocksList = new List<Quest>();
    public GameObject questPrefab;
    public Transform questParent;

    public TMP_Text FollowQuestNameText;
    public TMP_Text FollowQuestBriefingText;
    public TMP_Text FollowQuestQuestItemText;

    
    public TMP_Text QuestNameText;
    public Text QuestBriefingText;
    public Text QuestDescriptionText;
    public Image QuestItemImage;
    public Text QuestItemCountText;
    public GameObject QuestDescriptionWindow;
    public Text QuestRewardText;
    public Button CompleteQuestButton;
    public Button FollowQuestButton;

    public GameObject DescriptionWindow;
    public GameObject FinalWindow;
    public Text FinalRewards;
    public Text FinalText;

    public QuestData Survive1Day;
    public QuestData Survive3Day;
    public QuestData Survive7Day;

    public Links links;
    private QuestHandler questHandler;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
        questHandler = links.questHandler;
        
        var quests = Resources.LoadAll("", typeof(QuestData));
        
        // Добавление всех квестов в список
        allQuests.Clear();
        allQuests.AddRange(Resources.LoadAll<QuestData>("ScriptableObjects"));

        // Если у квеста нет ID или он повторяется, то создаем его на основе названия квеста
        foreach (QuestData quest in allQuests)
        {
            if (allQuestList.ContainsKey(quest.id) || quest.id.Length == 0) quest.id = quest.name.Replace(" ", "");
            allQuestList.Add(quest.id, quest);
        }
    }
    void Start()
    {
        QuestUpdate();
        onCollectItem += OnCollectItem;
    }

    void OnCollectItem()
    {
        // Проверка квестовых предметов
        QuestItemsRecount(); 
    }

    public void QuestEventRecount()
    {
        for (int i = 0; i < questBlocksList.Count; i++)
        {
            // Проверка квеста прожитого времени
            if (questBlocksList[i].questData.isTime && questBlocksList[i].questData.days < links.dayNight.thisDay)
            QuestDone(questBlocksList[i].questData);
            
            // Проверка квеста открытой двери
            if (questBlocksList[i].questData.isOpenDoor && !QuestDoors[(int)questBlocksList[i].questData.door].isLocked)
                QuestDone(questBlocksList[i].questData);
        }
    }

    public void QuestItemsRecount()
    {
        for (int i = 0; i < questBlocksList.Count; i++)
        {
            questBlocksList[i].QuestUnitsDone = 0;
            if (links.inventoryWindow.inventory != null)
            for (int j = 0; j < links.inventoryWindow.inventory.inventoryItems.Count; j++)
            {
                if (questHandler.takenQuestList[i].QuestItem == links.inventoryWindow.inventory.inventoryItems[j])
                {
                    questBlocksList[i].QuestUnitsDone++;
                }
            }
            if (questHandler.takenQuestList[i].QuestItem == links.inventoryWindow.LeftHandItem) questBlocksList[i].QuestUnitsDone++;
            if (questHandler.takenQuestList[i].QuestItem == links.inventoryWindow.RightHandItem) questBlocksList[i].QuestUnitsDone++;

            if (questBlocksList[i].QuestItem != null)
            {
                if (questBlocksList[i].QuestUnitsDone >= questBlocksList[i].QuestUnitsNeed)
                {
                    questBlocksList[i].isComplete = true;
                    questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
                }
            }

        }
        QuestItemsCountRedraw();
        QuestStatusUpdate();
    }

    public void QuestItemsCountRedraw()
    {
        if (OpenedQuest != null && OpenedQuest.QuestItem != null)
        QuestItemCountText.text = OpenedQuest.QuestItem.Name + " - " + OpenedQuest.QuestUnitsDone + "/" + OpenedQuest.QuestUnitsNeed;

        if (FollowingQuest != null) UpdateFollowingQuest();

        if (OpenedQuest != null) CompleteButtonActivator();
    }

    public void CompleteButtonActivator()
    {
        if (OpenedQuest.isComplete) CompleteQuestButton.interactable = true;
        else CompleteQuestButton.interactable = false;
    }    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) { QuestUpdate();  }
    }

    public void QuestUpdate()
    {
        if (questParent.childCount > 0)
        {
            for (int i = 0; i < questParent.childCount; i++)
            {
                questBlocksList.Remove(questParent.GetChild(i).GetComponent<Quest>());
                Destroy(questParent.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < questHandler.takenQuestList.Count; i++)
        {
            var quest = Instantiate(questPrefab, questParent);
            quest.name = questHandler.takenQuestList[i].questName;

            var questInfo = quest.GetComponent<Quest>();
            questBlocksList.Add(questInfo);

            questInfo.questData = questHandler.takenQuestList[i];

            questInfo.questNameButtonText.text = questHandler.takenQuestList[i].questName;

            questInfo.QuestItem = questHandler.takenQuestList[i].QuestItem;
            questInfo.QuestUnitsNeed = questHandler.takenQuestList[i].QuestItemNeed;

            questInfo.questWindow = this;
        }

        links.ui.activeQuestCount.text = questHandler.takenQuestList.Count.ToString();
        if (questHandler.takenQuestList.Count > 0) links.ui.activeQuestCount.enabled = true;
        else links.ui.activeQuestCount.enabled = false;
        QuestItemsRecount();
        QuestItemsCountRedraw();
        QuestEventRecount();
    }

    public void QuestStatusUpdate()
    {
        for (int i = 0; i < questBlocksList.Count; i++)
        {
            if (questBlocksList[i].isComplete)
            {
                links.ui.questCompleteSign.SetActive(true);
                return;
            }
        }
        links.ui.questCompleteSign.SetActive(false);
    }

    public GameObject FollowingQuestPanel;

    public void FollowQuest()
    {
        if (FollowingQuestPanel.activeSelf && FollowingQuest != null && FollowingQuest == OpenedQuest) FollowingQuestPanel.SetActive(false);
        else FollowingQuestPanel.SetActive(true);

        FollowingQuest = OpenedQuest;

        UpdateFollowingQuest();
    }

    public void UpdateFollowingQuest()
    {
        FollowQuestNameText.text = OpenedQuest.questData.questName;
        FollowQuestBriefingText.text = OpenedQuest.questData.Briefing;
        if (FollowingQuest.QuestItem != null)
            FollowQuestBriefingText.text =
                FollowingQuest.questData.Briefing + "\n\n" + FollowingQuest.QuestItem.Name + ": "
                + FollowingQuest.QuestUnitsDone + "/" + FollowingQuest.QuestUnitsNeed;
    }  
    
    public void CompleteQuest()
    {
        if (OpenedQuest.questData.RewardItem != null && links.inventoryWindow.inventory != null)
        {
            if (links.inventoryWindow.inventory.filledSlots < links.inventoryWindow.inventory.size)
            {
                links.inventoryWindow.inventory.AddItem(OpenedQuest.questData.RewardItem);
            }
            else if (links.inventoryWindow.RightHandItem == null)
            {
                links.inventoryWindow.RightHandItem = OpenedQuest.questData.RewardItem;
            }
            else if (links.inventoryWindow.LeftHandItem == null)
            {
                links.inventoryWindow.LeftHandItem = OpenedQuest.questData.RewardItem;
            }
            else
            {
                links.mousePoint.Comment("Нет свободного места для награды!");
                return;
            }

        }

        links.player.survivalPoint += OpenedQuest.questData.RewardSkillPoint;
        links.player.UpdateSkillPoints();
        if (OpenedQuest.questData.rewardNextQuestData != null) questHandler.takenQuestList.Add(OpenedQuest.questData.rewardNextQuestData);
        if (OpenedQuest.questData.RewardNewBlueprint != Blueprint.None) links.building.LearningBlueprint(OpenedQuest.questData.RewardNewBlueprint);

        questHandler.takenQuestList.Remove(OpenedQuest.questData);
        questHandler.completedQuests.Add(OpenedQuest.questData);
        OpenedQuest.isComplete = false;

        QuestUpdate();
        links.inventoryWindow.Redraw();
        if (FollowingQuest == OpenedQuest) FollowingQuestPanel.SetActive(false);
        CompleteQuestButton.interactable = false;
        FollowQuestButton.interactable = false;
        DescriptionWindow.SetActive(false);
        FinalWindow.SetActive(true);
        FinalText.text = OpenedQuest.questData.FinalText;
        FinalRewards.text = OpenedQuest.questData.RewardText;
    }

    public void QuestDone(QuestData questData)
    {
        if (questHandler.takenQuestList.Contains(questData))
        {
            for (int i = 0; i < questBlocksList.Count; i++)
            {
                if (questBlocksList[i].questData == questData)
                {
                    questBlocksList[i].isComplete = true;
                    questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
                    break;
                }
            }
        }
        QuestStatusUpdate();
    }

    public List<Door> QuestDoors = new List<Door>();

}

public enum QuestDoor
{
    None = 0,
    BakerHouse = 1
}
