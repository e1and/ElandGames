using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestHandler questHandler;
    public DialogueInterractor dialogueInterractor;
    public DialogueHandler dialogueHandler;
    public List<QuestData> givenQuestList;
    public GameObject questTarget;

    public QuestData testQuestData;
    public Branch testBranch;

    public QuestData IsCompletedQuest()
    {
        foreach (Quest quest in questHandler.questList)
        {
            if (quest.isComplete && givenQuestList.Contains(quest.QuestData()))
            {
                return quest.QuestData();
            }
        }
        return null;
    }

    public Quest IsRewardForQuest()
    {
        foreach (Quest quest in questHandler.questList)
        {
            if (quest.isComplete && givenQuestList.Contains(quest.QuestData()) && !quest.isRewarded)
            {
                return quest;
            }
        }
        return null;
    }

    public async void GiveQuest(QuestData questDataData, QuestData startQuestData)
    {
        questHandler.AddQuest(questDataData);
        givenQuestList.Add(questDataData);

        await UniTask.DelayFrame(1);
        foreach (Quest quest in questHandler.questList)
        {
            if (quest.QuestData() == questDataData && quest.gameObject.activeSelf)
            {
                quest.ConfigureQuest(questHandler.FindQuestTarget(questDataData.questTarget).gameObject, this);

                if (startQuestData != null) quest.SetRemainQuestChain(startQuestData);
                else if (questDataData.questChain.Count > 1)
                {
                    quest.SetRemainQuestChain(questDataData);
                    quest.SetAsStartQuest();      
                }
                if (questHandler.activeQuest == null || 
                   (questHandler.activeQuest != null && questHandler.activeQuest.isRewarded)) 
                    questHandler.SetActiveQuest(quest);
                break;
            }
        }
    }

    public void ContinueQuestChain(QuestData currentQuestData, QuestData startQuestData)
    {
        GiveQuest(currentQuestData, startQuestData);
    }

    public async void GiveReward(Quest quest)
    {
        questHandler.ActiveQuestSwitch();
        questHandler.questNotice.ShowQuestReward(quest.questData);
        quest.isRewarded = true;
        quest.gameObject.SetActive(false);
        questHandler.questList.Remove(quest);
        questHandler.RewardVFX();
        questHandler.completedQuests.Add(quest.questData);
        questHandler.takenQuestList.Remove(quest.questData);

        await UniTask.Delay(questHandler.questNotice.GetNoticeTime() * 500);
        questHandler.questNotice.CloseQuestRewardPanel();

        
    }

    public Quest QuestActive(QuestData questDataData)
    {
        foreach (Quest quest in questHandler.questList)
        {
            if (quest.QuestData() == questDataData)
            {
                return quest;
            }
        }
        return null;
    }

    public void AutoSelectNextQuest()
    {
        if (questHandler.questList.Count < 0)
        {
            foreach (Quest _quest in questHandler.questList)
            {
                if (_quest.gameObject.activeSelf)
                {
                    questHandler.SetActiveQuest(_quest);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) GiveQuest(testQuestData, null);
    }
}
