[System.Serializable]
public class DialogueNode
{
    public string text;                  // реплика NPC
    public DialogueChoice[] choices;     // null или пустой = линейная реплика
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;      // текст варианта игрока
    public int nextNodeIndex = -1; // индекс следующей реплики, -1 = конец диалога
}