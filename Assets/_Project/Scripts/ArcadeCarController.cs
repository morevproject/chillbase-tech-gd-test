using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeCarController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Дата-ассет с параметрами физики и баланса автомобиля")]
    [SerializeField] private CarSettingsSO settings;

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
        if (!isControlled || settings == null)
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
        if (!isGrounded || settings == null) return;

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
                Vector3 brakeVector = -transform.forward * Mathf.Sign(forwardSpeed) * settings.brakeStrength;
                rb.AddForce(brakeVector, ForceMode.Acceleration);
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
            return;
        }

        if (moveInput > 0.05f && forwardSpeed < settings.maxSpeed)
        {
            rb.AddForce(transform.forward * (moveInput * settings.acceleration), ForceMode.Acceleration);
        }
        else if (moveInput < -0.05f && forwardSpeed > -settings.reverseSpeed)
        {
            rb.AddForce(transform.forward * (moveInput * settings.acceleration), ForceMode.Acceleration);
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
            float turnAmount = steerInput * settings.turnSpeed * directionModifier * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.AngleAxis(turnAmount, transform.up);
            rb.MoveRotation(turnRotation * rb.rotation);
        }

        // 2. Мягкое выравнивание кузова по уклону рампы/горы
        Vector3 currentUp = transform.up;
        Vector3 targetUp = groundHit.normal;
        Quaternion tiltCorrection = Quaternion.FromToRotation(currentUp, targetUp);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, tiltCorrection * rb.rotation, Time.fixedDeltaTime * settings.slopeAlignSpeed));
    }

    private void ApplyLateralGrip()
    {
        Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right);
        rb.velocity -= lateralVelocity * (settings.gripFactor * Time.fixedDeltaTime * 10f);
    }
}