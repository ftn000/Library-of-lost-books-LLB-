using UnityEngine;

public class VisitorNPC : NPCBase
{
    [Header("Mood")]
    [SerializeField] private float moodCheckInterval = 5f;
    private float timer;
    private bool unhappy = false;

    protected override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (timer > moodCheckInterval)
        {
            timer = 0;
            unhappy = Random.value < 0.15f;
        }
    }

    public bool IsUnhappy() => unhappy;

    protected override void OnPatrolPoint(Transform point)
    {
        // можно добавить реакцию, например:
        unhappy = Random.value < 0.05f;
    }
}
