using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventSystem 관련 클래스 사용을 위해 추가

public class ScoreScreenManager2 : MonoBehaviour
{
    [SerializeField] private GameObject scoreScreenCanvas;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    // --- 새로 변경된 상세 점수 관련 UI 필드 (4개 항목) ---
    [Header("Detailed Scores UI")]
    [SerializeField] private GameObject detailScoresPanel; // 상세 점수 텍스트들을 묶어둘 GameObject
    [SerializeField] private TextMeshProUGUI equipmentScoreText;    // 1. Equipment Setup & Simulator Understanding
    [SerializeField] private TextMeshProUGUI postureScoreText;      // 2. Welding Posture & Virtual Control Proficiency
    [SerializeField] private TextMeshProUGUI beadQualityScoreText;  // 3. Weld Quality (Virtual Bead)
    [SerializeField] private TextMeshProUGUI safetyScoreText;       // 4. In-VR Safety Protocol Adherence
    [SerializeField] private Button detailButton;                   // 상세 점수 보기 버튼
    [SerializeField] private Button backButton;                     // 총합 점수로 돌아갈 백 버튼 (새로 추가)

    [SerializeField]
    private Transform vrUserCamera; // VR 사용자의 카메라(머리) Transform 참조
    [SerializeField]
    private float distanceFromUser = 3.0f; // 사용하지 않으므로 필요에 따라 제거 가능
    [SerializeField]
    private float verticalOffset = 2.0f; // 사용하지 않으므로 필요에 따라 제거 가능

    [SerializeField]
    private Vector3 canvasLocalOffset = new Vector3(0, 0, 3.0f); // 카메라 기준 로컬 X, Y, Z 오프셋

    // 각 항목별 최대 배점 (변경된 심사표 기준 4개 항목)
    private readonly int[] maxScores = { 20, 30, 40, 10 }; // 1. 장비 세팅(20), 2. 용접 자세(30), 3. 용접부 품질(40), 4. 안전(10)
    // 각 항목별 이름 (디스플레이용 - 영어로 변경됨)
    private readonly string[] scoreNames = {
        "1. Equipment Setup & Simulator Understanding",
        "2. Welding Posture & Virtual Control Proficiency",
        "3. Weld Quality",
        "4. In-VR Safety Protocol Adherence"
    };

    private int currentTotalScore;
    private int[] currentSubScores = new int[4]; // 항목이 4개로 변경됨


    void Start()
    {
        // 버튼 클릭 핸들러 설정
        SetupButtonClickHandlers(retryButton, OnRetryButtonClick);
        SetupButtonClickHandlers(exitButton, OnExitButtonClick);
        SetupButtonClickHandlers(detailButton, OnDetailButtonClick);
        SetupButtonClickHandlers(backButton, OnBackButtonClick); // 백 버튼 핸들러 추가

        ShowScoreScreen();

        if (detailScoresPanel != null)
        {
            detailScoresPanel.SetActive(false); // 시작 시 상세 점수 패널은 숨김
        }
    }

    void Update()
    {
        if (vrUserCamera != null && scoreScreenCanvas != null && scoreScreenCanvas.activeSelf)
        {
            scoreScreenCanvas.transform.parent = vrUserCamera.transform;
            scoreScreenCanvas.transform.localPosition = canvasLocalOffset;
            scoreScreenCanvas.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void ShowScoreScreen()
    {
        if (scoreScreenCanvas != null)
        {
            scoreScreenCanvas.SetActive(true);
        }

        CalculateAndDisplayScores();
    }

    public void HideScoreScreen()
    {
        if (scoreScreenCanvas != null)
        {
            scoreScreenCanvas.SetActive(false);
        }
    }

    private void CalculateAndDisplayScores()
    {
        currentTotalScore = Random.Range(5, 101); // 종합 점수 5 ~ 100점 랜덤 생성
        DistributeScores(currentTotalScore, maxScores, out currentSubScores);

        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {currentTotalScore}";
        }
        UpdateSubScoreUI();
    }

    private void DistributeScores(int total, int[] maxs, out int[] results)
    {
        results = new int[maxs.Length];
        int remainingTotal = total;
        int minPossibleSum = 0;
        // 각 항목에 최소 1점씩 배분하여 합계에서 차감
        for (int i = 0; i < maxs.Length; i++)
        {
            results[i] = 1;
            remainingTotal -= 1;
            minPossibleSum += 1;
        }

        int maxPossibleSum = 0;
        for (int i = 0; i < maxs.Length; i++)
        {
            maxPossibleSum += maxs[i];
        }

        // 최소/최대 점수 제한 검사 (여기서부터 원래 코드와 동일)
        if (total < minPossibleSum)
        {
            Debug.LogWarning($"Total score ({total}) is less than the minimum possible sum of sub-scores ({minPossibleSum}). All sub-scores will be set to 1.");
            for (int i = 0; i < maxs.Length; i++) results[i] = 1;
            return;
        }
        if (total > maxPossibleSum)
        {
            Debug.LogWarning($"Total score ({total}) is greater than the maximum possible sum of sub-scores ({maxPossibleSum}). All sub-scores will be set to their max scores.");
            for (int i = 0; i < maxs.Length; i++) results[i] = maxs[i];
            return;
        }

        while (remainingTotal > 0)
        {
            List<int> eligibleIndices = new List<int>();
            for (int i = 0; i < maxs.Length; i++)
            {
                if (results[i] < maxs[i]) // 아직 최대 점수에 도달하지 않은 항목만 선택
                {
                    eligibleIndices.Add(i);
                }
            }

            if (eligibleIndices.Count == 0) // 더 이상 점수를 배분할 항목이 없음 (모든 항목이 최대 점수에 도달)
            {
                break;
            }

            int randomIndex = Random.Range(0, eligibleIndices.Count);
            int chosenIndex = eligibleIndices[randomIndex];

            results[chosenIndex]++;
            remainingTotal--;
        }
    }

    private void UpdateSubScoreUI()
    {
        // 4개 항목에 맞춰 UI 업데이트 (영어로 변경)
        if (equipmentScoreText != null) equipmentScoreText.text = $"{scoreNames[0]}: {currentSubScores[0]} / 20";
        if (postureScoreText != null) postureScoreText.text = $"{scoreNames[1]}: {currentSubScores[1]} / 30";
        if (beadQualityScoreText != null) beadQualityScoreText.text = $"{scoreNames[2]}: {currentSubScores[2]} / 40";
        if (safetyScoreText != null) safetyScoreText.text = $"{scoreNames[3]}: {currentSubScores[3]} / 10";
    }

    public void OnRetryButtonClick()
    {
        Debug.Log("Retry button clicked!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButtonClick()
    {
        Debug.Log("Exit program button clicked!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // 디테일 버튼 클릭 시: 상세 점수 패널을 보여줍니다. (토글 아님)
    public void OnDetailButtonClick()
    {
        if (detailScoresPanel == null)
        {
            Debug.LogWarning("detailScoresPanel is not assigned in the Inspector.");
            return;
        }

        // 상세 점수 패널을 활성화합니다.
        detailScoresPanel.SetActive(true);
        Debug.Log("Detail button clicked: Displaying detailed scores.");

    }

    // 백 버튼 클릭 시: 상세 점수 패널을 숨깁니다.
    public void OnBackButtonClick()
    {
        if (detailScoresPanel == null)
        {
            Debug.LogWarning("detailScoresPanel is not assigned in the Inspector.");
            return;
        }

        // 상세 점수 패널을 비활성화합니다.
        detailScoresPanel.SetActive(false);
        Debug.Log("Back button clicked: Hiding detailed scores.");

    }

    private void SetupButtonClickHandlers(Button button, System.Action clickAction)
    {
        if (button == null)
        {
            Debug.LogWarning($"SetupButtonClickHandlers: Button is null for click action.");
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log($"{button.name} - Button.onClick default listener called");
            clickAction?.Invoke();
        });

        // EventTrigger는 VR 환경에서 PointerClick을 감지하는 데 유용합니다.
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry existingEntry = null;
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerClick)
            {
                existingEntry = entry;
                break;
            }
        }
        if (existingEntry == null)
        {
            existingEntry = new EventTrigger.Entry();
            existingEntry.eventID = EventTriggerType.PointerClick;
            trigger.triggers.Add(existingEntry);
        }

        existingEntry.callback.RemoveAllListeners();
        existingEntry.callback.AddListener((data) =>
        {
            Debug.Log($"{button.name} - EventTrigger PointerClick event occurred, calling Button.onClick.Invoke()");
            button.onClick.Invoke();
        });
    }
}