using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typeSpeed = 0.05f; // �������� ��������� ����

    private Coroutine typingCoroutine;
    private string currentLine;

    public void Initialize()
    {
        HideDialogue();
    }

    public void ShowDialogue(string line)
    {
        dialoguePanel.SetActive(true);
        currentLine = line;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        typingCoroutine = null;
    }

    public void HideDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = "";
        dialoguePanel.SetActive(false);
    }

    // ��������� ������ ��� ����������� ������ ���������
    public void ShowChoices(DialogueChoice[] choices, System.Action<int> onChoiceSelected)
    {
        // TODO: ������� ������ �� UI ��� ������� ��������
        // ��� ����� �� ������ �������� onChoiceSelected(index)
    }

    public void HideChoices()
    {
        // TODO: �������� ������ ������
    }

    public bool IsTyping()
    { 
        return typingCoroutine != null; 
    }

    public void SkipTypewriter()
    {
        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        dialogueText.text = currentLine;
    }
}
