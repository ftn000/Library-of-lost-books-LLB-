using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameTest";
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buttonPanel;

    public void PlayGame()
    {
        Debug.Log("[MainMenu] Starting game...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            buttonPanel.SetActive(false);

        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            buttonPanel.SetActive(true);

        }
    }

    public void ExitGame()
    {
        Debug.Log("[MainMenu] Exiting game...");
        Application.Quit();

        // � ��������� Unity �� �������� Quit � ��� �����:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
