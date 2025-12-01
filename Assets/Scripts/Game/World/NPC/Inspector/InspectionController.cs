using UnityEngine;

public class InspectionController : MonoBehaviour
{
    public static InspectionController Instance;

    [SerializeField] private float minTime = 180f;
    [SerializeField] private float maxTime = 300f;

    private float _timer;

    private void Awake()
    {
        Instance = this;
        ResetTimer();
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            ResetTimer();
            InspectorAI.Instance.StartInspection();
        }
    }

    private void ResetTimer()
    {
        _timer = UnityEngine.Random.Range(minTime, maxTime);
    }
}
