using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObjectScreenManager : MonoBehaviour
{
    [Header("UI ��ư��")]
    public Button metalPlateButton;
    public Button metalstickButton;
    public Button cylinderButton;
    public Button startButton;

    [Header("������ ���� ��ü ������")]
    public GameObject metalPlatePrefab;
    public GameObject metalstickPrefab;
    public GameObject cylinderPrefab;

    [Header("���� ����")]
    [Tooltip("Start ��ư Ŭ�� �� ������ ������ ������")]
    public GameObject welderPrefab;

    [Header("UI ���")]
    [Tooltip("Start ��ư Ŭ�� �� ������� �� ĵ����")]
    public GameObject objectScreenCanvas;

    [Header("���� ��ġ")]
    [Tooltip("�������� ������ ���� ��ġ")]
    public Transform spawnPoint;

    [Tooltip("���� ��ü(��, �� ��)�� ���� ��ġ ������")]
    public Vector3 spawnOffset;

    [Tooltip("���� �������� ���� ���� ��ġ ������")]
    public Vector3 welderSpawnOffset; // ������ ��ġ�� ���� �����ϱ� ���� ����

    private GameObject currentWeldingObject;

    void Start()
    {
        if (welderPrefab == null)
        {
            Debug.LogWarning("[UIManager] 'welderPrefab'�� �Ҵ���� �ʾҽ��ϴ�. Start ��ư�� �۵����� �ʽ��ϴ�.");
        }

        if (objectScreenCanvas == null)
        {
            Debug.LogWarning("[UIManager] 'objectScreenCanvas'�� �Ҵ���� �ʾҽ��ϴ�. ĵ������ ���� �� �����ϴ�.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[UIManager] 'spawnPoint'�� �Ҵ���� �ʾҽ��ϴ�. �������� �������� �ʽ��ϴ�!");
        }

        if (metalPlateButton != null)
        {
            metalPlateButton.onClick.AddListener(() => SpawnWeldingObject(metalPlatePrefab));
        }
        else Debug.LogWarning("[UIManager] Metal Plate Button�� �Ҵ���� �ʾҽ��ϴ�.");


        if (metalstickButton != null)
        {
            metalstickButton.onClick.AddListener(() => SpawnWeldingObject(metalstickPrefab));
        }
        else Debug.LogWarning("[UIManager] Metalstick Button�� �Ҵ���� �ʾҽ��ϴ�.");


        if (cylinderButton != null)
        {
            cylinderButton.onClick.AddListener(() => SpawnWeldingObject(cylinderPrefab));
        }
        else Debug.LogWarning("[UIManager] Cylinder Button�� �Ҵ���� �ʾҽ��ϴ�.");


        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClick);
        }
        else Debug.LogWarning("[UIManager] Start Button�� �Ҵ���� �ʾҽ��ϴ�.");

        Debug.Log("[UIManager] ��� ��ư ������ ������ �Ϸ�Ǿ����ϴ�.");
    }

    public void OnStartClick()
    {
        Debug.Log("[UIManager] START 버튼 클릭! 용접 장비를 소환하고 UI를 숨깁니다.");

        // 프리팹이 선택되었는지 확인
        if (currentWeldingObject == null)
        {
            Debug.LogWarning("[UIManager] 프리팹을 먼저 선택해주세요!");
            return;
        }

        // Start 버튼을 눌렀을 때 게임 시작: 큐브 카운트 및 타이머 시작
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeWeldingGuides();
            Debug.Log("[UIManager] ★★★ START! 게임 시작 - 큐브 카운트 완료 ★★★");
        }

        if (welderPrefab != null)
        {
            // 용접기 소환 위치 = 스폰포인트 + 용접기 전용 오프셋
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
            Debug.LogError("[UIManager] �����Ϸ��� �������� null �Դϴ�. Inspector ������ Ȯ�����ּ���.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[UIManager] SpawnPoint�� �Ҵ���� �ʾ� �������� ������ �� �����ϴ�.");
            return;
        }

        if (currentWeldingObject != null)
        {
            Debug.Log($"[UIManager] ���� ��ü '{currentWeldingObject.name}'�� �����մϴ�.");
            Destroy(currentWeldingObject);
        }

        // ���� ��ü ���� ��ġ = ������ + �Ϲ� ������
        Vector3 finalSpawnPosition = spawnPoint.position + spawnOffset;

        Debug.Log($"[UIManager] '{prefabToSpawn.name}' �������� �����մϴ�. ���� ��ġ: {spawnPoint.position}, ������: {spawnOffset}, ���� ��ġ: {finalSpawnPosition}");

        currentWeldingObject = Instantiate(prefabToSpawn, finalSpawnPosition, spawnPoint.rotation);

        currentWeldingObject.name = prefabToSpawn.name;

        Debug.Log($"[UIManager] '{prefabToSpawn.name}' 프리팹 소환 완료! START 버튼을 눌러 게임을 시작하세요.");
    }
}