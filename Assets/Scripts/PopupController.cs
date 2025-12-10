using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public static PopupController Instance;

    [Header("Panels")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Text fields (UGUI Text)")]
    [SerializeField] private Text successText;
    [SerializeField] private Text failText;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        HideAll();
    }

    public void ShowResult(int score, bool success)
    {
        HideAll();

        if (success)
        {
            if (successPanel != null) successPanel.SetActive(true);
            if (successText != null) successText.text = $"Инспекция пройдена\nОценка: {score}";
        }
        else
        {
            if (failPanel != null) failPanel.SetActive(true);
            if (failText != null) failText.text = $"Инспекция не пройдена\nОценка: {score}";
        }
    }

    public void HideAll()
    {
        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
    }

    // Hook for UI button to close popup
    public void ClosePopup()
    {
        HideAll();
    }
}
