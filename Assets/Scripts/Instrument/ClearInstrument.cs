using System.Collections.Generic;
using UnityEngine;

public class ClearInstrument : Instrument
{
    public override void ActivateInstrument(GridSlot target)
    {
        target.Clear();

        List<GridSlot> adjustGridSlots = target.GetAdjustSlotList();
        foreach (GridSlot gridSlot in adjustGridSlots)
        {
            gridSlot.Unlock();
        }

        CellToImageLinqController.instance.CheckingValidOfLinks();
        CellToImageLinqController.instance.SortLinqDictionary();
        CellToImageLinqController.instance.UpdateCellIndexOnGridSlots();
    }

    
}
