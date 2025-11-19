using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        menuUI.SetActive(false);
        //Time.timeScale = 1f; // Возобновляем игру
        isPaused = false;
    }

    void Pause()
    {
        menuUI.SetActive(true);
        //Time.timeScale = 0f; // Ставим игру на паузу
        isPaused = true;
    }

    public void QuitGame()
    {
        /*
        // Для сборки
        Application.Quit();
        // Для Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        */

        SceneManager.LoadScene("MainMenu");
    }
}