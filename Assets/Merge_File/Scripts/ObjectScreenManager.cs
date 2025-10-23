using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectScreenManager : MonoBehaviour
{
    [Header("UI 버튼들")]
    public Button metalPlateButton;
    public Button metalstickButton;
    public Button cylinderButton;
    public Button startButton;

    [Header("생성될 용접 객체 프리팹")]
    public GameObject metalPlatePrefab;
    public GameObject metalstickPrefab;
    public GameObject cylinderPrefab;

    [Header("용접 도구")]
    [Tooltip("Start 버튼 클릭 시 생성될 용접기 프리팹")]
    public GameObject welderPrefab;

    [Header("UI 요소")]
    [Tooltip("Start 버튼 클릭 시 사라지게 할 캔버스")]
    public GameObject objectScreenCanvas;

    [Header("생성 위치")]
    [Tooltip("프리팹이 생성될 기준 위치")]
    public Transform spawnPoint;

    [Tooltip("용접 객체(판, 봉 등)의 생성 위치 오프셋")]
    public Vector3 spawnOffset;

    [Tooltip("용접 도구만의 별도 생성 위치 오프셋")]
    public Vector3 welderSpawnOffset; // 용접기 위치만 따로 조절하기 위한 변수

    private GameObject currentWeldingObject;

    void Start()
    {
        if (welderPrefab == null)
        {
            Debug.LogWarning("[UIManager] 'welderPrefab'가 할당되지 않았습니다. Start 버튼이 작동하지 않습니다.");
        }

        if (objectScreenCanvas == null)
        {
            Debug.LogWarning("[UIManager] 'objectScreenCanvas'가 할당되지 않았습니다. 캔버스를 숨길 수 없습니다.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[UIManager] 'spawnPoint'가 할당되지 않았습니다. 프리팹이 생성되지 않습니다!");
        }

        if (metalPlateButton != null)
        {
            metalPlateButton.onClick.AddListener(() => SpawnWeldingObject(metalPlatePrefab));
        }
        else Debug.LogWarning("[UIManager] Metal Plate Button이 할당되지 않았습니다.");


        if (metalstickButton != null)
        {
            metalstickButton.onClick.AddListener(() => SpawnWeldingObject(metalstickPrefab));
        }
        else Debug.LogWarning("[UIManager] Metalstick Button이 할당되지 않았습니다.");


        if (cylinderButton != null)
        {
            cylinderButton.onClick.AddListener(() => SpawnWeldingObject(cylinderPrefab));
        }
        else Debug.LogWarning("[UIManager] Cylinder Button이 할당되지 않았습니다.");


        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClick);
        }
        else Debug.LogWarning("[UIManager] Start Button이 할당되지 않았습니다.");

        Debug.Log("[UIManager] 모든 버튼 리스너 설정이 완료되었습니다.");
    }

    public void OnStartClick()
    {
        Debug.Log("[UIManager] START 버튼 클릭! 용접 도구를 생성하고 UI를 숨깁니다.");

        if (welderPrefab != null)
        {
            // 용접기 생성 위치 = 기준점 + 용접기 전용 오프셋
            Vector3 welderPosition = spawnPoint.position + welderSpawnOffset;
            Instantiate(welderPrefab, welderPosition, spawnPoint.rotation);
        }

        if (objectScreenCanvas != null)
        {
            objectScreenCanvas.SetActive(false);
        }
    }

    private void SpawnWeldingObject(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("[UIManager] 생성하려는 프리팹이 null 입니다. Inspector 설정을 확인해주세요.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[UIManager] SpawnPoint가 할당되지 않아 프리팹을 생성할 수 없습니다.");
            return;
        }

        if (currentWeldingObject != null)
        {
            Debug.Log($"[UIManager] 이전 객체 '{currentWeldingObject.name}'를 삭제합니다.");
            Destroy(currentWeldingObject);
        }

        // 용접 객체 생성 위치 = 기준점 + 일반 오프셋
        Vector3 finalSpawnPosition = spawnPoint.position + spawnOffset;

        Debug.Log($"[UIManager] '{prefabToSpawn.name}' 프리팹을 생성합니다. 기준 위치: {spawnPoint.position}, 오프셋: {spawnOffset}, 최종 위치: {finalSpawnPosition}");

        currentWeldingObject = Instantiate(prefabToSpawn, finalSpawnPosition, spawnPoint.rotation);

        currentWeldingObject.name = prefabToSpawn.name;
    }
}