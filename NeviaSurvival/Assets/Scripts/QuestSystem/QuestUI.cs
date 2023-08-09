using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text questText;
    public TMP_Text questBriefingText;
    public TMP_Text questUnitsText;
    public TMP_Text questDistanceText;
    public Image questImage;
    public Sprite questCompleteImage;
    public Image activeQuestBG;
    public Image pointBG;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointBG.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointBG.enabled = false;
    }
}
