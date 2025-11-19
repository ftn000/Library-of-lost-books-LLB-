using UnityEngine;

public class Layer : MonoBehaviour
{
    [SerializeField] private GridSlot _gridSlotPrefab;
    [SerializeField] private Transform _gridParent;

    public GridSlot[,] GridSlots { get; private set; }

    private CanvasGroup _canvasGroup;
    private int width = 14;
    private int height = 18;

    private bool _isVisible;

    public bool IsVisible => _isVisible;

    public void Initialize(System.Action<Layer, GridSlot, bool> onCellClick)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        GridSlots = new GridSlot[width, height];

        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GridSlot cell = Instantiate(_gridSlotPrefab, _gridParent);
                Vector2Int pos = new Vector2Int(x, y);

                cell.Initialize(pos, (GridSlot gridSlot, bool useClearInstrument) => onCellClick?.Invoke(this, gridSlot, useClearInstrument), this);

                GridSlots[x, y] = cell;

                cell.transform.position = new Vector3(x, y, 0);
            }
        }

        foreach(GridSlot gridSlot in GridSlots)
        {
            gridSlot.CreateAdjustList();
        }
    }

    public GridSlot GetGridSlotByCoordinate(int x, int y)
    {
        if (x >= 0 & x < width & y >= 0 & y < height) return GridSlots[x, y];
        else return null;
    }

    public void Activate()
    {
        if (!_isVisible) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Deactivate()
    {
        if (!_isVisible)
        {
            _canvasGroup.alpha = 0f; 
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            return;
        }

        _canvasGroup.alpha = 0.3f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetVisibility(bool visible)
    {
        _isVisible = visible;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = visible ? 1f : 0.3f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }
    }

    public void HideLayer(bool visible)
    {
        _isVisible = visible;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }
    }

}
