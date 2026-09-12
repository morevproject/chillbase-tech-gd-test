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

        carController.isControlled = true;

        if (mainCamera != null)
        {
            mainCamera.SetTarget(carController.transform);
        }
    }

    private void ExitCar()
    {
        Vector3 spawnPos = exitPoint != null ? exitPoint.position : transform.position + transform.right * -2.5f;
        
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
}