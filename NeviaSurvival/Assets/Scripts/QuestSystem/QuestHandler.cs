using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestType { ExploreTarget, DestroyTarget, FindItem, OpenDoor, Enemies, DestroyObstacle, SurviveDays, Building, Sleep, Escort, Bag }
public enum QuestImageType { Explore, Destroy, FindItem, OpenDoor, Survive, Build, Sleep, Elf }
public enum EnemyType { None, Spider, Skeleton, Bear }

public class QuestHandler : MonoBehaviour
{
    public List<Quest> questList;
    public List<QuestData> completedQuests; 
    public List<QuestData> takenQuestList;
    public List<QuestData> startQuestList;
    public QuestData surviveDaysQuest;
    public List<QuestTarget> targetList;
    public Quest activeQuest;
    public Player player;
    public Transform questBar;
    public GameObject questPrefab;
    public QuestNotice questNotice;

    [SerializeField] Sprite DefaultQuestImage;
    [SerializeField] Sprite ExploreQuestImage;
    [SerializeField] Sprite FindItemQuestImage;
    [SerializeField] Sprite OpenDoorQuestImage;
    [SerializeField] Sprite SurviveQuestImage;
    [SerializeField] Sprite BuildQuestImage;
    [SerializeField] Sprite SleepQuestImage;
    [SerializeField] Sprite ElfImage;

    public GameObject levelUpVfx;
    public GameObject rewardVfx;
    public GameObject miniMapTarget;
    public GameObject miniMapArrow;
    public GameObject miniMapQuestArea;
    public float arrowDistance = 20;
    bool isQuestArrow;
    private QuestWindow questWindow;
    public Links links;

    private void Start()
    {
        questWindow = links.questWindow;
    }

    public async void AddStartQuests()
    {
        await UniTask.Delay(20000);
        foreach (QuestData questData in startQuestList)
        {
            AddQuest(questData);
            await UniTask.Delay(60000);
        }
    }

    public void AddQuest(QuestData questData)
    {
        if (!takenQuestList.Contains(questData))
        {
            takenQuestList.Add(questData);

            Quest newQuest = null;

            if (questData.questType == QuestType.ExploreTarget)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestExplore>();
            }
            else if (questData.questType == QuestType.FindItem)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestFindItem>();
            }
            else if (questData.questType == QuestType.OpenDoor)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestOpenDoor>();
            }
            else if (questData.questType == QuestType.DestroyTarget)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestDestroyTarget>();
            }
            else if (questData.questType == QuestType.SurviveDays)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestSurviveDays>();
            }
            else if (questData.questType == QuestType.Building)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestBuilding>();
            }
            else if (questData.questType == QuestType.Sleep)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestSleep>();
            }
            else if (questData.questType == QuestType.Escort)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestEscort>();
            }
            else if (questData.questType == QuestType.Bag)
            {
                newQuest = Instantiate(questPrefab, questBar).AddComponent<QuestBag>();
            }

            newQuest.questHandler = this;
            newQuest.questWindow = questWindow;
            newQuest.LinkUI();

            questList.Add(newQuest);
            newQuest.isFollowing = true;

            newQuest.SetQuestData(questData);
            newQuest.SetQuestImage(QuestImage(questData));

            newQuest.questBlock = questWindow.AddQuestBlock(newQuest);
            
            questNotice.ShowQuestNotice(questData);
            
            questWindow.QuestUpdate();
        }
    }

    public void SetActiveQuest(Quest quest)
    {
        activeQuest = quest;
        foreach (Quest _quest in questList) _quest.ActiveQuestVisual(false);
        quest.ActiveQuestVisual(true);
        if (quest.QuestTarget() != null && quest.QuestGiver() != null) SetMiniMapTarget(quest);
        else
        {
            miniMapTarget.SetActive(false);
            miniMapArrow.SetActive(false);
            miniMapQuestArea.SetActive(false);
        }
    }

    public void SetMiniMapTarget(Quest quest)
    {
        if (quest.isComplete)
        {
            miniMapTarget.transform.position = quest.QuestGiver().position;
            _ = QuestArrow(quest.QuestGiver(), quest);
        }
        else
        {
            miniMapTarget.transform.position = quest.QuestTarget().position;
            miniMapQuestArea.transform.position = new Vector3(quest.QuestTarget().position.x, 20, quest.QuestTarget().position.z);
            _ = QuestArrow(quest.QuestTarget(), quest);
        }       
    }

    async UniTask QuestArrow(Transform target, Quest quest)
    {
        miniMapArrow.SetActive(true);

        if (!quest.isComplete && quest.questData.questType is QuestType.OpenDoor or QuestType.Escort || quest.isComplete)
        miniMapTarget.SetActive(true);
        else miniMapTarget.SetActive(false);

        if (!quest.isComplete)
        {
            if (quest.questData.questAreaRadius > 0) miniMapQuestArea.SetActive(true);
            int radius = quest.QuestData().questAreaRadius;
            miniMapQuestArea.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }
        else miniMapQuestArea.SetActive(false);
        
        float distance;
        Quaternion _lookRotation;
        Vector3 _direction;

        while ((!activeQuest.isComplete && target == activeQuest.QuestTarget() ||
            !activeQuest.isRewarded && target == activeQuest.QuestGiver()) && activeQuest == quest)
        {
            if (activeQuest == null) break;
            
            distance = Vector3.Distance(player.transform.position, target.position);
            if (distance > 1f)
            {
                if (distance > arrowDistance)
                {
                    miniMapArrow.SetActive(true);
                    miniMapArrow.transform.position = player.transform.position +
                        (target.position - player.transform.position).normalized * arrowDistance;
                }
                else miniMapArrow.SetActive(false);

                _direction = (target.position - player.transform.position).normalized;
                _lookRotation = Quaternion.LookRotation(new Vector3(_direction.x, 0, _direction.z));
                miniMapArrow.transform.rotation = _lookRotation;
            }
            else
            {
                miniMapArrow.SetActive(false);
                miniMapTarget.SetActive(false);
                miniMapQuestArea.SetActive(false);
                break;
            }
            await UniTask.DelayFrame(2);
        }

        if (activeQuest.isRewarded)
        {
            miniMapArrow.SetActive(false);
            miniMapTarget.SetActive(false);
            miniMapQuestArea.SetActive(false);
        }
    }

    public void PlayVFX(GameObject vfx) => StartCoroutine(RewardVFXTimer(vfx));

    IEnumerator RewardVFXTimer(GameObject vfx)
    {
        float timer = 0;
        vfx.SetActive(true);
        while (timer < 3)
        { 
            levelUpVfx.transform.position = player.transform.position;
            yield return null;
            timer += Time.deltaTime;
        }
        vfx.SetActive(false);
    }

    public QuestTarget FindQuestTarget(QuestTargetPlace targetPlace)
    {
        foreach (QuestTarget questTarget in targetList)
        {
            if (questTarget.thisTargetPlace == targetPlace)
            {
                return questTarget;
            }
        }
        Debug.LogError("Target place must be added to QuestHandler target list!");
        return null;
    }

    public Quest GetQuestByQuestData(QuestData questDataData)
    {
        foreach (Quest quest in questList)
        {
            if (quest.questData == questDataData) return quest;   
        }
        return null;
    }
    
    public Sprite QuestImage(QuestData questData)
    {
        switch (questData.questImageType)
        {
            case QuestImageType.Explore:
                return ExploreQuestImage;
            case QuestImageType.FindItem:
                return FindItemQuestImage;
            case QuestImageType.OpenDoor:
                return OpenDoorQuestImage;
            case QuestImageType.Survive:
                return SurviveQuestImage;
            case QuestImageType.Build:
                return BuildQuestImage;
            case QuestImageType.Sleep:
                return SleepQuestImage;
            case QuestImageType.Elf:
                return ElfImage;

            default:
                return DefaultQuestImage;
        }
    }

    public void ActiveQuestSwitch()
    {
        int index = questList.IndexOf(activeQuest);
        if (index == questList.Count - 1) index = 0; else index++;
        SetActiveQuest(questList[index]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && questList.Count > 1) ActiveQuestSwitch();
    }
}
