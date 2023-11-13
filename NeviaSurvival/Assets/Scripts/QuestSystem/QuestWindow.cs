using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class QuestWindow : MonoBehaviour
{
    public Action onCollectItem;

    public List<QuestData> allQuests = new List<QuestData>();
    public Dictionary<string, QuestData> allQuestList = new Dictionary<string, QuestData>();

    public Quest OpenedQuest;
    public Quest FollowingQuest;
    
    //public List<Quest> questBlocksList = new List<Quest>(); // ???
    public GameObject questBlockPrefab;
    public Transform questBlocksParent;
    public Transform hiddenParent;

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
        onCollectItem += OnCollectItem;
    }

    public QuestBlock AddQuestBlock(Quest newQuest)
    {
        var quest = Instantiate(questBlockPrefab, questBlocksParent);
        quest.name = newQuest.questData.questName;
        Debug.Log(quest.name);

        var questBlock = quest.GetComponent<QuestBlock>();
        //questBlocksList.Add(questInfo);

        questBlock.quest = newQuest;
        questBlock.questNameButtonText.text = newQuest.questData.questName;

        return questBlock;
    }
    
    public void QuestUpdate()
    {
        // Обновляем счётчик активных квестов
        links.ui.activeQuestCount.text = questHandler.takenQuestList.Count.ToString();
        if (questHandler.takenQuestList.Count > 0) links.ui.activeQuestCount.enabled = true;
        else links.ui.activeQuestCount.enabled = false;
        
        QuestItemsRecount();
        QuestItemsCountRedraw();
    }

    public async void GetQuestBlockDelay(QuestData questData, QuestBlock questBlock)
    {
        await UniTask.DelayFrame(1);
        questHandler.GetQuestByQuestData(questData).questBlock = questBlock;
    }

    void OnCollectItem()
    {
        // Проверка квестовых предметов
        QuestItemsRecount(); 
    }

    public void QuestItemsRecount()
    {
        for (int i = 0; i < questHandler.questList.Count; i++)
        {
            if (questHandler.questList[i].questData.questType == QuestType.FindItem)
            {
                questHandler.questList[i].QuestUnitsDone = 0;

                if (questHandler.questList[i].questData.QuestItemType)
                {
                    if (links.inventoryWindow.inventory != null)
                        for (int j = 0; j < links.inventoryWindow.inventory.inventoryItems.Count; j++)
                        {
                            if (links.inventoryWindow.inventory.inventoryItems[j] != null && questHandler.takenQuestList[i].QuestItem.Type ==
                                links.inventoryWindow.inventory.inventoryItems[j].Type)
                            {
                                if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                            }
                        }

                    if (links.inventoryWindow.LeftHandItem != null && questHandler.takenQuestList[i].QuestItem.Type ==
                        links.inventoryWindow.LeftHandItem.Type)
                    {
                        if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                    }

                    if (links.inventoryWindow.RightHandItem != null && questHandler.takenQuestList[i].QuestItem.Type ==
                        links.inventoryWindow.RightHandItem.Type)
                    {
                        if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                    }
                }
                else
                {
                    if (links.inventoryWindow.inventory != null)
                        for (int j = 0; j < links.inventoryWindow.inventory.inventoryItems.Count; j++)
                        {
                            if (links.inventoryWindow.inventory.inventoryItems[j] != null && questHandler.takenQuestList[i].QuestItem ==
                                links.inventoryWindow.inventory.inventoryItems[j])
                            {
                                if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                            }
                        }

                    if (links.inventoryWindow.LeftHandItem != null && questHandler.takenQuestList[i].QuestItem ==
                        links.inventoryWindow.LeftHandItem)
                    {
                        if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                    }

                    if (links.inventoryWindow.RightHandItem != null && questHandler.takenQuestList[i].QuestItem ==
                        links.inventoryWindow.RightHandItem)
                    {
                        if (questHandler.questList[i] != null) questHandler.questList[i].QuestUnitDone();
                    }
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
        if (OpenedQuest.isComplete && !OpenedQuest.isQuestGiverToFinish) CompleteQuestButton.interactable = true;
        else CompleteQuestButton.interactable = false;
    }    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) { QuestUpdate();  }
    }

    public void QuestStatusUpdate()
    {
        for (int i = 0; i < questHandler.questList.Count; i++)
        {
            if (questHandler.questList[i].isComplete)
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

    public void CompleteOpenedQuest()
    {
        CompleteQuest(OpenedQuest);
    }

    public void FollowOpenedQuest()
    {
        if (OpenedQuest.isFollowing)
        {
            OpenedQuest.transform.parent = hiddenParent;
            OpenedQuest.transform.position = hiddenParent.transform.position;
            OpenedQuest.isFollowing = false;

        }
        else
        {
            OpenedQuest.transform.parent = questHandler.questBar;
            OpenedQuest.isFollowing = true;
        }
    }
    
    public void CompleteQuest(Quest quest)
    {
        Debug.Log("Quest Complete");
        if (quest.questData.RewardItem != null)
        {
            if (links.inventoryWindow.inventory != null && links.inventoryWindow.inventory.filledSlots < links.inventoryWindow.inventory.size)
            {
                links.inventoryWindow.inventory.AddItem(quest.questData.RewardItem);
            }
            else if (links.inventoryWindow.RightHandItem == null)
            {
                links.inventoryWindow.RightHandItem = quest.questData.RewardItem;
            }
            else if (links.inventoryWindow.LeftHandItem == null)
            {
                links.inventoryWindow.LeftHandItem = quest.questData.RewardItem;
            }
            else
            {
                links.mousePoint.Comment("Нет свободного места для награды!");
                return;
            }
        }

        // Получение очков навыков и опыта
        links.player.skillPoint += quest.questData.RewardSkillPoint;
        links.player.UpdateSkillPoints();
        links.player.ChangeXP(quest.questData.RewardXP, "Квест завершен:");
        
        // Получение нового квеста
        if (quest.questData.rewardNextQuestData != null) NewQuestDelay(quest);
        
        // Получение нового чертежа
        if (quest.questData.RewardNewBlueprint != Blueprint.None) links.buildingHandler.LearningBlueprint(quest.questData.RewardNewBlueprint);
        
        questHandler.questList.Remove(quest);
        questHandler.takenQuestList.Remove(quest.questData);
        questHandler.completedQuests.Add(quest.questData);
        quest.isComplete = false;
        
        if (FollowingQuest == quest) FollowingQuestPanel.SetActive(false);
        CompleteQuestButton.interactable = false;
        FollowQuestButton.interactable = false;
        DescriptionWindow.SetActive(false);
        FinalWindow.SetActive(true);
        FinalText.text = quest.questData.FinalText;
        FinalRewards.text = quest.questData.RewardText;
        
        if (OpenedQuest == quest) OpenedQuest = null;
        Destroy(quest.questBlock.gameObject);
        Destroy(quest.gameObject);
        
        RedrawDelay();
    }

    async void NewQuestDelay(Quest quest)
    {
        await UniTask.Delay(3000);
        questHandler.AddQuest(quest.questData.rewardNextQuestData);
    }

    async void RedrawDelay()
    {
        await UniTask.DelayFrame(1);
        links.inventoryWindow.Redraw();
        QuestUpdate();
    }

    public List<Door> QuestDoors = new List<Door>();

}

public enum QuestDoor
{
    None = 0,
    BakerHouse = 1,
    GraveYardDoor = 2
}
