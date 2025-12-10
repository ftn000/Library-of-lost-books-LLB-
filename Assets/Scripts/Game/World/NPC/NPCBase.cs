using UnityEngine;
using System.Collections.Generic;

public abstract class NPCBase : MonoBehaviour
{
    // ----------------------------------------------------------
    // CONFIG
    // ----------------------------------------------------------

    [Header("Movement")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected CharacterController controller;

    [Header("Patrol")]
    [SerializeField] protected Transform patrolParent;
    protected Transform[] patrolPoints;
    protected int patrolIndex = 0;

    protected List<int> recent = new();
    [SerializeField] protected int rememberCount = 3;

    [Header("Avoidance")]
    [SerializeField] protected float detectDist = .55f;
    [SerializeField] protected float avoidTime = .6f;

    private bool isAvoiding = false;
    private int avoidDir = 1;

    [Header("Gaze")]
    [SerializeField] protected float gazeRadius = 2f;
    [SerializeField] protected float gazeSpeed = 5f;
    [SerializeField] protected Transform gazeParent;
    protected Transform[] gazeTargets;

    [Header("Patrol tuning")]
    [SerializeField] protected float arriveDistance = .3f;


    // ----------------------------------------------------------
    // INIT
    // ----------------------------------------------------------

    protected virtual void Awake()
    {
        if (patrolParent != null)
        {
            patrolPoints = new Transform[patrolParent.childCount];
            for (int i = 0; i < patrolParent.childCount; i++)
                patrolPoints[i] = patrolParent.GetChild(i);
        }

        if (gazeParent != null)
        {
            gazeTargets = new Transform[gazeParent.childCount];
            for (int i = 0; i < gazeParent.childCount; i++)
                gazeTargets[i] = gazeParent.GetChild(i);
        }
    }


    // ----------------------------------------------------------
    // MAIN LOOP
    // ----------------------------------------------------------

    protected virtual void Update()
    {
        LookAtTargets();
        Patrol();
    }


    // ----------------------------------------------------------
    // BLOCKED POINT CHECK
    // ----------------------------------------------------------

    protected bool IsPointBlocked(Vector3 world)
    {
        // проверяем капсулу вокруг точки
        return Physics.CheckCapsule(
            world + Vector3.up * .1f,
            world + Vector3.up * 1.0f,
            .2f
        );
    }


    // ----------------------------------------------------------
    // AVOIDANCE
    // ----------------------------------------------------------

    protected bool CheckObstacle(out Vector3 dir)
    {
        dir = Vector3.zero;

        Vector3 origin = transform.position + Vector3.up * .2f;

        bool front = Physics.Raycast(origin, transform.forward, detectDist);
        if (!front) return false;

        bool left = Physics.Raycast(origin, -transform.right, detectDist);
        bool right = Physics.Raycast(origin, transform.right, detectDist);

        if (!left && !right)
        {
            avoidDir = Random.value < .5f ? -1 : 1;
        }
        else if (!left)
        {
            avoidDir = -1;
        }
        else if (!right)
        {
            avoidDir = 1;
        }
        else
        {
            avoidDir = 0;
        }

        switch (avoidDir)
        {
            case -1: dir = -transform.right; break;
            case 1: dir = transform.right; break;
            default: dir = -transform.forward; break;
        }

        return true;
    }


    protected void Move(Vector3 dir)
    {
        dir.y = 0;

        if (isAvoiding)
        {
            Vector3 side = avoidDir == 1 ? transform.right :
                           avoidDir == -1 ? -transform.right :
                                          -transform.forward;

            controller.Move(side.normalized * speed * Time.deltaTime);

            bool free = !Physics.Raycast(
                transform.position + Vector3.up * .2f,
                transform.forward,
                detectDist
            );

            if (free)
                isAvoiding = false;

            return;
        }

        if (CheckObstacle(out var avoid))
        {
            isAvoiding = true;
            controller.Move(avoid.normalized * speed * Time.deltaTime);
            return;
        }

        controller.Move(dir.normalized * speed * Time.deltaTime);
    }


    // ----------------------------------------------------------
    // PATROL
    // ----------------------------------------------------------

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        var target = patrolPoints[patrolIndex];

        Vector3 to = target.position - transform.position;
        to.y = 0;

        float dist = to.magnitude;

        // прибыл
        if (dist < arriveDistance)
        {
            OnPatrolPoint(target);

            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

            return;
        }

        // ходим
        Move(to.normalized);

        // поворот
        if (dist > arriveDistance * 2f)
            transform.forward = Vector3.Lerp(transform.forward,
                                             to.normalized,
                                             Time.deltaTime * 6f);
    }



    // ----------------------------------------------------------
    // GAZE
    // ----------------------------------------------------------

    protected void LookAtTargets()
    {
        if (gazeTargets == null || gazeTargets.Length == 0)
            return;

        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var t in gazeTargets)
        {
            if (!t) continue;

            float d = Vector3.Distance(transform.position, t.position);

            if (d < gazeRadius && d < bestDist)
            {
                best = t;
                bestDist = d;
            }
        }

        if (!best) return;

        Vector3 dir = best.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < .01f) return;

        var rot = Quaternion.LookRotation(dir);

        transform.rotation =
            Quaternion.Lerp(transform.rotation,
                            rot,
                            Time.deltaTime * gazeSpeed);
    }


    // ----------------------------------------------------------
    // HOOK
    // ----------------------------------------------------------

    protected virtual void OnPatrolPoint(Transform point) { }
}
