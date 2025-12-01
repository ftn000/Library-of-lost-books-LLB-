using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NPCWithPatrolAndMessing : MonoBehaviour
{
    [Header("Dialogue & Quest")]
    [SerializeField] private NPC npc;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Patrol")]
    [SerializeField] private Transform patrolParent; // пустой объект с точками патруля
    [SerializeField] private float patrolSpeed = 2f;
    private Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Books Messing")]
    [SerializeField] private Transform shelvesParent;    // пустой объект с полками
    [SerializeField] private Transform wrongSpotsParent; // пустой объект с "не туда" точками
    [SerializeField] private float timeBetweenMessing = 3f;
    [SerializeField] private float carrySpeed = 1.5f;
    private bool isMessing = false;
    [SerializeField] private GameObject bookPrefab;

    [Header("Heights")]
    [SerializeField] private float npcHeight = 0.5f;
    [SerializeField] private float bookHeight = 0.25f;
    [Space]
    [SerializeField] private CharacterController controller;
    private Shelf[] shelves;
    private Transform[] wrongSpots;

    private void Awake()
    {

        // Собираем все точки патруля из родителя
        if (patrolParent != null)
        {
            patrolPoints = new Transform[patrolParent.childCount];
            for (int i = 0; i < patrolParent.childCount; i++)
                patrolPoints[i] = patrolParent.GetChild(i);
        }

        // Собираем все полки
        if (shelvesParent != null)
        {
            shelves = new Shelf[shelvesParent.childCount];
            for (int i = 0; i < shelvesParent.childCount; i++)
                shelves[i] = shelvesParent.GetChild(i).GetComponent<Shelf>();
        }

        // Собираем все "не туда" точки
        if (wrongSpotsParent != null)
        {
            wrongSpots = new Transform[wrongSpotsParent.childCount];
            for (int i = 0; i < wrongSpotsParent.childCount; i++)
                wrongSpots[i] = wrongSpotsParent.GetChild(i);
        }
    }

    private void Start()
    {
        Vector3 pos = transform.position;
        pos.y = npcHeight;
        transform.position = pos;

        StartCoroutine(MessingRoutine());
    }

    private void Update()
    {
        if (!isMessing)
            Patrol();
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector3 dir = (target.position - transform.position);
        dir.y = 0f;
        Vector3 move = dir.normalized * patrolSpeed * Time.deltaTime;

        controller.Move(move);

        if (dir.sqrMagnitude > 0.001f)
            transform.forward = dir.normalized;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
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
                shelf.storedBooks--;

                GameObject bookObj = Instantiate(bookPrefab, transform.position + Vector3.up * bookHeight, Quaternion.identity);
                Book bookScript = bookObj.GetComponent<Book>();
                bookScript.Initialize(shelf.shelfId, shelf, null); // присвоили ID полки

                bookObj.transform.SetParent(transform);

                // Выбираем случайную точку "не туда"
                Transform wrongSpot = wrongSpots[Random.Range(0, wrongSpots.Length)];

                isMessing = true;
                while (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                        new Vector3(wrongSpot.position.x, 0f, wrongSpot.position.z)) > 0.1f)
                {
                    Vector3 dir = (wrongSpot.position - transform.position);
                    dir.y = 0f;
                    Vector3 move = dir.normalized * carrySpeed * Time.deltaTime;
                    controller.Move(move);

                    transform.forward = dir.normalized;
                    bookObj.transform.position = transform.position + Vector3.up * bookHeight;

                    yield return null;
                }

                Vector3 finalPos = wrongSpot.position;
                finalPos.y = bookHeight;
                bookObj.transform.SetParent(null);
                bookObj.transform.position = finalPos;

                isMessing = false;
            }
        }
    }
}
