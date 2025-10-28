using UnityEngine;

public class NPC : Interactable
{
    [TextArea(2, 5)]
    public string[] initialDialogue; // массив фраз NPC до квеста
    [TextArea(2, 5)]
    public string[] afterQuestDialogue; // после сбора книг

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject[] booksToSpawn; // книги, которые появляются при старте квеста
    //[SerializeField] private GameObject key;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Shelf shelf;

    private int currentLine = 0;
    private bool isTalking = false;
    private bool questGiven = false;
    private bool questCompleted = false;

    private string[] CurrentDialogue => questCompleted ? afterQuestDialogue : initialDialogue;

    public void Initialize()
    {
        // Если не назначен в инспекторе — ищем на сцене
        if (dialogueUI == null)
        {
            if (dialogueUI == null)
                Debug.LogWarning($"NPC ({name}): не найден DialogueUI в сцене. Перетащи DialogueUI в поле или добавь на сцену.");
            else
                Debug.Log($"NPC ({name}): DialogueUI найден автоматически.");
        }

        if (initialDialogue == null || initialDialogue.Length == 0) Debug.LogWarning($"NPC ({name}): dialogueLines пустые.");

        //if (key != null) key.SetActive(false);

        foreach (GameObject book in booksToSpawn) book.SetActive(false);

    }

    public override void Interact(PlayerInventory inventory)
    {
        playerInventory = inventory;

        Debug.Log($"NPC ({name}): Interact called. isTalking={isTalking}");

        if(isTalking && dialogueUI.IsTyping())
        {
            dialogueUI.SkipTypewriter();
            return;
        }

        if (dialogueUI == null)
        {
            Debug.LogError($"NPC ({name}): Нельзя начать диалог — dialogueUI == null.");
            return;
        }

        if (initialDialogue == null || initialDialogue.Length == 0)
        {
            Debug.LogWarning($"NPC ({name}): Нет линий диалога.");
            return;
        }

        if (!isTalking)
        {
            StartDialogue();
            if (shelf.storedBooks >= booksToSpawn.Length) questCompleted = true;

        }
        else
        {
            ContinueDialogue();
        }
    }

    private void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;

        if (!questGiven)
        {
            questGiven = true;
            foreach (GameObject book in booksToSpawn) book.SetActive(true);
        }

        Debug.Log($"currentDialogueArray = {CurrentDialogue[currentLine]}, {CurrentDialogue[currentLine + 1]}");
        dialogueUI.ShowDialogue(CurrentDialogue[currentLine]);

        playerInteraction?.StartDialogue(); //if player != null
    }

    private void ContinueDialogue()
    {
        currentLine++;

        // Если дошли до конца диалога
        if (currentLine >= CurrentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        // Показываем следующую строку
        dialogueUI.ShowDialogue(CurrentDialogue[currentLine]);
    }

    private void EndDialogue()
    {
        dialogueUI.HideDialogue();
        isTalking = false;
        currentLine = 0;

        // Проверка выполнения квеста
        if (!questCompleted && shelf.storedBooks >= booksToSpawn.Length)
        {
            questCompleted = true;
            playerInventory.keysCount++;
            Debug.Log($"NPC ({name}): ключ выдан!");
        }

        playerInteraction?.EndDialogue();
    }
}
