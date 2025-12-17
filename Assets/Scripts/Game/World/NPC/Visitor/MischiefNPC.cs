using UnityEngine;
using System.Collections;

public class MischiefNPC : NPCBase
{
    [Header("Books Messing")]
    [SerializeField] private Transform shelvesParent;    // пустой объект с полками
    [SerializeField] private Transform wrongSpotsParent; // пустой объект с "не туда" точками
    [SerializeField] private float timeBetweenMessing = 3f;
    [SerializeField] private float carrySpeed = 1.5f;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private float bookHeight = 0.25f;
    [SerializeField] private GameObject missedBooks;

    private Shelf[] shelves;
    private Transform[] wrongSpots;
    private bool isMessing = false;

    protected override void Awake()
    {
        base.Awake();

        // собираем все полки
        if (shelvesParent != null)
        {
            shelves = new Shelf[shelvesParent.childCount];
            for (int i = 0; i < shelvesParent.childCount; i++)
                shelves[i] = shelvesParent.GetChild(i).GetComponent<Shelf>();
        }

        // собираем все "не туда" точки
        if (wrongSpotsParent != null)
        {
            wrongSpots = new Transform[wrongSpotsParent.childCount];
            for (int i = 0; i < wrongSpotsParent.childCount; i++)
                wrongSpots[i] = wrongSpotsParent.GetChild(i);
        }
    }

    private void Start()
    {
        StartCoroutine(MessingRoutine());
    }

    protected override void Update()
    {
        if (!isMessing)
            base.Update(); // только патруль и взгляды, если не переносим книгу
    }

    private IEnumerator MessingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMessing);

            if (shelves == null || wrongSpots == null || shelves.Length == 0 || wrongSpots.Length == 0)
                continue;

            // выбираем случайную полку с книгой
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

            if (shelf == null) continue;

            shelf.storedBooks--;

            GameObject bookObj = Instantiate(bookPrefab, transform.position + Vector3.up * bookHeight, Quaternion.identity);
            Book bookScript = bookObj.GetComponent<Book>();
            bookScript.Initialize(shelf.shelfId, shelf, null);

            // временно выключаем коллайдер, чтобы не блокировал движение
            Collider bookCollider = bookObj.GetComponent<Collider>();
            if (bookCollider) bookCollider.enabled = false;

            bookObj.transform.SetParent(transform);

            // выбираем случайную точку "не туда"
            Transform wrongSpot = wrongSpots[Random.Range(0, wrongSpots.Length)];

            isMessing = true;

            float moveTimer = 0f;
            while (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                    new Vector3(wrongSpot.position.x, 0f, wrongSpot.position.z)) > 0.1f &&
                   moveTimer < 5f) // таймаут, чтобы не зависнуть
            {
                moveTimer += Time.deltaTime;

                Vector3 dir = (wrongSpot.position - transform.position);
                dir.y = 0;
                controller.Move(dir.normalized * carrySpeed * Time.deltaTime);

                transform.forward = dir.normalized;
                bookObj.transform.position = transform.position + Vector3.up * bookHeight;

                yield return null;
            }

            // окончательная позиция книги
            Vector3 finalPos = wrongSpot.position;
            finalPos.y = bookHeight;
            bookObj.transform.SetParent(missedBooks.transform);
            bookObj.transform.position = finalPos;

            // включаем коллайдер обратно
            if (bookCollider) bookCollider.enabled = true;

            isMessing = false;
        }
    }

    protected override void OnPatrolPoint(Transform point)
    {
        // можно добавить реакцию, например:
    }
}
