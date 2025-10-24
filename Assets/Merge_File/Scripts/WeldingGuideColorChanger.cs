using UnityEngine;

public class WeldingGuideColorChanger : MonoBehaviour
{
    // 색상 재질들
    public Material greenMaterial;  // 완벽한 용접 (초록색)
    public Material yellowMaterial; // 중간 용접 (노란색)
    private MeshRenderer meshRenderer;

    // 용접 상태
    public bool IsWelded { get; private set; } = false;
    private int contactCount = 0;
    private bool isInContact = false;

    // 색상 변경 기준
    [Header("용접 횟수 설정")]
    [Tooltip("노란색으로 변경되는 최소 접촉 횟수")]
    public int yellowContactCount = 1;

    [Tooltip("초록색으로 변경되는 최소 접촉 횟수")]
    public int greenContactCount = 3;

    // 현재 색상 상태 (중복 알림 방지용)
    private enum ColorState { Default, Yellow, Green }
    private ColorState currentColorState = ColorState.Default;

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
                MeshCollider physicalCollider = gameObject.AddComponent<MeshCollider>();
                physicalCollider.convex = true;
                physicalCollider.isTrigger = false;
                Debug.Log($"[{gameObject.name}] 물리적 MeshCollider 추가 (용접봉 뚫림 방지)");
            }
            else
            {
                BoxCollider physicalCollider = gameObject.AddComponent<BoxCollider>();
                physicalCollider.isTrigger = false;
                Debug.Log($"[{gameObject.name}] 물리적 BoxCollider 추가 (용접봉 뚫림 방지)");
            }
        }
    }

    // 접촉 횟수에 따라 색상 변경
    public void UpdateWeldingProgress()
    {
        if (meshRenderer == null || GameManager.Instance == null)
            return;

        // 초록색으로 변경 (완벽한 용접)
        if (contactCount >= greenContactCount && currentColorState != ColorState.Green)
        {
            if (greenMaterial != null)
            {
                meshRenderer.material = greenMaterial;
            }

            // 노란색에서 초록색으로 업그레이드되는 경우
            bool wasYellow = (currentColorState == ColorState.Yellow);

            currentColorState = ColorState.Green;
            IsWelded = true;

            // 완벽한 용접 달성 알림
            GameManager.Instance.OnPerfectWeld();

            // 노란색에서 업그레이드된 경우는 OnGuideWelded 호출 안 함 (이미 노란색일 때 호출했음)
            if (!wasYellow)
            {
                GameManager.Instance.OnGuideWelded(this);
            }

            Debug.Log($"[{gameObject.name}] ✅ 완벽한 용접! (초록색) - 접촉 {contactCount}회");
        }
        // 노란색으로 변경 (중간 용접)
        else if (contactCount >= yellowContactCount && currentColorState == ColorState.Default)
        {
            if (yellowMaterial != null)
            {
                meshRenderer.material = yellowMaterial;
            }

            currentColorState = ColorState.Yellow;
            IsWelded = true;

            // 중간 용접 완료 알림 (처음 완료되었을 때만)
            GameManager.Instance.OnGuideWelded(this);

            Debug.Log($"[{gameObject.name}] ⚠️ 중간 용접 완료 (노란색) - 접촉 {contactCount}회 (초록색까지 {greenContactCount - contactCount}회 더 필요)");
        }
    }

    // 용접 시작 (접촉 시작)
    public void StartWelding()
    {
        if (!isInContact)
        {
            isInContact = true;
            contactCount++;
            Debug.Log($"★ [{gameObject.name}] 접촉 {contactCount}회 (노란색: {yellowContactCount}회 필요, 초록색: {greenContactCount}회 필요)");
            UpdateWeldingProgress();
        }
    }

    // 용접 종료 (접촉 종료)
    public void EndWelding()
    {
        if (isInContact)
        {
            isInContact = false;
            Debug.Log($"[{gameObject.name}] 용접 접촉 종료 - 총 접촉 횟수: {contactCount}회");
        }
    }

    // 접촉 횟수 확인 (디버깅용)
    public int GetContactCount()
    {
        return contactCount;
    }
}
