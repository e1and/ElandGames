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
    [Space]
    public QuestWindow questWindow;
    public Text questNameButtonText;
    public Image checkMarkImage;

    
    public QuestUI questUI;

    public List<QuestData> remainQuestChain;

    QuestData _startQuestData;
    string questCompletePlayerPhrase = "Done!";

    public void OpenQuest()
    {
        questWindow.OpenedQuest = this;
        
        questWindow.QuestDescriptionWindow.SetActive(true);
        questWindow.QuestNameText.text = questData.questName;
        if (!isComplete) questWindow.QuestBriefingText.text = questData.Briefing;
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
        
        if (QuestUnitsNeed > 0)
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

    public virtual void SetQuestUnitsNeed() { }

    public virtual void SubscribeToEvents() { }

    public void SetQuestData(QuestData questData)
    {
        this.questData = questData;
        questCompletePlayerPhrase = this.questData.playerCompleteQuestPhrase;
        questUI.questText.text = this.questData.questName;
        UpdateQuestUnits();  
    }
    
    public void ConfigureQuest(GameObject target, QuestGiver giver)
    {
        SetQuestGiver(giver);
        SetQuestTarget(target);
        CheckDistanceToQuestTarget();
        SubscribeToEvents();
        SetQuestUnitsNeed();
        remainQuestChain = new List<QuestData>(0);
    }
    
    public void SetQuestGiver(QuestGiver newQuestGiver) => questGiver = newQuestGiver;
    
    public virtual void SetQuestTarget(GameObject target) => questTarget = target;

    public Transform QuestTarget() => questTarget.transform;
    public Transform QuestGiver() => questGiver.transform;

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

    public void QuestUnitDone()
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

            questGiver.dialogueHandler.SpellCharacterPhrase(Game.Player, questCompletePlayerPhrase);

            if (questData.questChain.Count > 0)
            {
                isRewarded = true;
                questGiver.ContinueQuestChain(remainQuestChain[0], _startQuestData);
                gameObject.SetActive(false);
                questGiver.questHandler.questList.Remove(this);
            }

            if (remainQuestChain.Count < 2) questGiver.questHandler.SetActiveQuest(this);
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
        questGiver.questHandler.SetActiveQuest(this);
    }

    public void ActiveQuestVisual(bool isActive)
    {
        questUI.activeQuestBG.enabled = isActive;
    }
}
