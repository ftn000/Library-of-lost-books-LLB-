using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentIndex = 0;
    public float speed = 2f;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            currentIndex = (currentIndex + 1) % waypoints.Length;
    }
}
