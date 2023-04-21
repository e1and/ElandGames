using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    public Action onCollectItem;

    public QuestInfo OpenedQuest;
    public QuestInfo FollowingQuest;

    public List<Quest> questList = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();
    public List<QuestInfo> questBlocksList = new List<QuestInfo>();
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

    public Quest Survive1Day;

    //[SerializeField] InventoryWindow inventoryWindow;
    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
    }
    void Start()
    {
        QuestUpdate();
        onCollectItem += OnCollectItem;
    }

    void OnCollectItem()
    {
        QuestItemsReCount();
        
    }

    public void QuestItemsReCount()
    {
        for (int i = 0; i < questBlocksList.Count; i++)
        {
            questBlocksList[i].QuestItemCount = 0;
            if (links.inventoryWindow.inventory != null)
            for (int j = 0; j < links.inventoryWindow.inventory.inventoryItems.Count; j++)
            {
                if (questList[i].QuestItem == links.inventoryWindow.inventory.inventoryItems[j])
                {
                    questBlocksList[i].QuestItemCount++;
                }
            }
            if (questList[i].QuestItem == links.inventoryWindow.LeftHandItem) questBlocksList[i].QuestItemCount++;
            if (questList[i].QuestItem == links.inventoryWindow.RightHandItem) questBlocksList[i].QuestItemCount++;

            if (questBlocksList[i].QuestItem != null)
            {
                if (questBlocksList[i].QuestItemCount >= questBlocksList[i].QuestItemNeed)
                {
                    questBlocksList[i].isComplete = true;
                    questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
                }
            }

        }
        QuestItemsCountRedraw();
    }

    public void QuestItemsCountRedraw()
    {
        if (OpenedQuest != null && OpenedQuest.QuestItem != null)
        QuestItemCountText.text = OpenedQuest.QuestItem.Name + " - " + OpenedQuest.QuestItemCount + "/" + OpenedQuest.QuestItemNeed;

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
                questBlocksList.Remove(questParent.GetChild(i).GetComponent<QuestInfo>());
                Destroy(questParent.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < questList.Count; i++)
        {
            var quest = Instantiate(questPrefab, questParent);
            quest.name = questList[i].Name;

            var questInfo = quest.GetComponent<QuestInfo>();
            questBlocksList.Add(questInfo);          

            questInfo.quest = questList[i];

            questInfo.questNameButtonText.text = questList[i].Name;

            questInfo.QuestItem = questList[i].QuestItem;
            questInfo.QuestItemNeed = questList[i].QuestItemNeed;

            questInfo.questWindow = this;

        }
        QuestItemsReCount();
    }

    public GameObject FollowingQuestPanel;

    public void FollowQuest()
    {
        if (FollowingQuestPanel.activeSelf) FollowingQuestPanel.SetActive(false);
        else FollowingQuestPanel.SetActive(true);

        FollowingQuest = OpenedQuest;

        UpdateFollowingQuest();
    }

    public void UpdateFollowingQuest()
    {
        FollowQuestNameText.text = OpenedQuest.quest.Name;
        FollowQuestBriefingText.text = OpenedQuest.quest.Briefing;
        if (FollowingQuest.QuestItem != null)
            FollowQuestBriefingText.text =
                FollowingQuest.quest.Briefing + "\n\n" + FollowingQuest.QuestItem.Name + ": "
                + FollowingQuest.QuestItemCount + "/" + FollowingQuest.QuestItemNeed;
    }  
    
    public void CompleteQuest()
    {
        if (OpenedQuest.quest.RewardItem != null && links.inventoryWindow.inventory != null)
        {
            if (links.inventoryWindow.inventory.filledSlots < links.inventoryWindow.inventory.size)
            {
                links.inventoryWindow.inventory.AddItem(OpenedQuest.quest.RewardItem);
            }
            else if (links.inventoryWindow.RightHandItem == null)
            {
                links.inventoryWindow.RightHandItem = OpenedQuest.quest.RewardItem;
            }
            else if (links.inventoryWindow.LeftHandItem == null)
            {
                links.inventoryWindow.LeftHandItem = OpenedQuest.quest.RewardItem;
            }
            else
            {
                links.mousePoint.Comment("Нет свободного места для награды!");
                return;
            }

        }

        links.player.survivalPoint += OpenedQuest.quest.RewardSkillPoint;
        links.player.UpdateSkillPoints();
        if (OpenedQuest.quest.RewardNextQuest != null) questList.Add(OpenedQuest.quest.RewardNextQuest);
        if (OpenedQuest.quest.RewardNewBlueprint != Blueprint.None) links.building.LearningBlueprint(OpenedQuest.quest.RewardNewBlueprint);

        questList.Remove(OpenedQuest.quest);
        completedQuests.Add(OpenedQuest.quest);

        QuestUpdate();
        links.inventoryWindow.Redraw();
        if (FollowingQuest == OpenedQuest) FollowingQuestPanel.SetActive(false);
        CompleteQuestButton.interactable = false;
        FollowQuestButton.interactable = false;
        DescriptionWindow.SetActive(false);
        FinalWindow.SetActive(true);
        FinalText.text = OpenedQuest.quest.FinalText;
        FinalRewards.text = OpenedQuest.quest.RewardText;
    }

    public void QuestDone(Quest quest)
    {
        if (questList.Contains(quest))
        {
            for (int i = 0; i < questBlocksList.Count; i++)
            {
                if (questBlocksList[i].quest == quest)
                {
                    questBlocksList[i].isComplete = true;
                    questBlocksList[i].checkMarkImage.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}
