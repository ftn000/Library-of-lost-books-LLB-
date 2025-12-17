using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private Transform missedBooksParent;
    [SerializeField] private TMP_Text missedBooks;

    public int MissedBooksCount =>
        missedBooksParent != null ? missedBooksParent.childCount : 0;

    private void Update()
    {
        missedBooks.text = $"Missed Books: {MissedBooksCount}";
    }
}
