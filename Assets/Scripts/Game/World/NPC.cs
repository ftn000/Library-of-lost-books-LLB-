using UnityEngine;

public class NPC : Interactable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueNode[] dialogueBeforeQuest;
    [SerializeField] private DialogueNode[] dialogueDuringQuest;
    [SerializeField] private DialogueNode[] dialogueAfterQuest;

    [Header("Quest")]
    [SerializeField] private GameObject bookPrefab; // Префаб книги
    [SerializeField] private int minBooks = 1;
    [SerializeField] private int maxBooks = 5;
    [SerializeField] private Transform[] spawnPoints; // Точки, где могут появляться книги
    [SerializeField] private Shelf shelf;
    [SerializeField] private PlayerInventory playerInventory;

    private GameObject[] spawnedBooks;
    private bool questGiven = false;
    private bool questCompleted = false;

    public void Initialize()
    {
        
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
        if (!questCompleted && questGiven && shelf.storedBooks >= spawnedBooks.Length)
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
            SpawnBooks();
        }

        if (questGiven && !questCompleted && shelf.storedBooks >= spawnedBooks.Length)
        {
            questCompleted = true;
            playerInventory.keysCount++;
            Debug.Log($"{name}: Ключ выдан игроку!");
        }
    }

    private void SpawnBooks()
    {
        int bookCount = Random.Range(minBooks, maxBooks);
        spawnedBooks = new GameObject[bookCount];

        for (int i = 0; i < bookCount; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnedBooks[i] = Instantiate(bookPrefab, point.position, point.rotation);
        }

        Debug.Log($"[{name}] Задание выдано: нужно собрать {bookCount} книг.");
    }
}


public enum QuestDialogueState
{
    BeforeQuest,
    DuringQuest,
    AfterQuest
}