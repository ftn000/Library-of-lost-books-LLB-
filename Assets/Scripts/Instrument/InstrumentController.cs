using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstrumentController : MonoBehaviour
{
    [SerializeField] private GameSpritesFolder _spriteFolder;
    [Header("Current instrument monitor")]
    [SerializeField] private Image _choosingInstrumentMonitor;
    [SerializeField] private Vector3 _mouseOffset;

    [SerializeField] private Instrument _clearInstrument;

    [Header("Instruments Prefab")]
    [SerializeField] private CellInstrument _cellInstrumentPrefab;

    [Header("Instrument Layouts")]
    [SerializeField] private Transform _instrumentsLayout;

    private List<Instrument> _instruments = new List<Instrument>();

    private Action<Instrument> _chooseInstument;

    private Instrument _currentInstrument;

    private GridSlotChanged _gridSlotChangedData;

    private float _timeToRevertChangedOnButtonDawn = 0.8f;
    private float _timeToRevertButtonDown = 0;

    
    private void OnEnable()
    {
        Debug.Log("enable");
        _chooseInstument += ChooseInstrument;
    }

    public void ChooseInstrument(Instrument instrument)
    {
        _currentInstrument = instrument;
        _choosingInstrumentMonitor.sprite = _currentInstrument.Icon;
    }

    
    private void OnDisable()
    {
        _chooseInstument -= ChooseInstrument;
    }
    
    private void Start()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        CreateInstruments(_spriteFolder.GetCellSpriteList(), _cellInstrumentPrefab);

        ChooseInstrument(_instruments[0]);


        AssociatedInstruments();

        _gridSlotChangedData = new GridSlotChanged(new List<GridSlotChanged.GridSlotSavedData>(), 1000);
    }

    
    public void CreateInstruments(List<Sprite> variants, Instrument prefab)
    {
        int variantIndex = 0;

        foreach(Sprite variant in variants)
        {
            Instrument newInstrument = Instantiate(prefab, _instrumentsLayout);
            newInstrument.Initialize(variant, variantIndex, _chooseInstument);
            _instruments.Add(newInstrument);

            variantIndex++;
        }
    }

    public void CreateInstruments(Sprite variant, Instrument prefab)
    {
        Instrument newInstrument = Instantiate(prefab, _instrumentsLayout);
        newInstrument.Initialize(variant, 0, _chooseInstument);
        _instruments.Add(newInstrument);

    }

    public void AssociatedInstruments()
    {
        List<Instrument> notAssociatedInstruments = new List<Instrument>();

        foreach (Instrument instrument in _instruments)
        {
            notAssociatedInstruments.Add(instrument);
        }

        while (notAssociatedInstruments.Count > 0)
        {

            foreach (Instrument checkedInstrument in notAssociatedInstruments)
            {
                if (notAssociatedInstruments[0] != checkedInstrument)
                {
                    if (notAssociatedInstruments[0]._tags[0] == checkedInstrument._tags[0])
                    {
                        notAssociatedInstruments[0].SetAssociatedInstrumentNext(checkedInstrument);
                        checkedInstrument.SetAssociatedInstrumentPrevious(notAssociatedInstruments[0]);
                        break;
                    }
                }
            }

            notAssociatedInstruments.RemoveAt(0);
        }

    }

    private void ClearInstruments(List<Instrument> list)
    {
        while (list.Count > 0)
        {
            Destroy(list[0].gameObject);
            list.RemoveAll(obj => obj == null);
        }

        _currentInstrument = null;
    }   

    public void UseCurrentInstrument(GridSlot gridSlot)
    {
        if (gridSlot.LastAppliedInstrument == _currentInstrument) return;

        _gridSlotChangedData.SaveChanges(gridSlot); 
        _currentInstrument.ActivateInstrument(gridSlot);
    }

    public void UseCleanerInstrument(GridSlot gridSlot)
    {
        _clearInstrument.ActivateInstrument(gridSlot);
    }

    private void FixedUpdate()
    {
        RevertChangesButtonCheck();
        
        if (_currentInstrument != null)
        {
            //_choosingInstrumentMonitor.transform.position = Input.mousePosition + _mouseOffset;
        }
        else
        {
            //_choosingInstrumentMonitor.gameObject.SetActive(false);
        }

        
    }

    private void Update()
    {
        ChangeInstrumentToAssociated();
    }

    private void ChangeInstrumentToAssociated()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");        

        Instrument instrument = null;

        if(scrollInput > 0)
        {
            instrument = _currentInstrument._associatedInstrumentPrevious;
        }
        else if(scrollInput < 0)
        {
            instrument = _currentInstrument._associatedInstrumentNext;
        }

        if (instrument != null) ChooseInstrument(instrument);
    }

    private void RevertChangesButtonCheck()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _timeToRevertButtonDown += Time.deltaTime;
            if (_timeToRevertButtonDown >= _timeToRevertChangedOnButtonDawn)
            {
                if (_gridSlotChangedData != null) _gridSlotChangedData.RevertChange();
            }
        }
        else if (_timeToRevertButtonDown > 0 & _timeToRevertButtonDown < _timeToRevertChangedOnButtonDawn)
        {
            if (_gridSlotChangedData != null) _gridSlotChangedData.RevertChange();
            _timeToRevertButtonDown = 0;
        }
        else if (_timeToRevertButtonDown > 0)
        {
            _timeToRevertButtonDown = 0;
        }
    }
    
}
