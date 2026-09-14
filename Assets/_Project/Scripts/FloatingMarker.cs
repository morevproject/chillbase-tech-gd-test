using UnityEngine;

public class FloatingMarker : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Скорость вращения вокруг вертикальной оси (град/сек)")]
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Bobbing (Hover)")]
    [Tooltip("Амплитуда колебания вверх-вниз (в метрах)")]
    [SerializeField] private float bobAmplitude = 0.35f;

    [Tooltip("Частота колебаний (скорость движения вверх-вниз)")]
    [SerializeField] private float bobFrequency = 2.5f;

    private Vector3 initialLocalPos;

    private void Awake()
    {
        initialLocalPos = transform.localPosition;
    }

    private void Update()
    {
        // 1. Плавное вращение вокруг оси Y
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // 2. Синусоидальное парение вверх-вниз
        float newY = initialLocalPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);
    }
}