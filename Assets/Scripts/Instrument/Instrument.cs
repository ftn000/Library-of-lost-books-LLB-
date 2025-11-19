using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Instrument : MonoBehaviour
{
    [SerializeField] public List<string> _tags;
    [SerializeField] protected Image _image;
    [SerializeField] protected Button _chooseButton;

    private int _variantIndex;
    private Action<Instrument> _chooseAction;

    public Instrument _associatedInstrumentNext { get; private set; }
    public Instrument _associatedInstrumentPrevious { get; private set; }
    public Sprite Icon => _image.sprite;
    public int VariantIndex => _variantIndex;

    public void Initialize(Sprite icon, int variantIndex, Action<Instrument> chooseAction)
    {
        _image.sprite = icon;
        _variantIndex = variantIndex;

        _chooseAction = chooseAction;

        _chooseButton.onClick.RemoveAllListeners();
        _chooseButton.onClick.AddListener(ChooseButonClicked);        
    }

    private void ChooseButonClicked()
    {
        _chooseAction?.Invoke(this);
    }

    public void SetAssociatedInstrumentNext(Instrument instrument)
    {
        _associatedInstrumentNext = instrument;
    }

    public void SetAssociatedInstrumentPrevious(Instrument instrument)
    {
        _associatedInstrumentPrevious = instrument;
    }

    public abstract void ActivateInstrument(GridSlot target);
}
