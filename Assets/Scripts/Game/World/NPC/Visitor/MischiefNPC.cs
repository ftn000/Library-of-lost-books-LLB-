using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MischiefNPC : NPCBase
{
    [Header("Books Messing")]
    [SerializeField] private Transform shelvesParent;     // контейнер полок
    [SerializeField] private Transform wrongSpotsParent;  // контейнер BookSlot
    [SerializeField] private float timeBetweenMessing = 3f;
    [SerializeField] private float carrySpeed = 1.5f;
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private float bookHeight = 0.25f;
    [SerializeField] private GameObject missedBooks;

    private Shelf[] shelves;
    private BookSlot[] bookSlots;

    private bool isMessing = false;

    protected override void Awake()
    {
        base.Awake();

        // Собираем полки
        if (shelvesParent != null)
        {
            shelves = new Shelf[shelvesParent.childCount];
            for (int i = 0; i < shelvesParent.childCount; i++)
                shelves[i] = shelvesParent.GetChild(i).GetComponent<Shelf>();
        }

        // Собираем слоты для книг
        if (wrongSpotsParent != null)
        {
            bookSlots = new BookSlot[wrongSpotsParent.childCount];
            for (int i = 0; i < wrongSpotsParent.childCount; i++)
                bookSlots[i] = wrongSpotsParent.GetChild(i).GetComponent<BookSlot>();
        }

        List<BookSlot> slots = new List<BookSlot>();

        for (int i = 0; i < wrongSpotsParent.childCount; i++)
        {
            BookSlot slot = wrongSpotsParent.GetChild(i).GetComponent<BookSlot>();
            if (slot != null)
                slots.Add(slot);
            else
                Debug.LogWarning($"[MischiefNPC] Child {wrongSpotsParent.GetChild(i).name} has no BookSlot");
        }

        bookSlots = slots.ToArray();
    }

    private void Start()
    {
        StartCoroutine(MessingRoutine());
    }

    protected override void Update()
    {
        if (!isMessing)
            base.Update(); // патруль и взгляды
    }

    private IEnumerator MessingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMessing);

            if (shelves == null || shelves.Length == 0 ||
                bookSlots == null || bookSlots.Length == 0)
                continue;

            // Ищем полку с книгами
            Shelf shelf = null;
            for (int i = 0; i < 10; i++)
            {
                Shelf candidate = shelves[Random.Range(0, shelves.Length)];
                if (candidate.storedBooks > 0)
                {
                    shelf = candidate;
                    break;
                }
            }

            if (shelf == null)
                continue;

            // Ищем свободный слот
            List<BookSlot> freeSlots = new List<BookSlot>();
            foreach (BookSlot slot in bookSlots)
            {
                if (slot.CanPlaceBook())
                    freeSlots.Add(slot);
            }

            // Если мест нет — просто ходим
            if (freeSlots.Count == 0)
                continue;

            BookSlot targetSlot = freeSlots[Random.Range(0, freeSlots.Count)];
            Transform targetPoint = targetSlot.transform;

            // Забираем книгу
            shelf.storedBooks--;

            GameObject bookObj = Instantiate(
                bookPrefab,
                transform.position + Vector3.up * bookHeight,
                Quaternion.identity
            );

            Book bookScript = bookObj.GetComponent<Book>();
            bookScript.Initialize(shelf.shelfId, shelf, null);

            Collider bookCollider = bookObj.GetComponent<Collider>();
            if (bookCollider) bookCollider.enabled = false;

            bookObj.transform.SetParent(transform);

            isMessing = true;

            float moveTimer = 0f;
            while (Vector3.Distance(
                       new Vector3(transform.position.x, 0f, transform.position.z),
                       new Vector3(targetPoint.position.x, 0f, targetPoint.position.z)) > 0.1f
                   && moveTimer < 5f)
            {
                moveTimer += Time.deltaTime;

                Vector3 dir = targetPoint.position - transform.position;
                dir.y = 0f;

                controller.Move(dir.normalized * carrySpeed * Time.deltaTime);
                transform.forward = dir.normalized;

                bookObj.transform.position = transform.position + Vector3.up * bookHeight;

                yield return null;
            }

            // Кладём книгу
            Vector3 finalPos = targetPoint.position;
            finalPos.y = bookHeight;

            bookObj.transform.SetParent(missedBooks.transform);
            bookObj.transform.position = finalPos;

            targetSlot.PlaceBook(bookScript);

            if (bookCollider) bookCollider.enabled = true;

            isMessing = false;
        }
    }

    protected override void OnPatrolPoint(Transform point)
    {
        // опционально
    }
}
