using UnityEngine;

public class WeldingGuideColorChanger : MonoBehaviour
{
    // 색상 재질들
    public Material greenMaterial;  // 완벽한 용접 (4회 이상)
    public Material yellowMaterial; // 중간 용접 (2~3회)
    private MeshRenderer meshRenderer;

    // 용접 상태를 추적하는 공개 속성
    public bool IsWelded { get; private set; } = false;

    // 용접 접촉 횟수
    private int contactCount = 0;
    private bool isInContact = false;

    // GameManager에 알림을 보냈는지 여부
    private bool hasNotifiedManager = false;

    // 완벽한 용접(초록색) 달성 여부
    private bool isPerfectWeld = false;

    // 용접 횟수 설정
    [Header("용접 횟수 설정")]
    [Tooltip("노란색으로 변경되는 최소 접촉 횟수")]
    public int yellowContactCount = 1;

    [Tooltip("초록색으로 변경되는 최소 접촉 횟수")]
    public int greenContactCount = 3;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // 용접봉이 뚫고 들어가지 않도록 물리적 Collider 추가
        EnsurePhysicalCollider();
    }

    void EnsurePhysicalCollider()
    {
        // 이미 있는 Collider들 확인
        Collider[] colliders = GetComponents<Collider>();

        // Non-Trigger Collider가 있는지 확인
        bool hasPhysicalCollider = false;
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                hasPhysicalCollider = true;
                break;
            }
        }

        // 물리적 Collider가 없으면 추가
        if (!hasPhysicalCollider)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                // MeshCollider 추가 (물리적으로 막기용)
                MeshCollider physicalCollider = gameObject.AddComponent<MeshCollider>();
                physicalCollider.convex = true; // Convex로 설정해야 물리 충돌 가능
                physicalCollider.isTrigger = false; // 물리적으로 막음
                Debug.Log($"[{gameObject.name}] 물리적 MeshCollider 추가 (용접봉 뚫림 방지)");
            }
            else
            {
                // MeshFilter가 없으면 BoxCollider 추가
                BoxCollider physicalCollider = gameObject.AddComponent<BoxCollider>();
                physicalCollider.isTrigger = false;
                Debug.Log($"[{gameObject.name}] 물리적 BoxCollider 추가 (용접봉 뚫림 방지)");
            }
        }
    }

    // 접촉 횟수에 따라 색상 변경 및 상태 설정
    public void UpdateWeldingProgress()
    {
        if (meshRenderer != null)
        {
            // 3회 이상 접촉 - 완벽한 용접 (초록색)
            if (contactCount >= greenContactCount)
            {
                if (greenMaterial != null)
                {
                    meshRenderer.material = greenMaterial;
                }

                // 초록색이 되면 완료로 표시
                if (!IsWelded)
                {
                    IsWelded = true;
                    Debug.Log($"[{gameObject.name}] ✅ 완벽한 용접! 접촉 횟수: {contactCount}회 → 초록색으로 변경!");
                }
                else
                {
                    Debug.Log($"[{gameObject.name}] 초록색 유지 중 (접촉 {contactCount}회)");
                }

                // 처음 초록색이 되었을 때 GameManager의 perfectWeldCount 증가
                if (!isPerfectWeld && GameManager.Instance != null)
                {
                    isPerfectWeld = true;
                    GameManager.Instance.OnPerfectWeld();
                    Debug.Log($"[{gameObject.name}] ★ 완벽한 용접 달성! (초록색)");
                }

                // GameManager에 한 번만 알림 (중복 방지)
                if (!hasNotifiedManager)
                {
                    if (GameManager.Instance != null)
                    {
                        hasNotifiedManager = true;
                        GameManager.Instance.OnGuideWelded(this);
                        Debug.Log($"[{gameObject.name}] ★ GameManager에 용접 완료 알림 - 초록색 (완벽)");
                    }
                    else
                    {
                        Debug.LogError($"[{gameObject.name}] ⚠️ GameManager.Instance가 null입니다!");
                    }
                }
            }
            // 1회 이상 접촉 - 중간 용접 (노란색) - 이것도 완료로 인정
            else if (contactCount >= yellowContactCount)
            {
                if (yellowMaterial != null)
                {
                    meshRenderer.material = yellowMaterial;
                }

                // 노란색이 되면 완료로 표시 (초록색으로 계속 변할 수 있음)
                if (!IsWelded)
                {
                    IsWelded = true;
                    Debug.Log($"[{gameObject.name}] ⚠️ 중간 용접 완료! 접촉 횟수: {contactCount}회 → 노란색으로 변경 (초록색까지 {greenContactCount - contactCount}회 더 필요)");
                }
                else
                {
                    Debug.Log($"[{gameObject.name}] 노란색 유지 중 (접촉 {contactCount}회, 초록색까지 {greenContactCount - contactCount}회 더 필요)");
                }

                // GameManager에 한 번만 알림 (중복 방지)
                if (!hasNotifiedManager)
                {
                    if (GameManager.Instance != null)
                    {
                        hasNotifiedManager = true;
                        GameManager.Instance.OnGuideWelded(this);
                        Debug.Log($"[{gameObject.name}] ★ GameManager에 용접 완료 알림 - 노란색 (중간)");
                    }
                    else
                    {
                        Debug.LogError($"[{gameObject.name}] ⚠️ GameManager.Instance가 null입니다!");
                    }
                }
            }
        }
    }

    // 용접 시작 (접촉 시작)
    public void StartWelding()
    {
        // 이미 접촉 중이 아닐 때만 카운트 증가 (IsWelded 체크 제거!)
        if (!isInContact)
        {
            isInContact = true;
            contactCount++;
            Debug.Log($"[{gameObject.name}] 용접 접촉 시작 - 현재 {contactCount}회 / 노란색: {yellowContactCount}회 / 초록색: {greenContactCount}회");
            UpdateWeldingProgress();
        }
    }

    // 용접 종료 (접촉 종료)
    public void EndWelding()
    {
        if (isInContact)
        {
            isInContact = false;
            Debug.Log($"용접 접촉 종료 - 총 접촉 횟수: {contactCount}회");
        }
    }

    // 접촉 횟수 확인 (디버깅용)
    public int GetContactCount()
    {
        return contactCount;
    }
}