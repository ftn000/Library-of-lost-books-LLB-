using UnityEngine;

public class NPC : Interactable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] dialogueBeforeQuest;
    [SerializeField] private DialogueNode[] dialogueDuringQuest;
    [SerializeField] private DialogueNode[] dialogueAfterQuest;

    [Header("Quest")]
    [SerializeField] private GameObject[] booksToSpawn;
    [SerializeField] private Shelf shelf;
    [SerializeField] private PlayerInventory playerInventory;

    private bool questGiven = false;
    private bool questCompleted = false;

    public void Initialize()
    {
        foreach (var book in booksToSpawn)
            book.SetActive(false);
    }

    /// <summary>
    /// Возвращает текущее состояние квеста
    /// </summary>
    public QuestDialogueState GetDialogueState()
    {
        if (!questGiven) return QuestDialogueState.BeforeQuest;
        if (questGiven && !questCompleted) return QuestDialogueState.DuringQuest;
        return QuestDialogueState.AfterQuest;
    }

    /// <summary>
    /// Возвращает массив реплик для текущего состояния
    /// </summary>
    public DialogueNode[] GetCurrentDialogue()
    {
        switch (GetDialogueState())
        {
            case QuestDialogueState.BeforeQuest:
                return dialogueBeforeQuest;
            case QuestDialogueState.DuringQuest:
                return dialogueDuringQuest.Length > 0 ? dialogueDuringQuest : dialogueBeforeQuest; //пока нет диалога во время квеста
            case QuestDialogueState.AfterQuest:
                return dialogueAfterQuest;
            default:
                return dialogueBeforeQuest;
        }
    }

    public override void Interact(PlayerInventory inventory)
    {
        playerInventory = inventory;

        // Проверяем выполнение квеста сразу
        if (!questCompleted && questGiven && shelf.storedBooks >= booksToSpawn.Length)
        {
            questCompleted = true;
            playerInventory.keysCount++;
            Debug.Log($"{name}: Ключ выдан игроку!");
        }

        Debug.Log($"[NPC] Interact called on {name}, questGiven={questGiven}, questCompleted={questCompleted}, state={GetDialogueState()}");

        EventBus.Raise(new DialogueStartedEvent(this));
    }

    public void EndDialogue()
    {
        if (!questGiven)
        {
            questGiven = true;
            foreach (var book in booksToSpawn)
                book.SetActive(true);
        }

        if (questGiven && !questCompleted && shelf.storedBooks >= booksToSpawn.Length)
        {
            questCompleted = true;
            playerInventory.keysCount++;
            Debug.Log($"{name}: Ключ выдан игроку!");
        }
    }
}


public enum QuestDialogueState
{
    BeforeQuest,
    DuringQuest,
    AfterQuest
}