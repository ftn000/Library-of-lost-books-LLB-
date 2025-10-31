using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInteraction playerInteraction;
    [SerializeField] NPC npc;
    [SerializeField] DialogueUI dialogueUI;
    [SerializeField] DialogueController dialogueController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController.Initialize();
        playerInteraction.Initialize();
        npc.Initialize();
        dialogueUI.Initialize();
        dialogueController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
