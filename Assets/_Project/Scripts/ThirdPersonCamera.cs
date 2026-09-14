using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target & Positioning")]
    [Tooltip("Объект слежения (персонаж или машина)")]
    public Transform target;

    [Tooltip("Смещение фокуса относительно цели (например, чуть выше центра)")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Tooltip("Дистанция от камеры до цели")]
    [SerializeField] private float distance = 5f;

    [Header("Rotation Settings")]
    [Tooltip("Чувствительность мыши по осям X и Y")]
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(3f, 2f);

    [Tooltip("Ограничение угла наклона по вертикали")]
    [SerializeField] private Vector2 pitchLimits = new Vector2(-20f, 60f);

    [Tooltip("Плавность слежения камеры")]
    [SerializeField] private float followSmoothTime = 0.05f;

    private float currentYaw;
    private float currentPitch = 15f;
    private Vector3 currentVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Вращение мышью
        currentYaw += Input.GetAxis("Mouse X") * mouseSensitivity.x;
        currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity.y;
        currentPitch = Mathf.Clamp(currentPitch, pitchLimits.x, pitchLimits.y);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 desiredPosition = (target.position + targetOffset) - (rotation * Vector3.forward * distance);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSmoothTime);
        transform.LookAt(target.position + targetOffset);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}