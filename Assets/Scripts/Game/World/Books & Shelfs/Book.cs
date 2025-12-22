using UnityEngine;

public class Book : Interactable
{
    public string bookId;
    private NPCBase questNPC;
    private Shelf shelf;

    public BookSlot currentSlot { get; private set; }

    public void Initialize(string id, Shelf s, NPCBase npc)
    {
        bookId = id;
        shelf = s;
        questNPC = npc;
    }

    public void SetSlot(BookSlot slot)
    {
        currentSlot = slot;
    }

    public void ClearSlot()
    {
        currentSlot = null;
    }

    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.AddBook(this))
        {
            currentSlot?.Clear();
            gameObject.SetActive(false);
            //questNPC?.CollectBook();
        }
    }

}
