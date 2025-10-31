using UnityEngine;

public class Book : Interactable
{
    public override void Interact(PlayerInventory inventory)
    {
        inventory.booksCount++;
        Debug.Log($"Книга поднята! Всего книг: {inventory.booksCount}");
        gameObject.SetActive(false); // "исчезает"
    }
}
