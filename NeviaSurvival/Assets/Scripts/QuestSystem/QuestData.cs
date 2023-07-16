using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Story/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public string id;
    public QuestType questType;
    public QuestImageType questImageType;
    public QuestTargetPlace questTarget;
    public int questAreaRadius;
    public string Briefing;
    [TextArea(10, 5)]
    public string Description;
    [Space]
    public Item QuestItem;
    public int QuestItemNeed;
    [Space]
    [TextArea()]
    public string RewardText;
    public QuestData rewardNextQuestData;
    public Item RewardItem;
    public int RewardSkillPoint;
    public Blueprint RewardNewBlueprint;
    [Space]
    [TextArea(10, 5)]
    public string FinalText;
    [Space] 
    public bool isTime;
    public int days;
    [Space]
    public bool isOpenDoor;
    public QuestDoor door;
    

    public EnemyType enemyType;
    public int questUnits; // num of enemies / num of props / num of items / distance to explore..
    [Header("Фраза игрока для одиночного квеста")]
    public string playerCompleteQuestPhrase;
    [Space(20)]
    public List<QuestData> questChain;
    [Header("Фразы игрока для каждого квеста из цепочки квестов")]
    public List<string> playerCompleteQuestPhrases;
    [Space]
    public bool isRepeatable;
    public int timeToRepeat;
}
