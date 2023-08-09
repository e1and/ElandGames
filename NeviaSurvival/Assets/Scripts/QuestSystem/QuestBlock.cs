using UnityEngine;
using UnityEngine.UI;

public class QuestBlock : MonoBehaviour
{
    public Quest quest;
    public Text questNameButtonText;
    public Image checkMarkImage;

    public void OpenQuest()
    {
        quest.OpenQuest();
    }
}
