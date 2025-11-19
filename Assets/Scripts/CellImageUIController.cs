using System.Collections.Generic;
using UnityEngine;

public class CellImageUIController : MonoBehaviour
{
    [SerializeField] private Transform _layout;
    [SerializeField] private LinqView _prefab;
    [SerializeField] private LayerController _layerController;

    private List<LinqView> _links = new List<LinqView>();

    public void UpdateView(Dictionary<int, int> linq, GameSpritesFolder imageFolder)
    {
        while(_links.Count > linq.Count)
        {
            Destroy(_links[_links.Count - 1].gameObject);
            _links.RemoveAt(_links.Count - 1);
        }

        while (_links.Count < linq.Count)
        {
            LinqView newView = Instantiate(_prefab, _layout);
            _links.Add(newView);
        }

        int linksIndex = 0;
        List<Sprite> images = imageFolder.GetCellSpriteList();

        foreach (KeyValuePair<int, int> linqPair in linq)
        {
            int cellCount = 0;
            foreach(Layer layer in _layerController.GetLayerList())
            {
                foreach(GridSlot gridSlot in layer.GridSlots)
                {
                    if(gridSlot.State == GridSlotState.filled)
                    {
                        if (gridSlot.CellData.Cellindex == linqPair.Key) cellCount++;
                    }
                }
                
            }
            _links[linksIndex].FillView(linqPair.Key, images[linqPair.Value], cellCount);
            linksIndex++;
        }
    }
}
