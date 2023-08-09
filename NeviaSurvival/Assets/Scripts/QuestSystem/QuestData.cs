using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Story/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public string id;
    public string Briefing;
    public QuestType questType;
    public QuestImageType questImageType;
    public QuestTargetPlace questTarget;
    public int questUnits; // num of enemies / num of props / num of items / distance to explore..
    public int questAreaRadius;
    
    [TextArea(10, 5)]
    public string Description;
    [Space]
    public Item QuestItem;
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
    public QuestDoor door;
    
    public EnemyType enemyType;

    public BuildingType buildingType;

    [Header("����� ������ ��� ���������� ������")]
    public string playerCompleteQuestPhrase;
    [Space(20)]
    public List<QuestData> questChain;
    [Header("����� ������ ��� ������� ������ �� ������� �������")]
    public List<string> playerCompleteQuestPhrases;
    [Space]
    public bool isRepeatable;
    public int timeToRepeat;
}
