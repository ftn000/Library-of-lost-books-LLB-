using System.Collections.Generic;
using UnityEngine;

public class GridSlotChanged
{
    private int _maxSavedSlotCount = 100;
    private List<GridSlotSavedData> _changedSlots;

    public GridSlotChanged(List<GridSlotSavedData> gridSlotDatas, int maxSavedSlotCount)
    {
        _changedSlots = gridSlotDatas;
        _maxSavedSlotCount = maxSavedSlotCount;
    }

    public void SaveChanges(GridSlot gridSlot)
    {
        /*
        if (_changedSlots.Count > _maxSavedSlotCount) _changedSlots.RemoveAt(0);

        GridSlotSavedData newData = new GridSlotSavedData();
        newData.GridSlot = gridSlot;
        newData.Type = gridSlot.Type;
        newData.VariantIndex = gridSlot.SpriteVariantIndex;
        newData.Sprite = gridSlot.CurrentSprite;

        _changedSlots.Add(newData);
        */
    }

    public void RevertChange()
    {
        /*
        if (_changedSlots.Count == 0) return;

        GridSlotSavedData targetData = _changedSlots[_changedSlots.Count - 1];
        targetData.GridSlot.ChangeGridSlot(targetData.Sprite, targetData.Type, targetData.VariantIndex, null);
        _changedSlots.RemoveAt(_changedSlots.Count - 1);
        */
    }

    public struct GridSlotSavedData
    {
        /*
        public GridSlot GridSlot;
        public GridSlotTypes Type;
        public int VariantIndex;
        public Sprite Sprite;
        */
    }
}
