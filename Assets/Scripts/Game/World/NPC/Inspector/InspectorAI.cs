using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InspectorAI : MonoBehaviour
{
    public static InspectorAI Instance;

    [Header("Path")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<PatrolPoint> patrolPoints;
    [SerializeField] private Transform exitPoint;

    private NavMeshAgent _agent;
    private int _current = 0;
    private bool _inspectionActive = false;

    private void Awake()
    {
        Instance = this;
        _agent = GetComponent<NavMeshAgent>();
        gameObject.SetActive(false);
    }

    public void StartInspection()
    {
        InspectionScore.Reset();

        transform.position = spawnPoint.position;
        gameObject.SetActive(true);

        _current = 0;
        _inspectionActive = true;

        MoveTo(patrolPoints[_current].transform.position);
    }

    private void Update()
    {
        if (!_inspectionActive) return;
        if (_agent.pathPending) return;

        if (_agent.remainingDistance <= 0.2f)
        {
            PatrolPoint currentPoint = patrolPoints[_current];
            currentPoint.PerformInspection();

            _current++;

            if (_current >= patrolPoints.Count)
            {
                StartCoroutine(FinishPath());
            }
            else
            {
                MoveTo(patrolPoints[_current].transform.position);
            }
        }
    }

    private void MoveTo(Vector3 point)
    {
        _agent.SetDestination(point);
    }

    private IEnumerator FinishPath()
    {
        _inspectionActive = false;

        MoveTo(exitPoint.position);

        // ждём пока дойдёт до двери
        while (!_agent.pathPending && _agent.remainingDistance > 0.25f)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // передаём итог
        int score = InspectionScore.FinalScore();

        if (score >= 80)
        {
            //PopupController.Instance.ShowPopup("InspectionSuccess");
            Debug.Log("Inspection Success");
        }
        else
        {
            //PopupController.Instance.ShowPopup("InspectionFail");
            Debug.Log("Inspection Fail");
        }

        // скрываем инспектора
        gameObject.SetActive(false);
    }
}
