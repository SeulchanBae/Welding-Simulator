using UnityEngine;

public class WeldingGuideColorChanger : MonoBehaviour
{
    // 초록색 재질
    public Material greenMaterial;
    private MeshRenderer meshRenderer;

    // 용접 상태를 추적하는 공개 속성
    public bool IsWelded { get; private set; } = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 외부에서 호출될 색상 변경 및 상태 설정 메서드
    public void WeldGuide()
    {
        if (IsWelded) return;

        if (meshRenderer != null && greenMaterial != null)
        {
            meshRenderer.material = greenMaterial;
            IsWelded = true;
        }
    }

    // OnTriggerEnter는 이제 더 이상 색상을 직접 변경하지 않습니다.
    // 역할 분리를 위해 이 함수는 비워둡니다.
    // 충돌 감지는 WeldingSparkController가 처리합니다.
}