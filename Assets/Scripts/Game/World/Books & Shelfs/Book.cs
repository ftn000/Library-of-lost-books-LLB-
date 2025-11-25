using UnityEngine;

public class Book : Interactable
{
    [SerializeField] private NPC questNPC; // NPC, который дал задание
    [SerializeField] private Shelf shelf;

    public void SetShelfAndNPC(Shelf s, NPC npc)
    {
        shelf = s;
        questNPC = npc;
    }

    public override void Interact(PlayerInventory inventory)
    {
        inventory.booksCount++;
        Debug.Log($"Книга поднята! Всего книг: {inventory.booksCount}");

        if (questNPC != null)
        {
            // Увеличиваем счётчик на полке
            if (questNPC.GetComponent<Shelf>() != null)
                questNPC.GetComponent<Shelf>().storedBooks++;

            questNPC.CollectBook(); // уведомляем NPC/UI
        }

        gameObject.SetActive(false); // "исчезает"
    }
}
