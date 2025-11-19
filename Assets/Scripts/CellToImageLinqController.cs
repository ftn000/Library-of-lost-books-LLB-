using System.Collections.Generic;
using UnityEngine;

public class CellToImageLinqController : MonoBehaviour
{
    [SerializeField] private GameSpritesFolder _imageFolder;
    [SerializeField] private LayerController _layerController;
    [SerializeField] private CellImageUIController _cellImageUIController;

    private Dictionary<int, int> _cellIndexToImageIndexLinqDictionary = new Dictionary<int, int>();
    public static CellToImageLinqController instance;

    private void Start()
    {
        Initialize(_imageFolder, _layerController, _cellImageUIController);
    }

    public void Initialize(GameSpritesFolder imageFolder, LayerController layerController, CellImageUIController cellImageUIController)
    {
        _imageFolder = imageFolder;
        _layerController = layerController;
        _cellImageUIController = cellImageUIController;
        instance = this;
    }

    private int AddLinqBasedOnCellIndex(int cellIndex)
    {
        int randomImageIndex = GetRandomFreeImageIndex();
        CreateLinq(cellIndex, randomImageIndex);
        return randomImageIndex;

    }

    private int GetRandomFreeImageIndex()
    {
        List<Sprite> allImages = _imageFolder.GetCellSpriteList();
        foreach (KeyValuePair<int, int> linq in _cellIndexToImageIndexLinqDictionary)
        {
            allImages.RemoveAt(linq.Value);
        }

        int randomImageIndex = Random.Range(0, allImages.Count);
        return randomImageIndex;
    }

    private int AddLinqBasedOnImageIndex(int imageIndex)
    {
        int newCellIndex = _cellIndexToImageIndexLinqDictionary.Count + 1;

        CreateLinq(newCellIndex, imageIndex);

        return newCellIndex;
    }

    public void CreateLinq(int cellIndex, int imageIndex)
    {
        if(_cellIndexToImageIndexLinqDictionary.ContainsKey(cellIndex) || _cellIndexToImageIndexLinqDictionary.ContainsValue(imageIndex))
        {
            ChangeLinq(cellIndex, imageIndex);
            return;
        }

        _cellIndexToImageIndexLinqDictionary.Add(cellIndex, imageIndex);
    }

    private void ChangeLinq(int cellIndex, int imageIndex)
    {
        if (_cellIndexToImageIndexLinqDictionary.ContainsKey(cellIndex))
        {
            if (_cellIndexToImageIndexLinqDictionary[cellIndex] == imageIndex) return;
            RemoveLinq(cellIndex);
        }
        
        if (_cellIndexToImageIndexLinqDictionary.ContainsValue(imageIndex))
        {
            foreach (KeyValuePair<int, int> linq in _cellIndexToImageIndexLinqDictionary)
            {
                if (linq.Value == imageIndex) _cellIndexToImageIndexLinqDictionary[linq.Key] = GetRandomFreeImageIndex();
            }
        }

        _cellIndexToImageIndexLinqDictionary.Add(cellIndex,imageIndex);
    }

    private void RemoveLinq(int cellIndex)
    {
        _cellIndexToImageIndexLinqDictionary.Remove(cellIndex);
    }

    public int GetImageIndexByCellIndex(int cellIndex)
    {
        if (_cellIndexToImageIndexLinqDictionary.ContainsKey(cellIndex))
        {
            return _cellIndexToImageIndexLinqDictionary[cellIndex];
        }

        int newCellLinqImageIndex = AddLinqBasedOnCellIndex(cellIndex);
        return newCellLinqImageIndex;


    }

    public int GetCellIndexByImageIndex(int imageIndex)
    {
        foreach(KeyValuePair<int, int> linq in _cellIndexToImageIndexLinqDictionary)
        {
            if (linq.Value == imageIndex) return linq.Key;
        }

        int newCellIndex = AddLinqBasedOnImageIndex(imageIndex);
        return newCellIndex;

    }

    public void SortLinqDictionary()
    {
        List<int> imagesIndex = new List<int>();

        foreach (KeyValuePair<int, int> linq in _cellIndexToImageIndexLinqDictionary)
        {
            imagesIndex.Add(linq.Value);
        }

        Dictionary<int, int> sortredLinq = new Dictionary<int, int>();
        int cellIndex = 0;

        foreach(int imageIndex in imagesIndex)
        {
            sortredLinq.Add(cellIndex, imageIndex);
            cellIndex++;
        }

        _cellIndexToImageIndexLinqDictionary = sortredLinq;

        
    }

    public void UpdateCellIndexOnGridSlots()
    {
        List<Layer> layers = _layerController.GetLayerList();
        List<Sprite> images = _imageFolder.GetCellSpriteList();

        foreach (Layer layer in layers)
        {
            foreach (GridSlot gridSlot in layer.GridSlots)
            {
                if (gridSlot.State == GridSlotState.filled)
                {
                    int imageIndex = gridSlot.CellData.ImageIndex;
                    int cellIndex = GetCellIndexByImageIndex(imageIndex);

                    gridSlot.Fill(cellIndex, imageIndex, images[imageIndex]);
                }
            }
        }

        _cellImageUIController.UpdateView(_cellIndexToImageIndexLinqDictionary, _imageFolder);
    }

    public void CheckingValidOfLinks()
    {
        List<Layer> layers = _layerController.GetLayerList();

        foreach(Layer layer in layers)
        {
            foreach(GridSlot gridSlot in layer.GridSlots)
            {
                if(gridSlot.State == GridSlotState.filled)
                {
                    if (_cellIndexToImageIndexLinqDictionary.ContainsKey(gridSlot.CellData.Cellindex))
                    {
                        if (_cellIndexToImageIndexLinqDictionary[gridSlot.CellData.Cellindex] == gridSlot.CellData.ImageIndex) continue;
                        else
                        {
                            ChangeLinq(gridSlot.CellData.Cellindex, gridSlot.CellData.ImageIndex);
                        }
                    }
                    else
                    {
                        CreateLinq(gridSlot.CellData.Cellindex, gridSlot.CellData.ImageIndex);
                    }
                }
            }
        }

        List<int> cellIndexToRemove = new List<int>();

        foreach (KeyValuePair<int, int> linqPair in _cellIndexToImageIndexLinqDictionary)
        {
            int cellCount = 0;
            foreach (Layer layer in _layerController.GetLayerList())
            {
                foreach (GridSlot gridSlot in layer.GridSlots)
                {
                    if (gridSlot.State == GridSlotState.filled)
                    {
                        if (gridSlot.CellData.Cellindex == linqPair.Key) cellCount++;
                    }
                }
            }

            if (cellCount == 0) cellIndexToRemove.Add(linqPair.Key);
        }

        foreach(int cellIndex in cellIndexToRemove)
        {
            _cellIndexToImageIndexLinqDictionary.Remove(cellIndex);
        }
    }

    
}
