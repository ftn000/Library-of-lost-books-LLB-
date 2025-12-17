using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private ButtonSelectionGroup selectionGroup;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Continue();
            else
                Pause();
        }
    }

    public void Continue()
    {
        menuUI.SetActive(false);
        confirmPanel.SetActive(false);
        selectionGroup.ClearSelection();

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        menuUI.SetActive(true);
        confirmPanel.SetActive(false);
        selectionGroup.ClearSelection();

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void PanelActivate()
    {
        confirmPanel.SetActive(true);
        selectionGroup.ClearSelection();
    }

    public void PanelDeactivate()
    {
        confirmPanel.SetActive(false);
        selectionGroup.ClearSelection();
    }

    public void QuitGame() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
