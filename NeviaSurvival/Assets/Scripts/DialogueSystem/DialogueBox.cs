using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueBox : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DialogueHandler dialogue;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dialogue.activeDialogue == null) dialogue.CloseCharacterPhrase();
        else if (dialogue.CurrentBranch().answer1.Length == 0 && dialogue.CurrentBranch().link1 != null)
            dialogue.GoToLink1();
        Debug.Log("111");
    }
}
