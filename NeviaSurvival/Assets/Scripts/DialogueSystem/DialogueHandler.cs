using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour
{
    public DialogueInterractor activeDialogue;
    public QuestHandler questHandler;
    public CameraMouseDrag _camera;
    public CinemachineVirtualCamera virtualCamera;

    public GameObject dialoguePanel;
    public TMP_Text npcNameText;
    public Image npcImage;
    public TMP_Text phraseText;
    public GameObject answer1;
    public GameObject answer2;
    public GameObject answer3;
    public TMP_Text answer1Text;
    public TMP_Text answer2Text;
    public TMP_Text answer3Text;
    public GameObject buttonPanel;

    [SerializeField] private CinemachineVirtualCamera vCam;
    private CinemachineComponentBase componentBase;
    CinemachineFramingTransposer cameraFrame;
    float lastDistance;

    private void OnEnable()
    {
        //componentBase = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        //cameraFrame = (CinemachineFramingTransposer)componentBase;
        //lastDistance = ((CinemachineFramingTransposer)componentBase).m_CameraDistance;
    }

    public void DialogueCameraPosition()
    {
        //lastDistance = cameraFrame.m_CameraDistance;
        //cameraFrame.m_CameraDistance = 1;
    }

    public void DialogueCameraToNPC(DialogueInterractor npc)
    {
        //virtualCamera.Follow = npc.transform;
        //virtualCamera.gameObject.transform.eulerAngles = new Vector3(10, npc.transform.eulerAngles.y + 180, 0);
        //cameraFrame.m_ScreenY = 0.67f;
        //cameraFrame.m_ScreenX = 0.5f;
    }

    public void DialogueCameraToPlayer(DialogueInterractor npc)
    {
        //virtualCamera.Follow = questHandler.player.transform;  
        //cameraFrame.m_CameraDistance = 4;
        //cameraFrame.m_ScreenY = 0.5f;
        //cameraFrame.m_ScreenX = 0.5f;
        //virtualCamera.gameObject.transform.eulerAngles = new Vector3(0, questHandler.player.transform.eulerAngles.y + 180, 0);
    }

    public void LastCameraPosition()
    {
        //virtualCamera.Follow = questHandler.player.transform;
        //cameraFrame.m_CameraDistance = lastDistance;
        //cameraFrame.m_ScreenY = 0.5f;
        //cameraFrame.m_ScreenX = 0.5f;
        //virtualCamera.gameObject.transform.eulerAngles = new Vector3(35, questHandler.player.transform.eulerAngles.y, 0);
    }

    public void CloseDialogueButton()
    {
        activeDialogue.CloseDialogue();
    }
    
    public void SetPhrase(string phrase)
    {
        phraseText.text = phrase;
    }

    public void SetActiveDialogue(DialogueInterractor character)
    {
        activeDialogue = character;
    }

    public DialogueInterractor ActiveDialogue()
    {
        return activeDialogue;
    }

    public Branch CurrentBranch()
    {
        return activeDialogue.CurrentBranch();
    }

    public void GoToLink1()
    {
        activeDialogue.SetNextBranch(activeDialogue.CurrentBranch().link1);
    }

    public void GoToLink2()
    {
        activeDialogue.SetNextBranch(activeDialogue.CurrentBranch().link2);
    }

    public void GoToLink3()
    {
        activeDialogue.SetNextBranch(activeDialogue.CurrentBranch().link3);
    }

    public void SpellCharacterPhrase(Player player, string playerPhrase)
    {
        buttonPanel.SetActive(false);
        npcNameText.text = player.CharacterName;
        npcImage.sprite = player.CharacterIcon;
        dialoguePanel.SetActive(true);
        SetPhrase(playerPhrase);
        DialogueCameraPosition();
        StartCoroutine(AutoCloseCharacterPhrase());
    }

    public void CloseCharacterPhrase()
    {
        buttonPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        LastCameraPosition();
    }

    IEnumerator AutoCloseCharacterPhrase()
    {
        yield return new WaitForSeconds(4);
        CloseCharacterPhrase();
    }
}

