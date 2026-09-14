using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Скорость базового перемещения персонажа (м/с)")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("Скорость сглаживания поворота персонажа")]
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [Header("Physics Settings")]
    [Tooltip("Сила гравитации при нахождении в воздухе")]
    [SerializeField] private float gravity = -19.62f;

    [Header("References")]
    [Tooltip("Ссылка на основную камеру для движения относительно ракурса")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private Vector3 verticalVelocity;
    private float turnSmoothVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleGravity();
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Расчет угла поворота относительно ориентации камеры
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Направление движения в мировых координатах
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f; // Прижим к земле на спусках
        }
        else
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }
}