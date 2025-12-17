using UnityEngine;

public class InspectionController : MonoBehaviour
{
    public static InspectionController Instance;

    [SerializeField] private InspectorNPC inspectorNPC;

    [Header("Timer")]
    [SerializeField] private float minTime = 180f;
    [SerializeField] private float maxTime = 300f;

    [Header("Debug")]
    [SerializeField] private bool startOnAwake = true;

    private float _timer;
    private bool _inspectionInProgress = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        InspectionScore.Reset();
        ResetTimer();
    }

    private void OnEnable()
    {
        InspectorAI.OnInspectionFinished += HandleInspectionFinished;
    }

    private void OnDisable()
    {
        InspectorAI.OnInspectionFinished -= HandleInspectionFinished;
    }

    private void OnDestroy()
    {
        InspectorAI.OnInspectionFinished -= HandleInspectionFinished;
    }

    private void Start()
    {
        if (startOnAwake) ResetTimer();
    }

    private void Update()
    {
        if (_inspectionInProgress) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            StartInspection();
        }
    }

    private void StartInspection()
    {
        if (_inspectionInProgress) return;

        _inspectionInProgress = true;
        InspectionScore.Reset();
        inspectorNPC.StartInspection();
    }

    private void HandleInspectionFinished(int score, bool success)
    {
        _inspectionInProgress = false;
        // —брос таймера дл€ следующей инспекции
        ResetTimer();
    }

    private void ResetTimer()
    {
        _timer = Random.Range(minTime, maxTime);
    }

    public void ForceInspection()
    {
        ResetTimer();
        StartInspection();
    }
}
