using UnityEngine;
using UnityEngine.InputSystem;

public enum LookDir
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}


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

    [Header("Inventory / Load")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private int booksUntilCantMove = 10; // при этом кол-ве книг нельзя двигаться

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Anim")]
    [SerializeField] private Animator animator;

    private LookDir lastDir = LookDir.Down;

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
        moveInput = moveAction.action.ReadValue<Vector2>();
        isSprinting = sprintAction.action.IsPressed() && currentStamina > 0f;

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

    LookDir Get8Dir(Vector2 dir)
    {
        if (dir.x > 0 && dir.y > 0) return LookDir.Up;        // NE
        if (dir.x < 0 && dir.y > 0) return LookDir.Up;        // NW
        if (dir.x > 0 && dir.y < 0) return LookDir.Down;      // SE
        if (dir.x < 0 && dir.y < 0) return LookDir.Down;      // SW

        if (dir.x > 0) return LookDir.Right;
        if (dir.x < 0) return LookDir.Left;
        if (dir.y > 0) return LookDir.Up;
        return LookDir.Down;
    }


    void UpdateVisualFromDelta(Vector2 delta)
    {
        if (delta.sqrMagnitude < 0.0001f)
            return;

        Vector2 dir = delta.normalized;

        // Округляем к ближайшему из 8 направлений
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);

        animator.SetFloat("InputX", dir.x);
        animator.SetFloat("InputY", dir.y);

        lastDir = Get8Dir(dir);
    }

    private void HandleMovement()
    {
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);

        int books = playerInventory != null ? playerInventory.slots.Length : 0;

        if (books >= booksUntilCantMove)
        {
            currentSpeed = 0f;
            controller.Move(Vector3.zero);
            isActuallyMoving = false;
            return;
        }

        float loadPercent = Mathf.Clamp01((float)books / booksUntilCantMove);

        float walkSpeed = baseMoveSpeed;

        float sprintSpeed = Mathf.Lerp(baseMoveSpeed * sprintMultiplier, walkSpeed, loadPercent);

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);

        Vector3 direction = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * currentSpeed;
        }

        // Гравитация
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        direction.y = verticalVelocity;

        // Движение
        Vector3 beforeMove = controller.transform.position;
        controller.Move(direction * Time.deltaTime);
        Vector3 afterMove = controller.transform.position;

        // Проверка, движемся ли мы реально (не врезаемся ли в стену)
        Vector3 delta = afterMove - beforeMove;
        delta.y = 0;
        isActuallyMoving = delta.magnitude > 0.001f;

        UpdateVisualFromDelta(new Vector2(delta.x, delta.z));
    }

    private void HandleStamina()
    {
        float staminaDrain = 0f;

        // Стамина тратится только если игрок ДЕЙСТВИТЕЛЬНО движется
        if (moveInput.magnitude > 0.1f && isActuallyMoving)
        {
            staminaDrain = isSprinting ? staminaDrainPerSecond : staminaDrainPerSecond * 0.4f;
        }

        currentStamina -= staminaDrain * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // Восстановление, если не двигаемся
        if (moveInput.magnitude <= 0.1f || !isActuallyMoving)
            currentStamina += staminaRecoverPerSecond * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // UI
        float percent = currentStamina / maxStamina;
        Color gradientColor = Color.Lerp(normalColor, lowStaminaColor, 1f - percent);

        if (percent < 0.1f)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            Color blinkColor = new Color(lowStaminaColor.r, lowStaminaColor.g, lowStaminaColor.b, 0f);
            gradientColor = Color.Lerp(gradientColor, blinkColor, t);
        }

        staminaBar.color = gradientColor;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    public float GetCurrentStaminaPercent()
    {
        return currentStamina / maxStamina;
    }
}
