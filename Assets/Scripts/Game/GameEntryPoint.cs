using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] PlayerInteraction playerInteraction;
    [SerializeField] NPC npc;
    [SerializeField] DialogueUI dialogueUI;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] PlayerMovementWithStamina movementWithStamina;
    [SerializeField] QuestUI questUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInteraction.Initialize();
        //npc.Initialize();
        dialogueUI.Initialize();
        dialogueController.Initialize();
        movementWithStamina.Initialize();
        questUI.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
