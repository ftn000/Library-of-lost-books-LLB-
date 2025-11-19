using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] private Image _imageView;
    [SerializeField] private Image _cellBackground;
    [SerializeField] private Image _cellIcon;
    private Vector2Int _gridPosition;
    private System.Action<GridSlot, bool> _onClickCallback; // bool = true => use clear instrument
    private Layer _layer;
    private List<GridSlot> _adjastGridSlots;
    public GridSlotState State { get; private set; }
    public CellData CellData { get; private set; }
    
    public Instrument LastAppliedInstrument { get; private set; }
    public void Initialize(Vector2Int gridPosition, System.Action<GridSlot, bool> onClick, Layer layer)
    {
        _gridPosition = gridPosition;
        _onClickCallback = onClick;
        _imageView.gameObject.SetActive(true);
        name = $"GridSlot X:{gridPosition.x} Y:{gridPosition.y}";
        State = GridSlotState.empty;
        _layer = layer;
    }


    public void SetSprite(Sprite sprite)
    {
        _imageView.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (State == GridSlotState.locked) return;

        if (Input.GetMouseButton(0)) // лева€ или права€ кнопка зажата
        {
            _onClickCallback?.Invoke(this, false);
        }
        else if (Input.GetMouseButton(1))
        {
            _onClickCallback?.Invoke(this, true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (State == GridSlotState.locked) return;

        if (Input.GetMouseButton(0)) // лева€ или права€ кнопка зажата
        {
            _onClickCallback?.Invoke(this, false);
        }
        else if(Input.GetMouseButton(1))
        {
            _onClickCallback?.Invoke(this, true);
        }
    }

    public void Fill(int cellIndex, int imageIndex, Sprite icon)
    {
        if (State == GridSlotState.locked) return;

        CellData = new CellData(cellIndex, imageIndex);
        _cellBackground.gameObject.SetActive(true);
        _cellIcon.sprite = icon;

        State = GridSlotState.filled;
        //TODO: Ѕлокировать соседние €чейки
    }

    public void Clear()
    {
        if (State != GridSlotState.filled) return;

        CellData = null;
        _cellBackground.gameObject.SetActive(false);
        _cellIcon.sprite = null;

        State = GridSlotState.empty;
        //TODO: –азблокировать соседние €чейки
    }

    
    public void Lock()
    {
        CellData = null;
        _cellBackground.gameObject.SetActive(false);
        _cellIcon.sprite = null;
        _imageView.gameObject.SetActive(false);

        State = GridSlotState.locked;
    }

    public void Unlock()
    {
        bool adjustFilled = false;
        foreach(GridSlot gridSlot in _adjastGridSlots)
        {
            if (gridSlot.State == GridSlotState.filled) adjustFilled = true;
        }

        if (adjustFilled) return;

        State = GridSlotState.empty;
        _imageView.gameObject.SetActive(true);
    }

    public void CreateAdjustList()
    {
        _adjastGridSlots = new List<GridSlot>();
        for (int x = _gridPosition.x - 1; x <= _gridPosition.x + 1; x++)
        {
            for (int y = _gridPosition.y - 1; y <= _gridPosition.y + 1; y++)
            {
                GridSlot adjustGridSlot = _layer.GetGridSlotByCoordinate(x, y);
                if (adjustGridSlot != null & adjustGridSlot != this) _adjastGridSlots.Add(adjustGridSlot);
            }
        }
    }

    public List<GridSlot> GetAdjustSlotList()
    {
        List<GridSlot> adjustList = new List<GridSlot>();
        adjustList.AddRange(_adjastGridSlots);

        return adjustList;
    }
}

public class CellData
{
    public int Cellindex { get; private set; }
    public int ImageIndex { get; private set; }

    public CellData(int cellIndex, int imageIndex)
    {
        Cellindex = cellIndex;
        ImageIndex = imageIndex;
    }
}

public enum GridSlotState
{
    empty,
    filled,
    locked,
}
