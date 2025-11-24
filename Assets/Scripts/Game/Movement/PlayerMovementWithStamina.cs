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

    [Header("Sprint")]
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private float sprintMultiplier = 1.7f;
    private bool isSprinting;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainPerSecond = 10f;
    [SerializeField] private float staminaRecoverPerSecond = 5f;
    [SerializeField] private float exhaustionSpeedMultiplier = 0.5f;
    [SerializeField] private float exhaustionDelay = 1f;
    private float exhaustionTimer = 0f;
    [SerializeField] private UnityEngine.UI.Image staminaBar;
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowStaminaColor = Color.red;
    [SerializeField] private float blinkSpeed = 4f;

    [Header("Inventory")]
    [SerializeField] private PlayerInventory playerInventory; // количество книг в инвентаре
    [SerializeField] private float loadSpeedMultiplier = 0.1f; // скорость падает на X% за книгу

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity;
    private bool isGrounded;

    private Vector2 moveInput;
    private float currentStamina;
    private float currentSpeed;
    private float rotationVelocity;
    private bool isActuallyMoving;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

    public void Initialize()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        isSprinting = sprintAction.action.IsPressed() && currentStamina > 0f;

        moveInput = moveAction.action.ReadValue<Vector2>();

        // Оглушение при нуле стамины
        if (currentStamina <= 0f)
            exhaustionTimer = exhaustionDelay;

        if (exhaustionTimer > 0f)
        {
            exhaustionTimer -= Time.deltaTime;
            moveInput = Vector2.zero;
            isSprinting = false;
        }

        HandleMovement();
        HandleStamina();
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);

        int books = playerInventory != null ? playerInventory.booksCount : 0;
        float loadMultiplier = Mathf.Max(0f, 1f - books * loadSpeedMultiplier);

        float targetSpeed = baseMoveSpeed * loadMultiplier;

        if (isSprinting)
            targetSpeed *= sprintMultiplier;
        else if (currentStamina <= 0f)
            targetSpeed *= exhaustionSpeedMultiplier;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);

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
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        direction.y = verticalVelocity;

        Vector3 moveDelta = direction * Time.deltaTime;
        controller.Move(moveDelta);

        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        isActuallyMoving = horizontalVelocity.magnitude > 0.01f;
    }

    private void HandleStamina()
    {
        float staminaDrain = 0f;

        // Тратим стамину только если игрок реально движется
        if (moveInput.magnitude > 0.1f && isActuallyMoving)
        {
            staminaDrain = isSprinting ? staminaDrainPerSecond : staminaDrainPerSecond * 0.4f;
        }

        currentStamina -= staminaDrain * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Восстановление, если стоим
        if (moveInput.magnitude <= 0.1f || !isActuallyMoving)
        {
            currentStamina += staminaRecoverPerSecond * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        // Обновление UI
        float percent = currentStamina / maxStamina;
        Color gradientColor = Color.Lerp(normalColor, lowStaminaColor, 1f - percent);

        // Мигание при критически низкой стамине (<10%)
        if (percent < 0.1f)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color blinkColor = new Color(lowStaminaColor.r, lowStaminaColor.g, lowStaminaColor.b, 0f);
            gradientColor = Color.Lerp(gradientColor, blinkColor, t);
        }

        staminaBar.color = gradientColor;
    }

    public float GetCurrentStaminaPercent()
    {
        return currentStamina / maxStamina;
    }
}
