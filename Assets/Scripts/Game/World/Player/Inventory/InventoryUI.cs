using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private TMP_Text[] slotTexts;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color selectedColor = Color.white;

    public void Initialize()
    {
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            Book book = inventory.slots[i];

            if (book == null)
            {
                slotTexts[i].text = $"{i + 1}. empty";
                slotTexts[i].color = normalColor;
            }
            else
            {
                slotTexts[i].text = $"{i + 1}. {book.bookId}";
                slotTexts[i].color =
                    (i == inventory.selectedSlot)
                    ? selectedColor
                    : normalColor;
            }
        }
    }
}
