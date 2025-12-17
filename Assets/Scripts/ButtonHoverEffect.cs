using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [SerializeField] private GameObject Corners;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private string hintText;
    [SerializeField] private bool selectable = true;

    private ButtonSelectionGroup group;
    private bool isSelected;

    public void Init(ButtonSelectionGroup owner)
    {
        group = owner;
    }

    // ====== PUBLIC API ======

    public void SetSelected(bool value)
    {
        isSelected = value;

        if (isSelected)
        {
            ShowCorners();
            ShowText();
        }
        else
        {
            HideCorners();
            HideText();
        }
    }

    // ====== POINTER EVENTS ======

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
            return;

        ShowCorners();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
            return;

        HideCorners();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selectable)
            return;

        group.Select(this);
    }

    // ====== VISUAL ======

    private void ShowCorners()
    {
        Corners.transform.position = transform.position;
        Corners.SetActive(true);
    }

    private void HideCorners()
    {
        Corners.SetActive(false);
    }

    private void ShowText()
    {
        rightText.text = hintText;
    }

    private void HideText()
    {
        rightText.text = "";
    }
}
