using System.Collections.Generic;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    [Header("Маршрут")]
    [Tooltip("Последовательный список точек. Порядок можно свободно менять в Инспекторе.")]
    [SerializeField] private List<Waypoint> waypoints = new List<Waypoint>();

    [Header("Ссылки")]
    [Tooltip("Камера со скриптом ThirdPersonCamera для отслеживания текущего активного игрока/машины")]
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private int currentWaypointIndex = 0;
    private bool isRouteCompleted = false;
    private float currentDistance = 0f;

    private void Start()
    {
        if (thirdPersonCamera == null && Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        InitializeRoute();
    }

    private void InitializeRoute()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("[RouteManager] Список точек маршрута пуст!", this);
            return;
        }

        // Активируем маркер только у первой точки
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                waypoints[i].SetActive(i == 0);
            }
        }
    }

    private void Update()
    {
        if (isRouteCompleted || waypoints.Count == 0) return;

        Transform activeTarget = GetActiveTarget();
        if (activeTarget == null) return;

        Waypoint currentWp = waypoints[currentWaypointIndex];
        if (currentWp == null) return;

        currentDistance = Vector3.Distance(activeTarget.position, currentWp.transform.position);

        // Проверка достижения текущей точки
        if (currentDistance <= currentWp.triggerRadius)
        {
            AdvanceToNextWaypoint();
        }
    }

    private void AdvanceToNextWaypoint()
    {
        // Выключаем визуал пройденной точки
        if (waypoints[currentWaypointIndex] != null)
        {
            waypoints[currentWaypointIndex].SetActive(false);
        }

        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Count)
        {
            isRouteCompleted = true;
            Debug.Log("[RouteManager] Маршрут успешно пройден!");
        }
        else
        {
            // Включаем маркер следующей цели
            if (waypoints[currentWaypointIndex] != null)
            {
                waypoints[currentWaypointIndex].SetActive(true);
            }
        }
    }

    private Transform GetActiveTarget()
    {
        if (thirdPersonCamera != null && thirdPersonCamera.target != null)
        {
            return thirdPersonCamera.target;
        }
        return null;
    }

    // Экранный интерфейс навигации (не требует ручного создания UI Canvas)
    private void OnGUI()
    {
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        Rect uiRect = new Rect(Screen.width / 2f - 220f, 20f, 440f, 50f);

        if (isRouteCompleted)
        {
            GUI.color = Color.green;
            GUI.Box(uiRect, "МАРШРУТ ЗАВЕРШЕН!", boxStyle);
        }
        else if (waypoints.Count > 0 && currentWaypointIndex < waypoints.Count)
        {
            Waypoint wp = waypoints[currentWaypointIndex];
            GUI.color = Color.white;
            string text = $"ЦЕЛЬ [{currentWaypointIndex + 1}/{waypoints.Count}]: {wp.waypointName} ({currentDistance:F0} м)";
            GUI.Box(uiRect, text, boxStyle);
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
            }
        }
    }
}