using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
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

    private void Update()
    {
        if (!inDialogue) // во время диалога не переключаем цель
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
            Debug.DrawLine(transform.position, colliders[0].transform.position, Color.green);
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

        Debug.Log($"PlayerInteraction: Interact called. currentTarget={(currentTarget ? currentTarget.name : "null")}, inDialogue={inDialogue}");

        if (inDialogue && currentTarget is NPC)
        {
            // Если уже в диалоге — шлём интеракт NPC ещё раз, чтобы перелистнуть
            currentTarget.Interact(inventory);
            return;
        }

        if (currentTarget != null)
        {
            currentTarget.Interact(inventory);

            // Если это NPC — включаем режим диалога, иначе очищаем цель
            if (currentTarget is NPC)
            {
                inDialogue = true;
                if (interactionHint != null)
                    interactionHint.text = ""; // можно убрать подсказку пока диалог открыт
            }
            else
            {
                currentTarget = null;
                if (interactionHint != null)
                    interactionHint.text = "";
            }
        }
    }

    public void StartDialogue()
    {
        inDialogue = true;
        if (playerController != null)
            playerController.enabled = false; // блокируем движение
    }

    // Этот метод вызывается NPC по окончании диалога (см. NPC.ContinueDialogue)
    public void EndDialogue()
    {
        inDialogue = false;
        if (playerController != null)
            playerController.enabled = true;
        currentTarget = null;
        if (interactionHint != null)
            interactionHint.text = "";
    }
}
