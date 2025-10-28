using UnityEngine;

public class Shelf : Interactable
{
    public int totalSlots = 3;
    private int storedBooks = 0;

    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.booksCount > 0 && storedBooks < totalSlots)
        {
            storedBooks++;
            inventory.booksCount--;
            Debug.Log($"Книга добавлена на полку. Теперь в полке {storedBooks}/{totalSlots}.");
        }
        else if (storedBooks >= totalSlots)
        {
            Debug.Log("Полка уже заполнена!");
        }
        else
        {
            Debug.Log("У вас нет книг, чтобы положить на полку.");
        }
    }
}
