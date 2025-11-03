using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private float typeSpeed = 0.03f;

    private DialogueNode[] nodes;
    private int currentNodeIndex;
    private Coroutine typingCoroutine;
    private NPC currentNPC;

    public bool IsTyping => typingCoroutine != null;

    public void Initialize()
    {
        dialoguePanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DialogueStartedEvent>(OnDialogueStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DialogueStartedEvent>(OnDialogueStarted);
    }

    private void OnDialogueStarted(DialogueStartedEvent evt)
    {
        Debug.Log($"[DialogueController] Dialogue started with NPC {evt.npc.name}");

        currentNPC = evt.npc;
        nodes = currentNPC.GetCurrentDialogue();

        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogWarning("[DialogueController] Нет линий диалога для этого NPC!");
            return;
        }

        currentNodeIndex = 0;
        dialoguePanel.SetActive(true);
        Debug.Log("[DialogueController] Showing first node: " + nodes[currentNodeIndex].text);
        ShowNode(nodes[currentNodeIndex]);
    }



    private void ShowNode(DialogueNode node)
    {
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(node.text));

        if (node.choices != null)
        {
            foreach (var choice in node.choices)
            {
                var btnObj = Instantiate(choiceButtonPrefab, choicesContainer);
                var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                btnText.text = choice.choiceText;
                btnObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.nextNodeIndex));
            }
        }

        if (node.nodeType == DialogueNodeType.RestartQuest)
        {
            EventBus.RestartQuest();
            Debug.Log("[DialogueController] Quest restarted via dialogue node");
        }
    }

    private IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        typingCoroutine = null;
    }

    public void SkipTypewriter()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            dialogueText.text = nodes[currentNodeIndex].text;
        }
    }

    public void Next()
    {
        if (IsTyping)
        {
            SkipTypewriter();
            return;
        }

        if (nodes[currentNodeIndex].choices == null || nodes[currentNodeIndex].choices.Length == 0)
        {
            currentNodeIndex++;
            if (currentNodeIndex >= nodes.Length)
            {
                EndDialogue();
                return;
            }
            ShowNode(nodes[currentNodeIndex]);
        }
    }

    private void OnChoiceSelected(int nextNodeIndex)
    {
        if (nextNodeIndex < 0 || nextNodeIndex >= nodes.Length)
        {
            EndDialogue();
            return;
        }
        currentNodeIndex = nextNodeIndex;
        ShowNode(nodes[currentNodeIndex]);
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentNPC?.EndDialogue();
        EventBus.Raise(new DialogueEndedEvent(currentNPC));
        currentNPC = null;
    }
}

public enum DialogueNodeType
{
    Normal,
    RestartQuest
}
