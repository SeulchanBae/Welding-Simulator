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
        Debug.Log("[UIManager] START ��ư Ŭ��! ���� ������ �����ϰ� UI�� ����ϴ�.");

        if (welderPrefab != null)
        {
            // ������ ���� ��ġ = ������ + ������ ���� ������
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

        // 프리팹 소환 후 GameManager에게 가이드 개수를 다시 세도록 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeWeldingGuides();
            Debug.Log("[UIManager] 프리팹 소환 완료 후 GameManager에 가이드 초기화 요청");
        }
    }
}