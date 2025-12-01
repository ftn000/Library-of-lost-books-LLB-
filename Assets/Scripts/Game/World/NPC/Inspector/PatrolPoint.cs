using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Antlr3.Runtime.Tree.TreeWizard;

public class PatrolPoint : MonoBehaviour
{
    [Header("Shelf zones")]
    [SerializeField] private List<Shelf> shelves;

    [Header("Misplaced book zones")]
    [SerializeField] private List<MisplacedBookSpot> misplacedSpots;

    [Header("Visitor zones")]
    [SerializeField] private List<Visitor> visitors;

    public void PerformInspection()
    {
        // ѕровер€ем неправильные книги на полках
        foreach (var shelf in shelves)
        {
            InspectionScore.misplacedBooks += shelf.CountWrongBooks();
        }

        // ѕровер€ем книги, оставленные не на полках
        foreach (var spot in misplacedSpots)
        {
            if (spot.hasBook)
                InspectionScore.misplacedBooks++;
        }
        /*
        // ѕровер€ем настроение посетителей
        foreach (var visitor in visitors)
        {
            if (visitor.IsUnhappy)
                InspectionScore.unhappyVisitors++;
        }*/
    }
}
