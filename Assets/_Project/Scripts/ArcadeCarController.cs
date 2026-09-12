using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    [Header("Engine & Speed")]
    [Tooltip("Ускорение автомобиля (м/с²)")]
    [SerializeField] private float acceleration = 32f;

    [Tooltip("Максимальная скорость вперед (м/с)")]
    [SerializeField] private float maxSpeed = 26f;

    [Tooltip("Максимальная скорость назад (м/с)")]
    [SerializeField] private float reverseSpeed = 12f;

    [Tooltip("Сила торможения (м/с²)")]
    [SerializeField] private float brakeStrength = 45f;

    [Header("Steering & Handling")]
    [Tooltip("Скорость поворота кузова (градусов/сек)")]
    [SerializeField] private float turnSpeed = 130f;

    [Tooltip("Коэффициент бокового сцепления (0 - лед, 1 - рельсы)")]
    [Range(0.1f, 1f)]
    [SerializeField] private float gripFactor = 0.88f;

    [Tooltip("Скорость подстройки наклона под рельеф")]
    [SerializeField] private float slopeAlignSpeed = 15f;

    [Header("State")]
    public bool isControlled = false;

    private Rigidbody rb;
    private float moveInput;
    private float steerInput;
    private bool isBraking;
    private RaycastHit groundHit;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isControlled)
        {
            moveInput = 0f;
            steerInput = 0f;
            isBraking = true;
            return;
        }

        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;

        isGrounded = CheckGround();
        if (!isGrounded) return;

        ApplyMovement();
        ApplySteering();
        ApplyLateralGrip();
    }

    private bool CheckGround()
    {
        // Проверка контакта с землей чуть ниже днища
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out groundHit, 1.8f, ~0, QueryTriggerInteraction.Ignore);
    }

    private void ApplyMovement()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);

        if (isBraking)
        {
            if (Mathf.Abs(forwardSpeed) > 0.1f)
            {
                Vector3 brakeVector = -transform.forward * Mathf.Sign(forwardSpeed) * brakeStrength;
                rb.AddForce(brakeVector, ForceMode.Acceleration);
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
            return;
        }

        if (moveInput > 0.05f && forwardSpeed < maxSpeed)
        {
            rb.AddForce(transform.forward * (moveInput * acceleration), ForceMode.Acceleration);
        }
        else if (moveInput < -0.05f && forwardSpeed > -reverseSpeed)
        {
            rb.AddForce(transform.forward * (moveInput * acceleration), ForceMode.Acceleration);
        }
        else if (Mathf.Abs(moveInput) <= 0.05f)
        {
            // Мягкий накат при отпущенном газе
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.fixedDeltaTime * 1.5f);
        }
    }

    private void ApplySteering()
    {
        float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);

        // 1. Мгновенный отзывчивый поворот руля вокруг локальной оси вверх
        if (Mathf.Abs(steerInput) > 0.05f && (Mathf.Abs(forwardSpeed) > 0.1f || Mathf.Abs(moveInput) > 0.05f))
        {
            float directionModifier = forwardSpeed >= -0.1f ? 1f : -1f;
            float turnAmount = steerInput * turnSpeed * directionModifier * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.AngleAxis(turnAmount, transform.up);
            rb.MoveRotation(turnRotation * rb.rotation);
        }

        // 2. Мягкое выравнивание кузова по уклону рампы/горы
        Vector3 currentUp = transform.up;
        Vector3 targetUp = groundHit.normal;
        Quaternion tiltCorrection = Quaternion.FromToRotation(currentUp, targetUp);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, tiltCorrection * rb.rotation, Time.fixedDeltaTime * slopeAlignSpeed));
    }

    private void ApplyLateralGrip()
    {
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        rb.velocity -= lateralVelocity * (gripFactor * Time.fixedDeltaTime * 10f);
    }
}