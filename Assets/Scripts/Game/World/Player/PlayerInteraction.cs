using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactionHint;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerController playerController;

    private Interactable currentTarget;
    private bool inDialogue = false;

    public void Initialize()
    {
        if (interactionHint != null)
            interactionHint.text = "";
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DialogueStartedEvent>(OnDialogueStarted);
        EventBus.Subscribe<DialogueEndedEvent>(OnDialogueEnded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DialogueStartedEvent>(OnDialogueStarted);
        EventBus.Unsubscribe<DialogueEndedEvent>(OnDialogueEnded);
    }

    private void Update()
    {
        if (!inDialogue)
            CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);

        if (colliders.Length > 0)
        {
            currentTarget = colliders[0].GetComponent<Interactable>();
            if (interactionHint != null && currentTarget != null)
                interactionHint.text = "Нажмите E, чтобы взаимодействовать";
        }
        else
        {
            currentTarget = null;
            if (interactionHint != null)
                interactionHint.text = "";
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Debug.Log($"[PlayerInteraction] Interact pressed. inDialogue={inDialogue}, currentTarget={(currentTarget ? currentTarget.name : "null")}");

        if (currentTarget != null)
        {
            currentTarget.Interact(inventory);
        }
    }

    private void OnDialogueStarted(DialogueStartedEvent evt)
    {
        inDialogue = true;
        playerController.enabled = false;
        currentTarget = null;
        if (interactionHint != null)
            interactionHint.text = "";
        Debug.Log($"[PlayerInteraction] Started dialogue with NPC {evt.npc.name}");
    }

    private void OnDialogueEnded(DialogueEndedEvent evt)
    {
        inDialogue = false;
        playerController.enabled = true;
        currentTarget = null;
        if (interactionHint != null)
            interactionHint.text = "";
        Debug.Log($"[PlayerInteraction] Dialogue ended with NPC {evt.npc.name}");
    }
}
