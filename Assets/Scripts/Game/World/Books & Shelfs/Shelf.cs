using UnityEngine;

public class Shelf : Interactable
{
    public int totalSlots = 3;
    public int storedBooks = 0;
    public string shelfId = "History";

    public override void Interact(PlayerInventory inventory)
    {
        Book selected = inventory.GetSelectedBook();

        if (selected == null)
        {
            Debug.Log("Выбрана пустая ячейка инвентаря.");
            return;
        }

        if (selected.bookId != shelfId)
        {
            Debug.Log($"Книга [{selected.bookId}] не подходит для полки [{shelfId}]!");
            return;
        }

        if (storedBooks >= totalSlots)
        {
            Debug.Log($"Полка [{shelfId}] уже заполнена.");
            return;
        }

        storedBooks++;
        Destroy(selected.gameObject);
        inventory.RemoveSelectedBook();
        Debug.Log($"Книга [{selected.bookId}] поставлена на полку [{shelfId}] ({storedBooks}/{totalSlots})");
    }

    /// <summary>
    /// Количество неправильных книг (если потребуется для инспектора)
    /// </summary>
    public int CountWrongBooks()
    {
        // TODO: позже можно хранить список книг вместо счётчика
        return 0;
    }
}
