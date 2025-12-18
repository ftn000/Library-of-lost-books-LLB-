using UnityEngine;

public class ButtonSelectionGroup : MonoBehaviour
{
    [SerializeField] private GameObject corners;

    private ButtonHoverEffect currentSelected;
    private ButtonHoverEffect currentHover;

    private void Awake()
    {
        var buttons = GetComponentsInChildren<ButtonHoverEffect>();
        foreach (var button in buttons)
            button.Init(this);
    }

    // ====== HOVER ======

    public void Hover(ButtonHoverEffect button)
    {
        currentHover = button;
        MoveCorners(button);
    }

    public void Unhover(ButtonHoverEffect button)
    {
        if (currentHover != button)
            return;

        currentHover = null;

        // Возвращаем стрелки на выбранную кнопку
        if (currentSelected != null)
            MoveCorners(currentSelected);
        else
            corners.SetActive(false);
    }

    // ====== SELECTION ======

    public void Select(ButtonHoverEffect button)
    {
        if (currentSelected == button)
            return;

        ClearSelection();

        currentSelected = button;
        currentSelected.SetSelected(true);
        MoveCorners(button);
    }

    // ====== PUBLIC API ======

    public void ClearSelection()
    {
        if (currentSelected != null)
            currentSelected.SetSelected(false);

        currentSelected = null;
        currentHover = null;

        corners.SetActive(false);
    }

    // ====== VISUAL ======

    private void MoveCorners(ButtonHoverEffect button)
    {
        corners.transform.position = button.transform.position;
        corners.SetActive(true);
    }
}
