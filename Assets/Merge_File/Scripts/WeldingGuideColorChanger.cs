using UnityEngine;

public class WeldingGuideColorChanger : MonoBehaviour
{
    // 색상 재질들
    public Material greenMaterial;  // 완벽한 용접 (6회 이상)
    public Material yellowMaterial; // 중간 용접 (3~5회)
    private MeshRenderer meshRenderer;

    // 용접 상태를 추적하는 공개 속성
    public bool IsWelded { get; private set; } = false;

    // 용접 접촉 횟수
    private int contactCount = 0;
    private bool isInContact = false;

    // 용접 횟수 설정
    [Header("용접 횟수 설정")]
    [Tooltip("노란색으로 변경되는 최소 접촉 횟수")]
    public int yellowContactCount = 3;

    [Tooltip("초록색으로 변경되는 최소 접촉 횟수")]
    public int greenContactCount = 6;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // 접촉 횟수에 따라 색상 변경 및 상태 설정
    public void UpdateWeldingProgress()
    {
        if (IsWelded) return;

        if (meshRenderer != null)
        {
            // 6회 이상 접촉 - 완벽한 용접 (초록색)
            if (contactCount >= greenContactCount)
            {
                if (greenMaterial != null)
                {
                    meshRenderer.material = greenMaterial;
                }
                IsWelded = true;
                Debug.Log($"완벽한 용접! 접촉 횟수: {contactCount}회");
            }
            // 3~5회 접촉 - 중간 용접 (노란색)
            else if (contactCount >= yellowContactCount)
            {
                if (yellowMaterial != null)
                {
                    meshRenderer.material = yellowMaterial;
                }
                Debug.Log($"중간 용접 진행 중... 접촉 횟수: {contactCount}회");
            }
        }
    }

    // 용접 시작 (접촉 시작)
    public void StartWelding()
    {
        if (!isInContact && !IsWelded)
        {
            isInContact = true;
            contactCount++;
            Debug.Log($"용접 접촉 시작 - 현재 접촉 횟수: {contactCount}회");
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