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
        inventory.RemoveSelectedBook();

        Debug.Log($"Книга [{selected.bookId}] поставлена на полку [{shelfId}] ({storedBooks}/{totalSlots})");
    }

    // Количество неправильных книг на полке
    public int CountWrongBooks()
    {
        // Логика: книги, стоящие на полке с неправильным ID
        // Например, если мы хотим учитывать все книги на полке, кроме правильного bookId
        // Сейчас у нас только storedBooks и shelfId, значит, если есть книги, они считаются правильными
        // Но если у книги есть bookId != shelfId, то считаем её неправильной
        // Для упрощения считаем, что все книги на полке правильные (можно расширить позже)
        return 0;
    }
}
