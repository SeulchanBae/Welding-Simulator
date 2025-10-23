using UnityEngine;

public class WeldingPrefabData : MonoBehaviour
{
    [Header("프리팹 난이도 설정")]
    [Tooltip("이 프리팹의 이름")]
    public string prefabName = "Unknown";

    [Tooltip("용접 제한 시간 (초)")]
    public float timeLimit = 20.0f;

    [Tooltip("난이도 (1=쉬움, 2=보통, 3=어려움)")]
    public int difficulty = 1;

    void Start()
    {
        // 프리팹 소환 시 GameManager에 설정 전달
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetWeldingPrefabData(this);
            Debug.Log($"[WeldingPrefabData] '{prefabName}' 프리팹 데이터 전달 - 제한시간: {timeLimit}초, 난이도: {difficulty}");
        }
    }
}
