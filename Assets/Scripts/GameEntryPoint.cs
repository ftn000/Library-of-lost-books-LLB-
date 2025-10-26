using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
