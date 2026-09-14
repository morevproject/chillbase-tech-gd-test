using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings_Default", menuName = "Game/Vehicle/Car Settings")]
public class CarSettingsSO : ScriptableObject
{
    [Header("Engine & Speed")]
    [Tooltip("Ускорение автомобиля (м/с²)")]
    public float acceleration = 32f;

    [Tooltip("Максимальная скорость вперед (м/с)")]
    public float maxSpeed = 26f;

    [Tooltip("Максимальная скорость назад (м/с)")]
    public float reverseSpeed = 12f;

    [Tooltip("Сила торможения (м/с²)")]
    public float brakeStrength = 45f;

    [Header("Steering & Handling")]
    [Tooltip("Скорость поворота кузова (градусов/сек)")]
    public float turnSpeed = 130f;

    [Tooltip("Коэффициент бокового сцепления (0 - лед, 1 - рельсы)")]
    [Range(0.1f, 1f)]
    public float gripFactor = 0.88f;

    [Tooltip("Скорость подстройки наклона под рельеф")]
    public float slopeAlignSpeed = 15f;
}