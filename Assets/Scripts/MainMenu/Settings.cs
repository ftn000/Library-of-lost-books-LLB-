using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider volumeSlider;

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private TMP_Text fullscreenButtonText;

    [Header("References")]
    [SerializeField] private AudioSource mainAudioSource; // можно не указывать, если используешь AudioMixer
    [SerializeField] private GameObject settingsPanel;

    private Resolution[] resolutions;

    private bool isFullscreen;

    public void Initialize()
    {
        LoadResolutions();
        LoadSettings();
        fullscreenButtonText.text = isFullscreen ? "ON" : "OFF";
        isFullscreen = PlayerPrefs.GetInt("fullscreen", 0) == 1;
        ApplyFullscreen();
    }

    private void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }

    public void OnResolutionChanged(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", index);
    }

    public void ButtonFullscreen()
    {
        isFullscreen = !isFullscreen;
        ApplyFullscreen();

        // Сохраняем выбор
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }
    private void ApplyFullscreen()
    {
        Screen.fullScreen = isFullscreen;
        fullscreenButtonText.text = isFullscreen ? "ON" : "OFF";
    }

    public void BackToMenu()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        int resIndex = PlayerPrefs.GetInt("resolutionIndex", 0);
        bool fullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;

        volumeSlider.value = volume;
        AudioListener.volume = volume;

        if (resIndex < resolutions.Length)
            resolutionDropdown.value = resIndex;
    }
}
