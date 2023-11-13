using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueInterractor : MonoBehaviour
{
    public string npcName;
    public string NpcName { get => npcName; }

    bool isDialogue;
    public bool isEscort;

    [SerializeField] Sprite npcAvatar;
    [SerializeField] DialogueHandler dialogueHandler;
    [SerializeField] QuestGiver questGiver;
    [SerializeField] Branch startBranch;
    public Branch currentBranch;
    public Branch nextQuestBranch;
    public QuestData givenQuest;

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
        if (other.TryGetComponent(out Player _player) && !isDialogue && dialogueCooldown <= 0 && !isEscort && !player.isReading )
        {
            RewardForQuest();
            
            if (nextQuestBranch != null && questGiver.questHandler.completedQuests.Contains(givenQuest))
            {
                SetNextBranch(nextQuestBranch);
            }
            
            player = _player;
            isDialogue = true;
            OpenDialogue();
            
            player.transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
            player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
            
            _ = RotateToPlayer(_player.transform);
        }
    }

    void RewardForQuest()
    {
        Quest quest = questGiver.IsRewardForQuest();

        if (quest != null)
        {
            questGiver.GiveReward(quest);

            if (quest.QuestData().isRepeatable)
            {
                TimeToRepeat(quest.QuestData().timeToRepeat, quest.questData);
            }
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
        SetNextBranch(currentBranch);
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
        if (nextBranch.isCloseDialogue && (givenQuest == null || !questGiver.questHandler.GetQuestByQuestData(givenQuest).isComplete))
        {
            CloseDialogue();
            currentBranch = nextBranch.link1;
            return;
        }
        
        if (nextBranch.isEscort)
        {
            CloseDialogue();
            if (TryGetComponent(out NPC_Move npc))
            {
                npc.Escort();
                isEscort = true;
            }
            return;
        }
        
        // Если ветка диалога СОДЕРЖИТ КВЕСТ
        if (nextBranch.questData != null)
        {
            // Если этот квест еще НЕ ВЗЯТ
            if (!questGiver.questHandler.takenQuestList.Contains(nextBranch.questData) && !questGiver.questHandler.completedQuests.Contains(nextBranch.questData))
            {
                if (!nextBranch.isSelectableQuest)
                {
                    Branch branch = nextBranch;
                    QuestData currentQuestData = nextBranch.questData;

                    if (gettingQuestBranch != null && gettingQuestBranch == nextBranch)
                    {
                        branch = selectableQuestBranch;
                        currentQuestData = selectableQuestBranch.questData;
                        if (nextBranch.questData != null) 
                            Debug.LogError("В ветке выбираемого ответа не нужно указывать квест, сработает тот, что указана в isSelectableQuest ветке");
                    }

                    givenQuest = currentQuestData;
                    questGiver.GiveQuest(currentQuestData, null);
                    nextQuestBranch = branch.questGoToLink;
                }
                else if (nextBranch.LinkToGetQuest != null && gettingQuestBranch == null)
                {
                    gettingQuestBranch = nextBranch.LinkToGetQuest;
                    selectableQuestBranch = nextBranch;
                }

                currentBranch = nextBranch;

            }
            // Если этот квест УЖЕ ВЗЯТ
            else
            {
                Quest quest = questGiver.questHandler.GetQuestByQuestData(nextBranch.questData);
                
                // Если этот квест ЕЩЕ НЕ ЗАВЕРШЁН
                if (quest != null && !quest.isComplete)
                {
                    if (nextBranch.questNotCompleteLink != null)
                    {
                        if (nextBranch.questNotCompleteLink.questGoToLink != null)
                            nextQuestBranch = nextBranch.questNotCompleteLink.questGoToLink;

                        currentBranch = nextBranch.questNotCompleteLink;
                    }
                    else
                    {
                        currentBranch = nextBranch;
                    }
                }
                // Если этот квест УЖЕ ВЫПОЛНЕН
                else
                {
                    if (nextBranch.isRepeatable)
                    {
                        if (nextBranch.questNotReadyLink == null)
                            Debug.LogError("Для неготового повторяющегося квеста нужна альтернативная ветка диалога!");
                        else if (nextBranch.questNotReadyLink.questData != null)
                            questGiver.GiveQuest(nextBranch.questNotReadyLink.questData, null);

                        if (nextBranch.questNotReadyLink != null && nextBranch.questNotReadyLink.questGoToLink != null)
                            nextQuestBranch = nextBranch.questNotReadyLink.questGoToLink;

                        if (nextBranch.questNotReadyLink != null) currentBranch = nextBranch.questNotReadyLink;
                        else currentBranch = nextQuestBranch;
                    }
                    else
                    {
                        currentBranch = nextQuestBranch;
                        RewardForQuest();
                    }
                }
            }
        }
        else
        {
            if (gettingQuestBranch != null && nextBranch == gettingQuestBranch)
            {
                questGiver.GiveQuest(selectableQuestBranch.questData, null);
                nextQuestBranch = selectableQuestBranch.questGoToLink;
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

