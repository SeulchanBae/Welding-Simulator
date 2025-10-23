using UnityEngine;

public class WeldingGuideColorChanger : MonoBehaviour
{
    // 색상 재질들
    public Material greenMaterial;  // 완벽한 용접
    public Material yellowMaterial; // 불완전한 용접
    private MeshRenderer meshRenderer;

    // 용접 상태를 추적하는 공개 속성
    public bool IsWelded { get; private set; } = false;

    // 용접 시간 측정
    private float weldingStartTime = 0f;
    private bool isWelding = false;

    // 적절한 용접 시간 범위 (초 단위)
    [Header("용접 시간 설정")]
    [Tooltip("최소 용접 시간 (초)")]
    public float minWeldingTime = 0.3f;

    [Tooltip("최대 용접 시간 (초)")]
    public float maxWeldingTime = 1.0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 외부에서 호출될 색상 변경 및 상태 설정 메서드
    public void WeldGuide(float weldingDuration)
    {
        if (IsWelded) return;

        if (meshRenderer != null)
        {
            // 적절한 시간 동안 용접했는지 확인
            if (weldingDuration >= minWeldingTime && weldingDuration <= maxWeldingTime)
            {
                // 완벽한 용접 - 초록색
                if (greenMaterial != null)
                {
                    meshRenderer.material = greenMaterial;
                }
                IsWelded = true;
                Debug.Log($"완벽한 용접! 시간: {weldingDuration:F2}초");
            }
            else
            {
                // 불완전한 용접 - 노란색
                if (yellowMaterial != null)
                {
                    meshRenderer.material = yellowMaterial;
                }
                IsWelded = true;

                // 품질 점수 감점
                if (GameManager.Instance != null)
                {
                    if (weldingDuration < minWeldingTime)
                    {
                        GameManager.Instance.OnPoorQuality();
                        Debug.Log($"너무 빠른 용접! 시간: {weldingDuration:F2}초 (최소: {minWeldingTime}초)");
                    }
                    else if (weldingDuration > maxWeldingTime)
                    {
                        GameManager.Instance.OnPoorQuality();
                        Debug.Log($"너무 느린 용접! 시간: {weldingDuration:F2}초 (최대: {maxWeldingTime}초)");
                    }
                }
            }
        }
    }

    // 용접 시작
    public void StartWelding()
    {
        if (!isWelding && !IsWelded)
        {
            isWelding = true;
            weldingStartTime = Time.time;
        }
    }

    // 용접 종료 및 평가
    public float EndWelding()
    {
        if (isWelding)
        {
            isWelding = false;
            float duration = Time.time - weldingStartTime;
            return duration;
        }
        return 0f;
    }
}