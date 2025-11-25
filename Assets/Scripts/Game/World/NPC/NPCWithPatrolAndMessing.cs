using UnityEngine;
using System.Collections;

public class NPCWithPatrolAndMessing : MonoBehaviour
{
    [Header("Dialogue & Quest")]
    [SerializeField] private NPC npc; 
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    private int currentPatrolIndex = 0;

    [Header("Books Messing")]
    [SerializeField] private Shelf[] shelves; 
    [SerializeField] private Transform[] wrongSpots; 
    [SerializeField] private float timeBetweenMessing = 3f;
    [SerializeField] private float carrySpeed = 1.5f;
    private bool isMessing = false;
    [SerializeField] private GameObject bookPrefab;

    [Header("Heights")]
    [SerializeField] private float npcY = 0.5f;    // NPC на полу
    [SerializeField] private float bookY = 0.15f; // Книга чуть выше пола
    [SerializeField] private float carryHeight = 0.5f; // книга в руках NPC

    private void Start()
    {
        if (patrolPoints.Length == 0) Debug.LogWarning("No patrol points set!");
        if (shelves.Length == 0 || wrongSpots.Length == 0) Debug.LogWarning("Shelves or wrong spots not set!");
        StartCoroutine(MessingRoutine());
    }

    private void Update()
    {
        if (!isMessing)
            Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f;

        transform.position += dir * patrolSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, npcY, transform.position.z);

        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir;

        if (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                             new Vector3(target.position.x, 0f, target.position.z)) < 0.1f)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private IEnumerator MessingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMessing);

            // Выбираем случайную полку с книгой
            Shelf shelf = null;
            int tries = 0;
            while (tries < 10)
            {
                Shelf candidate = shelves[Random.Range(0, shelves.Length)];
                if (candidate.storedBooks > 0)
                {
                    shelf = candidate;
                    break;
                }
                tries++;
            }

            if (shelf != null)
            {
                // Берём книгу с полки
                shelf.storedBooks--;

                // Создаём объект книги, привязываем к NPC
                Vector3 spawnPos = new Vector3(transform.position.x, npcY + carryHeight, transform.position.z);
                GameObject bookObj = Instantiate(bookPrefab, spawnPos, Quaternion.identity);
                bookObj.transform.SetParent(transform);

                // Выбираем случайную точку "не туда"
                Transform wrongSpot = wrongSpots[Random.Range(0, wrongSpots.Length)];
                Vector3 targetPos = new Vector3(wrongSpot.position.x, bookY, wrongSpot.position.z);

                // NPC идёт к точке с книгой
                isMessing = true;
                while (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                        new Vector3(targetPos.x, 0f, targetPos.z)) > 0.05f)
                {
                    Vector3 dir = (targetPos - transform.position);
                    dir.y = 0f;
                    transform.position += dir.normalized * carrySpeed * Time.deltaTime;
                    transform.position = new Vector3(transform.position.x, npcY, transform.position.z);

                    if (dir.sqrMagnitude > 0.001f)
                        transform.forward = dir;

                    // Книга в руках NPC
                    bookObj.transform.position = new Vector3(transform.position.x, npcY + carryHeight, transform.position.z);

                    yield return null;
                }

                // Оставляем книгу на "не туда" точке
                bookObj.transform.SetParent(null);
                bookObj.transform.position = targetPos;

                isMessing = false;
            }
        }
    }
}
