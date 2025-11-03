using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementWithStamina : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera mainCamera;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainPerSecond = 10f;
    [SerializeField] private float staminaRecoverPerSecond = 5f;
    [SerializeField] private float exhaustionSpeedMultiplier = 0.5f;

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory; // количество книг в инвентаре
    [SerializeField] private float loadSpeedMultiplier = 0.1f; // скорость падает на X% за книгу

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    private float verticalVelocity;
    private bool isGrounded;

    private Vector2 moveInput;
    private float currentStamina;
    private float currentSpeed;
    private float rotationVelocity;

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
    }

    public void Initialize()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        HandleStamina();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);

        // Расчет скорости с нагрузкой
        int books = playerInventory != null ? playerInventory.booksCount : 0;
        float loadMultiplier = Mathf.Max(0f, 1f - books * loadSpeedMultiplier);
        currentSpeed = baseMoveSpeed * loadMultiplier;
        if (currentStamina <= 0f)
            currentSpeed *= exhaustionSpeedMultiplier;

        // Поворот относительно камеры
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            direction = moveDir.normalized * currentSpeed;
        }
        else
        {
            direction = Vector3.zero;
        }

        // Гравитация
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // маленькая фиксация, чтобы стоять на земле

        verticalVelocity += gravity * Time.deltaTime;
        direction.y = verticalVelocity;

        controller.Move(direction * Time.deltaTime);
    }

    private void HandleStamina()
    {
        if (moveInput.magnitude > 0.1f)
        {
            // Движение тратит стамину
            currentStamina -= staminaDrainPerSecond * Time.deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
        }
        else
        {
            // Отдых восстанавливает стамину
            currentStamina += staminaRecoverPerSecond * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina);
        }
    }



    // Для UI или отладки
    public float GetCurrentStaminaPercent()
    {
        return currentStamina / maxStamina;
    }
}
