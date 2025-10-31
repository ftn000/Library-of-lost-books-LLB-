using UnityEngine;

public class Book : Interactable
{
    public override void Interact(PlayerInventory inventory)
    {
        inventory.booksCount++;
        Debug.Log($"����� �������! ����� ����: {inventory.booksCount}");
        gameObject.SetActive(false); // "��������"
    }
}
