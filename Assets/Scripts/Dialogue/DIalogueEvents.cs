public struct DialogueStartedEvent
{
    public NPC npc;
    public DialogueStartedEvent(NPC npc) => this.npc = npc;
}

public struct DialogueEndedEvent
{
    public NPC npc;
    public DialogueEndedEvent(NPC npc) => this.npc = npc;
}
