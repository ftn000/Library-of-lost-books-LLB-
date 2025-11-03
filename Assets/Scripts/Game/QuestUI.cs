using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questText;
    [SerializeField] private NPC npc; // NPC, который даёт квест
    [SerializeField] private GameObject questPanel;

    private void OnEnable()
    {
        if (npc != null)
        {
            npc.OnQuestUpdated += UpdateQuestUI; // подписка на событие
            UpdateQuestUI();
        }
    }

    private void OnDisable()
    {
        if (npc != null)
            npc.OnQuestUpdated -= UpdateQuestUI;
    }

    public void Initialize()
    {
        questPanel.SetActive(false);
        questText.text = "";
    }

    public void UpdateQuestUI()
    {
        if (npc == null) return;

        int collected = npc.GetCollectedBooksCount();
        int required = npc.GetRequiredBooksCount();

        // Обновляем текст
        questText.text = $"Собрать книги: {collected}/{required}";

        // Показываем панель, пока квест активен (даже если все книги собраны)
        questPanel.SetActive(npc.GetDialogueState() == QuestDialogueState.DuringQuest);

    }

}
