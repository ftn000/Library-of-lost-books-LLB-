using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask interactableLayer;

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Debug.Log("Interact pressed");

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        if (colliders.Length == 0)
        {
            Debug.Log("Ничего не найдено для взаимодействия");
            return;
        }

        foreach (Collider col in colliders)
        {
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log($"Взаимодействие с {interactable.objectName}");
                interactable.Interact();
            }
        }
    }

}
