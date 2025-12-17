using UnityEngine;

public class ButtonSelectionGroup : MonoBehaviour
{
    private ButtonHoverEffect current;

    private void Awake()
    {
        var buttons = GetComponentsInChildren<ButtonHoverEffect>();
        foreach (var button in buttons)
            button.Init(this);
    }

    public void Select(ButtonHoverEffect button)
    {
        if (current == button)
            return;

        ClearSelection();

        current = button;
        current.SetSelected(true);
    }

    public void ClearSelection()
    {
        if (current == null)
            return;

        current.SetSelected(false);
        current = null;
    }
}
