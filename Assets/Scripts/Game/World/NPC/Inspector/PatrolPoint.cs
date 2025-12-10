using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Antlr3.Runtime.Tree.TreeWizard;

public class PatrolPoint : MonoBehaviour
{
    [Header("Shelf zones")]
    [SerializeField] private List<Shelf> shelves = new List<Shelf>();

    [Header("Misplaced book zones")]
    [SerializeField] private List<MisplacedBookSpot> misplacedSpots = new List<MisplacedBookSpot>();

    [Header("Dirt / Cleanliness")]
    [SerializeField] private List<DirtSpot> dirtSpots = new List<DirtSpot>();

    [Header("Visitor zones")]
    [SerializeField] private List<VisitorNPC> visitors = new List<VisitorNPC>();

    [Header("Optional")]
    [Tooltip("Включать лог при инспекции этой точки")]
    public bool debugLog = false;

    public void PerformInspection()
    {
        int addedWrong = 0;
        int addedDirt = 0;
        int addedUnhappy = 0;

        // Проверяем неправильные книги на полках
        foreach (var shelf in shelves)
        {
            if (shelf == null) continue;
            int wrong = shelf.CountWrongBooks();
            addedWrong += wrong;
        }

        // Проверяем книги, оставленные не на полках
        foreach (var spot in misplacedSpots)
        {
            if (spot == null) continue;
            if (spot.hasBook)
                addedWrong++;
        }

        // Проверяем грязь
        foreach (var dirt in dirtSpots)
        {
            if (dirt == null) continue;
            addedDirt += dirt.GetDirtLevel();
        }

        // Проверяем настроение посетителей
        foreach (VisitorNPC visitorNPC in visitors)
        {
            if (visitorNPC == null) continue;
            if (visitorNPC.IsUnhappy())
                addedUnhappy++;
        }

        InspectionScore.misplacedBooks += addedWrong;
        InspectionScore.dirtiness += addedDirt;
        InspectionScore.unhappyVisitors += addedUnhappy;

        if (debugLog)
        {
            Debug.Log($"PatrolPoint '{name}' inspected: misplaced={addedWrong}, dirt={addedDirt}, unhappy={addedUnhappy}");
        }
    }
}
