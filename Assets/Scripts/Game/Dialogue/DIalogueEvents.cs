public struct DialogueStartedEvent
{
    public NPCBase npc;
    public DialogueStartedEvent(NPCBase npc) => this.npc = npc;
}

public struct DialogueEndedEvent
{
    public NPCBase npc;
    public DialogueEndedEvent(NPCBase npc) => this.npc = npc;
}
