using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("��������� ��������������")]
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
        if (!inDialogue) // �� ����� ������� �� ����������� ����
            CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);

        if (colliders.Length > 0)
        {
            currentTarget = colliders[0].GetComponent<Interactable>();
            if (interactionHint != null && currentTarget != null)
                interactionHint.text = "������� E, ����� �����������������";
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
            // ���� ��� � ������� � ��� �������� NPC ��� ���, ����� ������������
            currentTarget.Interact(inventory);
            return;
        }

        if (currentTarget != null)
        {
            currentTarget.Interact(inventory);

            // ���� ��� NPC � �������� ����� �������, ����� ������� ����
            if (currentTarget is NPC)
            {
                inDialogue = true;
                if (interactionHint != null)
                    interactionHint.text = ""; // ����� ������ ��������� ���� ������ ������
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
            playerController.enabled = false; // ��������� ��������
    }

    // ���� ����� ���������� NPC �� ��������� ������� (��. NPC.ContinueDialogue)
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
