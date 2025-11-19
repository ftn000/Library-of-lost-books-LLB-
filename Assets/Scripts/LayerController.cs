using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerController : MonoBehaviour
{
    [SerializeField] private Layer _layerPrefab;
    [SerializeField] private Transform _layerParent;
    [SerializeField] private Transform _uiButtonsParent;
    [SerializeField] private Button _uiButtonPrefab;

    [SerializeField] private InstrumentController _instrumentController;
    private List<Layer> _layers = new List<Layer>();
    private Layer _activeLayer;

    public void Start()
    {
        if (_layers.Count == 0)
            AddLayer();
    }

    public void Initialize(InstrumentController instrumentController)
    {
        _instrumentController = instrumentController;
    }

    public List<Layer> GetLayerList()
    {
        List<Layer> layers = new List<Layer>();
        layers.AddRange(_layers);
        return layers;
    }

    public void AddLayer()
    {
        var newLayer = Instantiate(_layerPrefab, _layerParent);
        newLayer.name = $"Layer {_layers.Count + 1}";
        newLayer.Initialize(OnCellClicked);

        _layers.Add(newLayer);

        CreateLayerButton(newLayer);
        SetActiveLayer(newLayer);
    }

    private void CreateLayerButton(Layer layer)
    {
        // Контейнер для кнопок слоя
        GameObject container = new GameObject($"LayerButtons_{layer.name}", typeof(RectTransform));
        container.transform.SetParent(_uiButtonsParent);
        container.transform.localScale = Vector3.one;

        HorizontalLayoutGroup hLayout = container.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 5;
        hLayout.childForceExpandWidth = false;
        hLayout.childForceExpandHeight = false;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        ContentSizeFitter fitter = container.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // --- Основная кнопка слоя ---
        Button mainButton = Instantiate(_uiButtonPrefab, container.transform);
        SetButtonText(mainButton, layer.name);

        LayoutElement mainLayout = mainButton.gameObject.AddComponent<LayoutElement>();
        mainLayout.preferredWidth = 160;
        mainLayout.preferredHeight = 30;
        mainLayout.flexibleWidth = 0;
        mainLayout.flexibleHeight = 0;

        mainButton.onClick.AddListener(() => SetActiveLayer(layer));

        // --- Кнопка "глаз" ---
        Button toggleButton = Instantiate(_uiButtonPrefab, container.transform);
        SetButtonText(toggleButton, "On");

        LayoutElement toggleLayout = toggleButton.gameObject.AddComponent<LayoutElement>();
        toggleLayout.preferredWidth = 30;
        toggleLayout.preferredHeight = 30;
        toggleLayout.flexibleWidth = 0;
        toggleLayout.flexibleHeight = 0;

        var tmp = toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null) tmp.fontSize = 14;

        toggleButton.onClick.AddListener(() =>
        {
            bool newVisibility = !layer.IsVisible;
            layer.HideLayer(newVisibility);

            SetButtonText(toggleButton, newVisibility ? "On" : "Off");
        });

        // --- Кнопка "X" ---
        Button removeButton = Instantiate(_uiButtonPrefab, container.transform);
        SetButtonText(removeButton, "X");

        LayoutElement removeLayout = removeButton.gameObject.AddComponent<LayoutElement>();
        removeLayout.preferredWidth = 30;
        removeLayout.preferredHeight = 30;
        removeLayout.flexibleWidth = 0;
        removeLayout.flexibleHeight = 0;

        removeButton.onClick.AddListener(() =>
        {
            Destroy(container); // удаляем весь контейнер кнопок
            RemoveLayer(layer);
        });

        // **Обновляем Layout сразу после создания**
        LayoutRebuilder.ForceRebuildLayoutImmediate(_uiButtonsParent.GetComponent<RectTransform>());
    }

    public void RemoveLayer(Layer layerToRemove)
    {
        if (!_layers.Contains(layerToRemove)) return;

        _layers.Remove(layerToRemove);
        Destroy(layerToRemove.gameObject);

        if (_activeLayer == layerToRemove && _layers.Count > 0)
            SetActiveLayer(_layers[0]);
    }


    private void SetButtonText(Button button, string text)
    {
        Text label = button.GetComponentInChildren<Text>();
        if (label != null) label.text = text;
        else
        {
            var tmp = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null) tmp.text = text; 
        }
    }



    public void SetActiveLayer(Layer activeLayer)
    {
        foreach (Layer layer in _layers)
        {
            if (layer == activeLayer) layer.Activate();
            else layer.Deactivate();
        }

        _activeLayer = activeLayer;
    }

    private void OnCellClicked(Layer layer, GridSlot gridSlot, bool clearButton)
    {
        Debug.Log($"Клик по ячейке {gridSlot} на слое {layer.name}");
        if (layer != _activeLayer) return;

        if (clearButton) _instrumentController.UseCleanerInstrument(gridSlot);
        else _instrumentController.UseCurrentInstrument(gridSlot);
    }
}
