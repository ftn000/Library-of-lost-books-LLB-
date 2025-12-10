using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class InspectorAI : MonoBehaviour
{
    public static InspectorAI Instance;
    public static Action<int, bool> OnInspectionFinished;

    [Header("Path")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<PatrolPoint> patrolPoints;
    [SerializeField] private Transform exitPoint;

    [Header("Behaviour")]
    [SerializeField] private float inspectDelay = 2f;
    [SerializeField] private float arrivalThreshold = 0.2f;
    [SerializeField] private float stuckTimeout = 8f;

    private NavMeshAgent _agent;
    private int _current = 0;
    private bool _inspectionActive = false;
    private Coroutine _currentInspectCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        _agent = GetComponent<NavMeshAgent>();
        gameObject.SetActive(false);
    }

    public void StartInspection()
    {
        StopAllCoroutines();

        InspectionScore.Reset();

        transform.position = spawnPoint.position;
        gameObject.SetActive(true);

        _current = 0;
        _inspectionActive = true;

        MoveTo(patrolPoints[_current].transform.position);

        // optionally play spawn animation here
    }

    private void Update()
    {
        if (!_inspectionActive) return;

        // Normal flow handled in coroutines to avoid racing in Update.
    }

    private void MoveTo(Vector3 point)
    {
        if (_agent == null) return;
        _agent.isStopped = false;
        _agent.SetDestination(point);
    }

    private IEnumerator InspectRoutine()
    {
        while (_inspectionActive)
        {
            if (_current >= patrolPoints.Count) break;

            // wait until path ready
            float wait = 0f;
            while (_agent.pathPending)
            {
                yield return null;
            }

            // wait until arrived or timeout
            while (!_agent.pathPending && _agent.remainingDistance > arrivalThreshold && wait < stuckTimeout)
            {
                wait += Time.deltaTime;
                yield return null;
            }

            // arrived or timed out — stop agent and inspect
            _agent.isStopped = true;

            // small delay to "look around"
            yield return new WaitForSeconds(inspectDelay);

            // perform inspection on the point (cooperative: point adds to InspectionScore)
            PatrolPoint point = patrolPoints[_current];
            point.PerformInspection();

            _current++;
            if (_current < patrolPoints.Count)
            {
                MoveTo(patrolPoints[_current].transform.position);
                yield return null;
            }
        }

        // move to exit only after points are done
        StartCoroutine(FinishPath());
    }

    private IEnumerator FinishPath()
    {
        _inspectionActive = false;

        MoveTo(exitPoint.position);

        // wait for arrival or small timeout
        float wait = 0f;
        while (_agent.pathPending)
            yield return null;

        while (!_agent.pathPending && _agent.remainingDistance > arrivalThreshold && wait < stuckTimeout)
        {
            wait += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.4f);

        int score = InspectionScore.FinalScore();
        bool success = score >= 80;

        // show popup via controller
        PopupController.Instance.ShowResult(score, success);

        Debug.Log($"Inspection finished. Score: {score}. Success: {success}");

        // raise event
        OnInspectionFinished?.Invoke(score, success);

        // hide inspector
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Start the internal coroutine only when inspector is active
        if (_inspectionActive && _currentInspectCoroutine == null)
            _currentInspectCoroutine = StartCoroutine(InspectRoutine());
    }

    private void OnDisable()
    {
        if (_currentInspectCoroutine != null)
        {
            StopCoroutine(_currentInspectCoroutine);
            _currentInspectCoroutine = null;
        }
    }

    // This is called from StartInspection so start the coroutine explicitly there:
    private void OnValidate()
    {
        // nothing
    }

    // Ensure the InspectRoutine is running after StartInspection
    public void EnsureRoutineRunning()
    {
        if (_currentInspectCoroutine == null) _currentInspectCoroutine = StartCoroutine(InspectRoutine());
    }

    // modify StartInspection to call EnsureRoutineRunning
    public void StartInspection_Public()
    {
        StartInspection();
        EnsureRoutineRunning();
    }
}
