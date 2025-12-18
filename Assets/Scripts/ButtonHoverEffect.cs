using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private string hintText;
    [SerializeField] private bool selectable = true;

    private ButtonSelectionGroup group;
    private bool isSelected;

    public void Init(ButtonSelectionGroup owner)
    {
        group = owner;
    }

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (isSelected)
            ShowText();
        else
            HideText();
    }

    // ====== POINTER ======

    public void OnPointerEnter(PointerEventData eventData)
    {
        group.Hover(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        group.Unhover(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selectable)
            return;

        group.Select(this);
    }

    // ====== TEXT ======

    private void ShowText()
    {
        rightText.text = hintText;
    }

    private void HideText()
    {
        rightText.text = "";
    }
}
