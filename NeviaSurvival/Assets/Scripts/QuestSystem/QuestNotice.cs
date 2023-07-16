using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotice : MonoBehaviour
{
    [SerializeField] TMP_Text questText;
    [SerializeField] TMP_Text questDescriptionText;
    [SerializeField] Image questImage;
    [SerializeField] GameObject questNoticePanel;
    [SerializeField] GameObject rewardNoticePanel;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] TMP_Text rewardDescriptionText;
    [SerializeField] int noticeTime = 5;
    [SerializeField] QuestHandler questHandler;
    
    private void Start()
    {
        questNoticePanel.SetActive(false);
        rewardNoticePanel.SetActive(false);
    }

    public void ShowQuestNotice(QuestData questData)
    {
        questText.text = questData.questName;
        questDescriptionText.text = questData.Description;
        questImage.sprite = questHandler.QuestImage(questData);
        StartCoroutine(ShowQuestNoticePanel());
    }

    IEnumerator ShowQuestNoticePanel()
    {
        questNoticePanel.SetActive(true);
        yield return new WaitForSeconds(noticeTime);
        questNoticePanel.SetActive(false);
    }

    public void ShowQuestReward(QuestData questData)
    {
        rewardText.text = questData.RewardText;
        rewardNoticePanel.SetActive(true);
    }

    public void CloseQuestRewardPanel()
    {
        rewardNoticePanel.SetActive(false);
    }

    public int GetNoticeTime()
    {
        return noticeTime;
    }    
}

