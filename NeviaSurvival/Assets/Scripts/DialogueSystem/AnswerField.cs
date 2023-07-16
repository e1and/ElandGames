using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image bg;
    [SerializeField] int index;
    [SerializeField] DialogueHandler dialogue;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (index == 1 && dialogue.CurrentBranch().link1 != null)
            dialogue.GoToLink1();
        else if (index == 2 && dialogue.CurrentBranch().link2 != null)
            dialogue.GoToLink2();
        else if (index == 3 && dialogue.CurrentBranch().link3 != null)
            dialogue.GoToLink3();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bg.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bg.enabled = false;
    }
}
