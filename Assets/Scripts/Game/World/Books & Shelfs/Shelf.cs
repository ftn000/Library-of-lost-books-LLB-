using UnityEngine;

public class Shelf : Interactable
{
    public int totalSlots = 3;
    public int storedBooks = 0;
    public string shelfId = "History"; 

    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.booksCount > 0 && storedBooks < totalSlots)
        {
            storedBooks++;
            inventory.booksCount--;
            Debug.Log($"Книга добавлена на полку {shelfId}. Сейчас: {storedBooks}/{totalSlots}");
        }
        else if (storedBooks >= totalSlots)
        {
            Debug.Log($"Полка {shelfId} уже заполнена!");
        }
        else
        {
            Debug.Log("У вас нет книг для полки.");
        }
    }
}
