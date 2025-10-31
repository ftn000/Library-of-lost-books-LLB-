[System.Serializable]
public class DialogueNode
{
    public string text;                  // ������� NPC
    public DialogueChoice[] choices;     // null ��� ������ = �������� �������
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;      // ����� �������� ������
    public int nextNodeIndex = -1; // ������ ��������� �������, -1 = ����� �������
}