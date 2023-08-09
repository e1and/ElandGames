using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Quest : MonoBehaviour, IPointerClickHandler
{
    public QuestData questData;
    public QuestBlock questBlock;
    public GameObject questTarget;
    public QuestGiver questGiver;
    public bool isComplete;
    public bool isFollowing;
    public bool isPlayerInQuestArea;
    public bool isRewarded;
    public Item QuestItem;
    public int QuestUnitsDone;
    public int QuestUnitsNeed;
    public float distanceToTarget;
    [Space] public QuestWindow questWindow;

    public QuestHandler questHandler;

    public QuestUI questUI;

    public List<QuestData> remainQuestChain;

    QuestData _startQuestData;
    string questCompletePlayerPhrase = "Done!";

    public void OpenQuest()
    {
        questWindow.OpenedQuest = this;

        questWindow.QuestDescriptionWindow.SetActive(true);
        questWindow.QuestNameText.text = questData.questName;
        if (!isComplete || questGiver == null) questWindow.QuestBriefingText.text = questData.Briefing;
        else
            questWindow.QuestBriefingText.text =
                "Теперь нужно вернуться к " + questWindow.links.questHandler.GetQuestByQuestData(questData)
                    .questGiver.dialogueInterractor.npcName;

        questWindow.QuestDescriptionText.text = questData.Description;

        if (QuestItem != null)
        {
            questWindow.QuestItemImage.gameObject.SetActive(true);
            questWindow.QuestItemImage.sprite = QuestItem.Icon;
        }
        else questWindow.QuestItemImage.gameObject.SetActive(false);

        if (QuestItem != null && QuestUnitsNeed > 0)
            questWindow.QuestItemCountText.text = QuestItem.Name + " - " + QuestUnitsDone + "/" + QuestUnitsNeed;
        else questWindow.QuestItemCountText.text = "";

        questWindow.QuestRewardText.text = questData.RewardText;

        questWindow.FollowQuestButton.interactable = true;
        questWindow.DescriptionWindow.SetActive(true);
        questWindow.FinalWindow.SetActive(false);

        questWindow.CompleteButtonActivator();

    }

    public void LinkUI()
    {
        questUI = GetComponent<QuestUI>();
    }

    public QuestData QuestData() => questData;

    public virtual void SetQuestUnitsNeed()
    {
    }

    public virtual void SubscribeToEvents()
    {
    }

    public void SetQuestData(QuestData questData)
    {
        this.questData = questData;
        questCompletePlayerPhrase = this.questData.playerCompleteQuestPhrase;
        questUI.questText.text = this.questData.questName;
        questUI.questBriefingText.text = this.questData.Briefing;
        if (questData.questTarget != null)
        {
            questTarget = questHandler.FindQuestTarget(questData.questTarget).gameObject;
            CheckDistanceToQuestTarget();
        }
        else questUI.questDistanceText.enabled = false;
        SubscribeToEvents();
        SetQuestUnitsNeed();
        UpdateQuestUnits();
    }

    public void ConfigureQuest(GameObject target, QuestGiver giver)
    {
        SetQuestGiver(giver);
        SetQuestTarget(target);

        remainQuestChain = new List<QuestData>(0);
    }

    public void SetQuestGiver(QuestGiver newQuestGiver) => questGiver = newQuestGiver;

    public virtual void SetQuestTarget(GameObject target) => questTarget = target;

    public Transform QuestTarget()
    {
        if (questTarget != null) return questTarget.transform;
        return null;
    }

    public Transform QuestGiver()     
    {
        if (questGiver != null) return questGiver.transform;
        return null;
    }

    public void SetQuestImage(Sprite image) => questUI.questImage.sprite = image;

    public void SetAsStartQuest()
    {
        _startQuestData = questData;
    }

    public void SetRemainQuestChain(QuestData startQuestData)
    {
        _startQuestData = startQuestData;
        remainQuestChain = new List<QuestData>(startQuestData.questChain);

        if (remainQuestChain.Contains(questData))
        {
            int index = remainQuestChain.FindIndex(a => a == questData);
            for (int i = index; i >= 0; i--)
            {
                remainQuestChain.RemoveAt(i);
            }
            questCompletePlayerPhrase = startQuestData.playerCompleteQuestPhrases[index];
        }
    }

    public virtual void UpdateQuestUnits()
    {
        if (questUI != null) questUI.questUnitsText.text = QuestUnitsDone + " / " + questData.questUnits;
    }

    public virtual void QuestUnitDone()
    {
        if (!isComplete)
        {
            QuestUnitsDone++;
            UpdateQuestUnits();
            Debug.Log(questUI);
            if (questUI != null) CheckQuestCondition();
        }
    }

    public virtual void CheckQuestCondition()
    {
        if (QuestUnitsDone >= QuestUnitsNeed)
        {
            isComplete = true;
            questUI.questImage.sprite = questUI.questCompleteImage;
            questBlock.checkMarkImage.gameObject.SetActive(true);
            
            if (questCompletePlayerPhrase != null)
            questHandler.links.dialogueHandler.SpellCharacterPhrase(Game.Player, questCompletePlayerPhrase);

            if (questGiver != null)
            {
                if (questData.questChain.Count > 0)
                {
                    isRewarded = true;
                    questGiver.ContinueQuestChain(remainQuestChain[0], _startQuestData);
                    questWindow.CompleteQuest(this);
                }

                if (remainQuestChain.Count < 2) questHandler.SetActiveQuest(this);
            }
        }
    }

    public async void CheckDistanceToQuestTarget()
    {
        while (!isComplete)
        {
            distanceToTarget = Mathf.Round(Vector3.Distance(Game.Player.transform.position, questTarget.transform.position));
            questUI.questDistanceText.text = distanceToTarget + " m";

            if (questData.questType == QuestType.ExploreTarget)
            {
                QuestUnitsDone = -(int)distanceToTarget + questData.questUnits;
                CheckQuestCondition();
            }

            await UniTask.Delay(500);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        questWindow.links.questHandler.SetActiveQuest(this);
    }

    public void ActiveQuestVisual(bool isActive)
    {
        questUI.activeQuestBG.enabled = isActive;
    }
}
