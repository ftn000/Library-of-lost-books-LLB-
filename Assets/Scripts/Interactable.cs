using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string objectName = "������";
    public abstract void Interact(PlayerInventory inventory);
}