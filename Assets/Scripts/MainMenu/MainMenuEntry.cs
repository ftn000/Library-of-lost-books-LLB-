using UnityEngine;

public class MainMenuEntry : MonoBehaviour
{
    [SerializeField] SettingsPanel settings;
    void Start()
    {
        settings.Initialize();
    }

    void Update()
    {
        
    }
}
