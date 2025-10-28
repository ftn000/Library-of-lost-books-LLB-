using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactionHint;
    [SerializeField] private PlayerInventory inventory;

    private Interactable currentTarget;
    private Outline currentOutline;

    private void Start()
    {
        if (interactionHint != null)
            interactionHint.text = "";
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);

        Interactable newTarget = null;
        Outline newOutline = null;

        if (colliders.Length > 0)
        {
            newTarget = colliders[0].GetComponent<Interactable>();

            if (newTarget != null)
            {
                newOutline = newTarget.GetComponent<Outline>();

                if (interactionHint != null)
                    interactionHint.text = "Нажмите E, чтобы взаимодействовать";
            }
        }
        else
        {
            if (interactionHint != null)
                interactionHint.text = "";
        }

        // Если сменился объект, выключаем старый Outline
        if (currentOutline != null && currentOutline != newOutline)
            currentOutline.enabled = false;

        // Включаем Outline нового объекта
        if (newOutline != null && newOutline != currentOutline)
            newOutline.enabled = true;

        currentTarget = newTarget;
        currentOutline = newOutline;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && currentTarget != null)
        {
            currentTarget.Interact(inventory);

            // После поднятия предмета выключаем Outline
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }

            currentTarget = null;
            if (interactionHint != null)
                interactionHint.text = "";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
