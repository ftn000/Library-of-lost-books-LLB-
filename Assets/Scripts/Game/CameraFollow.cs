using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // игрок

    [Header("Offset & Smooth")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 5f, -7f);
    [SerializeField] private float smoothTime = 0.3f;

    [Header("Movement Ahead")]
    [SerializeField] private float sprintForwardOffset = 2f; // смещение вперёд при беге
    [SerializeField] private float walkForwardOffset = 0.5f;  // смещение вперёд при ходьбе

    [Header("Room Bounds")]
    [SerializeField] private Vector2 xBounds = new Vector2(-5f, 5f);
    [SerializeField] private Vector2 zBounds = new Vector2(-5f, 5f);

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastTargetPosition;

    // Ссылка на скрипт игрока, чтобы проверить, бежит ли он
    [SerializeField] private PlayerMovementWithStamina playerMovement;

    void Start()
    {
        if (target != null)
            lastTargetPosition = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Направление движения игрока
        Vector3 movement = target.position - lastTargetPosition;
        Vector3 horizontalMovement = new Vector3(movement.x, 0f, movement.z);

        // Выбираем смещение вперёд в зависимости от того, бежит игрок или просто ходит
        float forwardMultiplier = walkForwardOffset;
        if (playerMovement != null && playerMovement.IsSprinting())
            forwardMultiplier = sprintForwardOffset;

        Vector3 dynamicOffset = baseOffset + horizontalMovement * forwardMultiplier;

        // Желаемая позиция камеры
        Vector3 desiredPosition = target.position + dynamicOffset;

        // Ограничение по комнате
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, xBounds.x + baseOffset.x, xBounds.y + baseOffset.x);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, zBounds.x + baseOffset.z, zBounds.y + baseOffset.z);

        // Плавное движение камеры
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Камера смотрит на игрока
        transform.LookAt(target.position + Vector3.up * 1f);

        lastTargetPosition = target.position;
    }
}
