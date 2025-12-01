using UnityEngine;

public class Book : Interactable
{
    public string bookId;
    [SerializeField] private NPC questNPC; // NPC, который дал задание
    [SerializeField] private Shelf shelf;

    public void Initialize(string id, Shelf s, NPC npc)
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

            if (questNPC != null)
                questNPC.CollectBook();
        }
    }
}
