using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Настройки точки")]
    [Tooltip("Название точки для отображения в интерфейсе")]
    public string waypointName = "Контрольная точка";

    [Tooltip("Радиус регистрации прохождения (в метрах)")]
    public float triggerRadius = 7f;

    [Header("Визуал")]
    [Tooltip("Визуальный маркер в мире (подсвечивается, когда точка активна)")]
    [SerializeField] private GameObject visualMarker;

    public void SetActive(bool isActive)
    {
        if (visualMarker != null)
        {
            visualMarker.SetActive(isActive);
        }
    }

    private void OnDrawGizmos()
    {
        // Отображение радиуса точки в окне Scene
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}