using System.Collections.Generic;
using UnityEngine;

public class CellInstrument : Instrument
{
    public override void ActivateInstrument(GridSlot target)
    {
        int cellIndex = CellToImageLinqController.instance.GetCellIndexByImageIndex(VariantIndex);
        target.Fill(cellIndex, VariantIndex, Icon);

        List<GridSlot> adjustGridSlots = target.GetAdjustSlotList();
        foreach(GridSlot gridSlot in adjustGridSlots)
        {
            gridSlot.Lock();
        }

        CellToImageLinqController.instance.CheckingValidOfLinks();
        CellToImageLinqController.instance.SortLinqDictionary();
        CellToImageLinqController.instance.UpdateCellIndexOnGridSlots();
    }
}
