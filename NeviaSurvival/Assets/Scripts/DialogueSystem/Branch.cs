using UnityEngine;

[CreateAssetMenu(fileName = "Branch", menuName = "Dialogue Branch")]
public class Branch : ScriptableObject
{        
    [Header("Основная фраза или однострочный вопрос")]
    [TextArea(3, 3)]
    public string phrase;
    [Space]
    [Header("Ответы и ветки диалога на которые они ведут")]
    [Header("если поля пустые, то будет просто фраза без ответов")]
    public string answer1;
    public Branch link1;
    public string answer2;
    public Branch link2;
    public string answer3;
    public Branch link3;      
    [Space]
    [Header("Квест, который выдается на этой фразе")]
    public QuestData questData;
    [Header("Ветка диалога к которой ведет выполнение квеста")]
    public Branch questGoToLink;
    [Header("Ветка диалога если квест еще не выполнен")]
    public Branch questNotCompleteLink;

    [Space] [Header("Ветка диалога если квест не готов")]
    public bool isRepeatable;
    public Branch questNotReadyLink;
    [Space]
    public bool isSelectableQuest;
    public Branch LinkToGetQuest;
    public bool isCloseDialogue;
    public bool isEscort;
}
