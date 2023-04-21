using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfo : MonoBehaviour
{
    public Quest quest;
    public bool isComplete;
    public bool isFollowing;
    public Item QuestItem;
    public int QuestItemCount;
    public int QuestItemNeed;
    [Space]
    public QuestWindow questWindow;
    public Text questNameButtonText;
    public Image checkMarkImage;

    public void OpenQuest()
    {
        questWindow.OpenedQuest = this;
        
        questWindow.QuestDescriptionWindow.SetActive(true);
        questWindow.QuestNameText.text = quest.Name;
        questWindow.QuestBriefingText.text = quest.Briefing;
        questWindow.QuestDescriptionText.text = quest.Description;
        
        if (QuestItem != null)
        {
            questWindow.QuestItemImage.gameObject.SetActive(true);
            questWindow.QuestItemImage.sprite = QuestItem.Icon;
        }
        else questWindow.QuestItemImage.gameObject.SetActive(false);
        
        if (QuestItemNeed > 0)
            questWindow.QuestItemCountText.text = QuestItem.Name + " - " + QuestItemCount + "/" + QuestItemNeed;
        else questWindow.QuestItemCountText.text = "";

        questWindow.QuestRewardText.text = quest.RewardText;

        questWindow.FollowQuestButton.interactable = true;
        questWindow.DescriptionWindow.SetActive(true);
        questWindow.FinalWindow.SetActive(false);

        questWindow.CompleteButtonActivator();

    }
}
