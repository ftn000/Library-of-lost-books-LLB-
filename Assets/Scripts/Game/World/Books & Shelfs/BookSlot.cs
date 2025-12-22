using UnityEngine;

public class BookSlot : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    public Book CurrentBook { get; private set; }

    public bool CanPlaceBook()
    {
        return !IsOccupied;
    }

    public void PlaceBook(Book book)
    {
        IsOccupied = true;
        CurrentBook = book;
        book.SetSlot(this);
    }

    public void Clear()
    {
        IsOccupied = false;
        CurrentBook = null;
    }
}