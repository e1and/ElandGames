using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Story/Quest")]
public class Quest : ScriptableObject
{
    public string Name;
    public string id;
    public string Briefing;
    [TextArea(10, 5)]
    public string Description;
    [Space]
    public Item QuestItem;
    public int QuestItemNeed;
    [Space]
    [TextArea()]
    public string RewardText;
    public Quest RewardNextQuest;
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
}
