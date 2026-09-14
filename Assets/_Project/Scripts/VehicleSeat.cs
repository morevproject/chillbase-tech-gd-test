using UnityEngine;

public class VehicleSeat : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Ссылка на контроллер этого автомобиля")]
    [SerializeField] private ArcadeCarController carController;

    [Tooltip("Точка высадки водителя (слева от двери)")]
    [SerializeField] private Transform exitPoint;

    [Header("Keybindings")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Настройки")]
    [Tooltip("Максимальная дистанция для взаимодействия (страховка от багов триггера)")]
    [SerializeField] private float maxInteractDistance = 3.5f;

    private GameObject playerInside;
    private ThirdPersonCamera mainCamera;
    private bool isPlayerNearby = false;
    private GameObject nearbyPlayer;

    private void Awake()
    {
        if (carController == null) carController = GetComponentInParent<ArcadeCarController>();
        if (mainCamera == null && Camera.main != null) mainCamera = Camera.main.GetComponent<ThirdPersonCamera>();
    }

    private void Update()
    {
        // Страховка: если игрок отошел от машины дальше допустимого, гарантированно гасим флаг
        if (!playerInside && nearbyPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, nearbyPlayer.transform.position);
            if (dist > maxInteractDistance)
            {
                isPlayerNearby = false;
                nearbyPlayer = null;
            }
        }

        // Посадка
        if (isPlayerNearby && playerInside == null && Input.GetKeyDown(interactKey))
        {
            EnterCar(nearbyPlayer);
        }
        // Высадка
        else if (playerInside != null && Input.GetKeyDown(interactKey))
        {
            ExitCar();
        }
    }

    private void EnterCar(GameObject player)
    {
        playerInside = player;
        playerInside.SetActive(false);
        isPlayerNearby = false;

        carController.isControlled = true;

        if (mainCamera != null)
        {
            mainCamera.SetTarget(carController.transform);
        }
    }

    private void ExitCar()
    {
        Vector3 spawnPos = exitPoint != null ? exitPoint.position : transform.position + transform.right * -2.5f;
        isPlayerNearby = false;
        
        playerInside.transform.position = spawnPos;
        playerInside.SetActive(true);

        carController.isControlled = false;

        if (mainCamera != null)
        {
            mainCamera.SetTarget(playerInside.transform);
        }

        playerInside = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            isPlayerNearby = true;
            nearbyPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == nearbyPlayer)
        {
            isPlayerNearby = false;
            nearbyPlayer = null;
        }
    }

    private void OnGUI()
    {
        GUIStyle hintStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        // Подсказка на посадку при входе в триггер двери
        if (isPlayerNearby && !playerInside)
        {
            Rect enterRect = new Rect(Screen.width / 2f - 110f, Screen.height - 90f, 220f, 40f);
            GUI.color = Color.white;
            GUI.Box(enterRect, "Сесть [F]", hintStyle);
        }
        // Подсказка на высадку во время управления
        else if (playerInside)
        {
            Rect exitRect = new Rect(Screen.width / 2f - 110f, Screen.height - 70f, 220f, 32f);
            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            GUI.Box(exitRect, "Выйти [F]", hintStyle);
        }
    }
}