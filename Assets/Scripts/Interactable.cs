using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string objectName = "Объект";
    public abstract void Interact(PlayerInventory inventory);
}