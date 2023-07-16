using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueInterractor : MonoBehaviour
{
    public string npcName;
    public string NpcName { get => npcName; }

    bool isDialogue;

    [SerializeField] Sprite npcAvatar;
    [SerializeField] DialogueHandler dialogueHandler;
    [SerializeField] QuestGiver questGiver;
    [SerializeField] Branch startBranch;
    Branch currentBranch;
    Branch nextQuestBranch;

    public Branch gettingQuestBranch;
    public Branch selectableQuestBranch;

    public Player player;
    public NPC_Move npcMove;

    [SerializeField] float RotationSpeed = 10;
    float dialogueCooldown;

    public UnityAction ChooseAnswer;


    private void Start()
    {
        CloseDialogue();
        currentBranch = startBranch;

        dialogueHandler.answer1.SetActive(false);
        dialogueHandler.answer2.SetActive(false);
        dialogueHandler.answer3.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _player) && !isDialogue && dialogueCooldown <= 0)
        {
            Quest quest = questGiver.IsRewardForQuest();

            if (quest != null)
            {
                questGiver.GiveReward(quest);

                if (quest.QuestData().isRepeatable)
                {
                    TimeToRepeat(quest.QuestData().timeToRepeat, quest.questData);
                }

                if (nextQuestBranch != null)
                {
                    SetNextBranch(nextQuestBranch);
                }
            }
            player = _player;
            isDialogue = true;
            OpenDialogue();
            
            _ = RotateToPlayer(_player.transform);
        }
    }

    public async void TimeToRepeat(int time, QuestData questData)
    {
        Debug.Log("Timer started");
        await UniTask.Delay(time * 1000);
        questGiver.questHandler.takenQuestList.Remove(questData);
        Debug.Log("Quest remived");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _player) && isDialogue)
        {
            CloseDialogue();
            player = null;
            isDialogue = false;
        }
    }

    async UniTask RotateToPlayer(Transform playerTransform)
    {
        Vector3 _direction;
        Vector3 _directionXZ;
        Quaternion _lookRotation;

        while (player != null)
        {
            _direction = playerTransform.position - transform.position;
            _directionXZ.x = _direction.x; _directionXZ.z = _direction.z; _directionXZ.y = 0;
            _lookRotation = Quaternion.LookRotation(_directionXZ.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * RotationSpeed);
            await UniTask.DelayFrame(1);
            
            if (_direction.sqrMagnitude > 25) break;
        }
    }
    
    void OpenDialogue()
    {
        dialogueHandler.SetActiveDialogue(this);
        dialogueHandler.npcNameText.text = npcName;
        dialogueHandler.npcImage.sprite = npcAvatar;
        dialogueHandler.dialoguePanel.SetActive(true);
        dialogueHandler.SetPhrase(currentBranch.phrase);
        dialogueHandler.DialogueCameraPosition();
        dialogueHandler.DialogueCameraToNPC(this);
        dialogueHandler._camera.enabled = false;
        player.isControl = false;
        npcMove.StopMove();
    }

    public void CloseDialogue()
    {
        isDialogue = false;
        dialogueHandler.SetActiveDialogue(null);
        dialogueHandler.dialoguePanel.SetActive(false);
        dialogueHandler._camera.enabled = true;
        if (player != null)
        {
            player.isControl = true;
            dialogueHandler.LastCameraPosition();
            StartCoroutine(Cooldown());
        }
        npcMove.ContinueMove();
    }

    IEnumerator Cooldown()
    {
        dialogueCooldown = 1;
        while (dialogueCooldown > 0)
        {
            if (questGiver.questHandler.player.isControl) dialogueCooldown -= Time.deltaTime;
            yield return null;
        }
    }

    public Branch CurrentBranch()
    {
        return currentBranch;
    }

    public void SetNextBranch(Branch nextBranch)
    {
        if (nextBranch.questData != null)
        {
            if (!questGiver.questHandler.takenQuestList.Contains(nextBranch.questData))
            {
                if (!nextBranch.isSelectableQuest)
                {
                    Branch branch = nextBranch;
                    QuestData currentQuestData = nextBranch.questData;

                    if (gettingQuestBranch != null && gettingQuestBranch == nextBranch)
                    {
                        branch = selectableQuestBranch;
                        currentQuestData = selectableQuestBranch.questData;
                        if (nextBranch.questData != null) Debug.LogError("В ветке выбираемого ответа не нужно указывать квест, сработает тот, что указана в isSelectableQuest ветке");
                    }

                    questGiver.GiveQuest(currentQuestData, null);
                    nextQuestBranch = branch.nextQuestLink;
                }
                else if (nextBranch.LinkToGetQuest != null && gettingQuestBranch == null)
                {
                    gettingQuestBranch = nextBranch.LinkToGetQuest;
                    selectableQuestBranch = nextBranch;
                }

                currentBranch = nextBranch;

            }
            else
            {
                Quest quest = questGiver.questHandler.GetQuestByQuestData(nextBranch.questData);
                if (quest != null && !quest.isComplete)
                {
                    if (nextBranch.questNotCompleteLink != null)
                    {
                        if (nextBranch.questNotCompleteLink.nextQuestLink != null)
                            nextQuestBranch = nextBranch.questNotCompleteLink.nextQuestLink;

                        currentBranch = nextBranch.questNotCompleteLink;
                    }
                    else
                    {
                        currentBranch = nextBranch;
                    }
                }
                else
                {
                    if (nextBranch.questNotReadyLink == null) 
                        Debug.LogError("Для неготового повторяющегося квеста нужна альтернативная ветка диалога!");
                    else if (nextBranch.questNotReadyLink.questData != null)
                        questGiver.GiveQuest(nextBranch.questNotReadyLink.questData, null);

                    if (nextBranch.questNotReadyLink.nextQuestLink != null)
                        nextQuestBranch = nextBranch.questNotReadyLink.nextQuestLink;

                    currentBranch = nextBranch.questNotReadyLink;
                }
            }
        }
        else
        {
            if (gettingQuestBranch != null && nextBranch == gettingQuestBranch)
            {
                questGiver.GiveQuest(selectableQuestBranch.questData, null);
                nextQuestBranch = selectableQuestBranch.nextQuestLink;
            }
            currentBranch = nextBranch;
        }
        dialogueHandler.SetPhrase(currentBranch.phrase);

        dialogueHandler.answer1.SetActive(false);
        dialogueHandler.answer2.SetActive(false);
        dialogueHandler.answer3.SetActive(false);

        if (currentBranch.answer1.Length > 0) dialogueHandler.DialogueCameraToPlayer(this);
        else dialogueHandler.DialogueCameraToNPC(this);

        if (currentBranch.answer1.Length > 0)
        {
            
            dialogueHandler.answer1.SetActive(true);
            dialogueHandler.answer1Text.text = currentBranch.answer1;
        }
        if (currentBranch.answer2.Length > 0)
        {
            dialogueHandler.answer2.SetActive(true);
            dialogueHandler.answer2Text.text = currentBranch.answer2;
        }
        if (currentBranch.answer3.Length > 0)
        {
            dialogueHandler.answer3.SetActive(true);
            dialogueHandler.answer3Text.text = currentBranch.answer3;
        }
    }
}

