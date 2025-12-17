using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    public int maxSlots = 5;
    public Book[] slots;          // Массив книг
    public int selectedSlot = 0;  // Текущий выбранный слот

    public int keysCount = 0;

    private void Awake()
    {
        slots = new Book[maxSlots];
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SelectSlot(4);
    }

    // Добавление книги в первый пустой слот
    public bool AddBook(Book b)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = b;
                selectedSlot = i; // выбираем этот слот как текущий
                inventoryUI.Refresh();
                Debug.Log($"Книга [{b.bookId}] помещена в слот {i + 1}");
                return true;
            }
        }

        Debug.Log("Инвентарь заполнен!");
        return false;
    }

    // Получение книги из выбранного слота
    public Book GetSelectedBook()
    {
        return slots[selectedSlot];
    }

    // Выбор слота
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        selectedSlot = index;
        inventoryUI.Refresh();
        Book b = slots[selectedSlot];

        if (b == null)
            Debug.Log($"Слот {index + 1}: пусто");
        else
            Debug.Log($"Слот {index + 1}: книга [{b.bookId}]");
    }

    // Удаление книги из выбранного слота
    public void RemoveSelectedBook()
    {
        slots[selectedSlot] = null;
        inventoryUI.Refresh();
    }
}