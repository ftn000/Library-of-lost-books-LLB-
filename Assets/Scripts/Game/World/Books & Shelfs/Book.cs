using UnityEngine;

public class Book : Interactable
{
    public string bookId;
    private NPCBase questNPC;
    private Shelf shelf;

    public void Initialize(string id, Shelf s, NPCBase npc)
    {
        bookId = id;
        shelf = s;
        questNPC = npc;
    }

    public override void Interact(PlayerInventory inventory)
    {
        if (inventory.AddBook(this))
        {
            gameObject.SetActive(false);
            //questNPC?.CollectBook();
        }
    }
}
