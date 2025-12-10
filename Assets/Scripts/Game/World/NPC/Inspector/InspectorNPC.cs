using UnityEngine;

public class InspectorNPC : NPCBase
{
    [Header("Inspection")]
    [SerializeField] private PatrolPoint[] inspectionPoints;
    private int current = 0;

    private bool active = false;

    public void StartInspection()
    {
        active = true;
        gameObject.SetActive(true);

        InspectionScore.Reset();

        current = 0;

        patrolPoints = new Transform[inspectionPoints.Length];
        for (int i = 0; i < inspectionPoints.Length; i++)
            patrolPoints[i] = inspectionPoints[i].transform;
    }

    protected override void Update()
    {
        if (!active) return;

        base.Update();

        if (current >= inspectionPoints.Length)
            Finish();
    }

    protected override void OnPatrolPoint(Transform point)
    {
        int index = current;

        if (index < inspectionPoints.Length)
            inspectionPoints[index].PerformInspection();

        current++;

        Debug.Log($"current index = {current}; pos = {this.transform.position}");
    }

    private void Finish()
    {
        active = false;

        int score = InspectionScore.FinalScore();

        Debug.Log(score >= 80 ?
                  $"Inspection SUCCESS, score = {score}" :
                  $"Inspection FAIL = {score}");

        gameObject.SetActive(false);
    }
}
