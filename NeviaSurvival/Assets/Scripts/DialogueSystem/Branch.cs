using UnityEngine;

[CreateAssetMenu(fileName = "Branch", menuName = "Dialogue Branch")]
public class Branch : ScriptableObject
{        
    [Header("�������� ����� ��� ������������ ������")]
    [TextArea(3, 3)]
    public string phrase;
    [Space]
    [Header("������ � ����� ������� �� ������� ��� �����")]
    [Header("���� ���� ������, �� ����� ������ ����� ��� �������")]
    public string answer1;
    public Branch link1;
    public string answer2;
    public Branch link2;
    public string answer3;
    public Branch link3;      
    [Space]
    [Header("�����, ������� �������� �� ���� �����")]
    public QuestData questData;
    [Header("����� ������� � ������� ����� ���������� ������")]
    public Branch nextQuestLink;
    [Header("����� ������� ���� ����� �� �����")]
    public Branch questNotReadyLink;
    [Header("����� ������� ���� ����� ��� �� ��������")]
    public Branch questNotCompleteLink;
    [Space]
    public bool isSelectableQuest;
    public Branch LinkToGetQuest;
}
