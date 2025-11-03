using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] private PlayerMovementWithStamina playerMovement;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        if (playerMovement == null)
            Debug.LogError("PlayerMovementWithStamina not assigned in PlayerStaminaUI");
        if (staminaSlider == null)
            Debug.LogError("Stamina Slider not assigned in PlayerStaminaUI");
    }

    private void Update()
    {
        if (playerMovement != null && staminaSlider != null)
        {
            float percent = playerMovement.GetCurrentStaminaPercent();
            staminaSlider.value = percent;

            // Μενÿεμ φβες
            if (fillImage != null)
                fillImage.color = Color.Lerp(Color.red, Color.green, percent);
        }
    }
}
