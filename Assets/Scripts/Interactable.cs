using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string objectName;

    public virtual void Interact()
    {
        Debug.Log($"Взаимодействие с {objectName}");
    }
}

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 2f;
    public LayerMask interactableLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange, interactableLayer))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                    interactable.Interact();
            }
        }
    }
}
