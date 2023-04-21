using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Story/Quest")]
public class Quest : ScriptableObject
{
    public string Name;
    public string Briefing;
    public string Description;
    [Space]
    public Item QuestItem;
    public int QuestItemNeed;
    [Space]
    public string RewardText;
    public Quest RewardNextQuest;
    public Item RewardItem;
    public int RewardSkillPoint;
    public Blueprint RewardNewBlueprint;
    [Space]
    public string FinalText;
}
