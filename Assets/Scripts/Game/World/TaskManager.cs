using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private Transform missedBooksParent;
    [SerializeField] private TMP_Text missedBooks;
    [SerializeField] private TMP_Text inspectionTimer;

    public int MissedBooksCount =>
        missedBooksParent != null ? missedBooksParent.childCount : 0;

    private void Update()
    {
        missedBooks.text = $"Missed Books: {MissedBooksCount}";
        if (InspectionController.Instance.getTimer() != 0)
            inspectionTimer.text = $"Inspection: {InspectionController.Instance.getTimer()}";
        if (InspectionController.Instance.getTimer() == 0)
            inspectionTimer.text = $"Inspection running";
    }
}
