using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] public string objectName;

    public virtual void Interact()
    {
        Debug.Log($"Взаимодействие с {objectName}");
    }
}

