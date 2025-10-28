using UnityEngine;

public class NPC : Interactable
{
    [TextArea(2, 5)]
    public string[] initialDialogue; // ������ ���� NPC �� ������
    [TextArea(2, 5)]
    public string[] afterQuestDialogue; // ����� ����� ����

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject[] booksToSpawn; // �����, ������� ���������� ��� ������ ������
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
        // ���� �� �������� � ���������� � ���� �� �����
        if (dialogueUI == null)
        {
            if (dialogueUI == null)
                Debug.LogWarning($"NPC ({name}): �� ������ DialogueUI � �����. �������� DialogueUI � ���� ��� ������ �� �����.");
            else
                Debug.Log($"NPC ({name}): DialogueUI ������ �������������.");
        }

        if (initialDialogue == null || initialDialogue.Length == 0) Debug.LogWarning($"NPC ({name}): dialogueLines ������.");

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
            Debug.LogError($"NPC ({name}): ������ ������ ������ � dialogueUI == null.");
            return;
        }

        if (initialDialogue == null || initialDialogue.Length == 0)
        {
            Debug.LogWarning($"NPC ({name}): ��� ����� �������.");
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

        // ���� ����� �� ����� �������
        if (currentLine >= CurrentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        // ���������� ��������� ������
        dialogueUI.ShowDialogue(CurrentDialogue[currentLine]);
    }

    private void EndDialogue()
    {
        dialogueUI.HideDialogue();
        isTalking = false;
        currentLine = 0;

        // �������� ���������� ������
        if (!questCompleted && shelf.storedBooks >= booksToSpawn.Length)
        {
            questCompleted = true;
            playerInventory.keysCount++;
            Debug.Log($"NPC ({name}): ���� �����!");
        }

        playerInteraction?.EndDialogue();
    }
}
