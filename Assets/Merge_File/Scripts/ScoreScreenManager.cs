using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoreScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject scoreScreenCanvas;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI proficiencyText;
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private TextMeshProUGUI qualityText;
    [SerializeField] private TextMeshProUGUI elapsedTimeText;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    [SerializeField]
    private Transform vrUserCamera;

    [SerializeField]
    private Vector3 canvasLocalOffset = new Vector3(0, 0, 3.0f);

    void Start()
    {
        SetupButtonClickHandlers(retryButton, OnRetryButtonClick);
        SetupButtonClickHandlers(exitButton, OnExitButtonClick);

        // HideScoreScreen(); // 시작 시 숨기기 (원래 코드에서는 주석 처리됨)
    }

    // SetScores 함수 (int형 파라미터를 받던 버전)
    public void SetScores(int accuracy, int proficiency, int depth, int quality, float elapsedTime,
                          int wrongContactCount, int backboardContactCount, int perfectWeldCount, int totalGuideCount, float timeLimit)
    {
        if (accuracyText != null)
        {
            accuracyText.text = $"정확도 : {accuracy}점\n(잘못된 접촉 {wrongContactCount}회)";
        }
        if (proficiencyText != null)
        {
            proficiencyText.text = $"숙련도 : {proficiency}점\n(제한시간 {timeLimit}초)";
        }
        if (depthText != null)
        {
            depthText.text = $"깊이 : {depth}점\n(백보드 접촉 {backboardContactCount}회)";
        }
        if (qualityText != null)
        {
            qualityText.text = $"품질 : {quality}점\n(완벽한 용접 {perfectWeldCount}/{totalGuideCount}개)";
        }
        if (elapsedTimeText != null)
        {
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            elapsedTimeText.text = $"소요 시간 : {minutes:00}:{seconds:00}";
        }
    }

    public void ShowScoreScreen()
    {
        if (scoreScreenCanvas != null)
        {
            scoreScreenCanvas.SetActive(true);

            if (vrUserCamera != null)
            {
                scoreScreenCanvas.transform.SetParent(vrUserCamera.transform, false);
                scoreScreenCanvas.transform.localPosition = canvasLocalOffset;
                scoreScreenCanvas.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("VR User Camera가 할당되지 않았습니다. 화면이 초기 위치에 배치됩니다.");
            }
        }
    }

    public void HideScoreScreen()
    {
        if (scoreScreenCanvas != null)
        {
            scoreScreenCanvas.SetActive(false);
        }
    }

    private void SetupButtonClickHandlers(Button button, System.Action clickAction)
    {
        if (button == null)
        {
            Debug.LogWarning("SetupButtonClickHandlers: Button is null.");
            return;
        }
        button.onClick.AddListener(() =>
        {
            Debug.Log($"{button.name} - Button.onClick 기본 이벤트 호출");
            clickAction?.Invoke();
        });

        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            Debug.Log($"{button.name} - EventTrigger PointerClick 이벤트 발생, Button.onClick.Invoke() 호출 시도");
            button.onClick.Invoke();
        });

        bool found = false;
        foreach (var existingEntry in trigger.triggers)
        {
            if (existingEntry.eventID == EventTriggerType.PointerClick)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogWarning($"{button.name}에 이미 PointerClick EventTrigger가 존재합니다. 추가하지 않습니다.");
        }
    }

    public void OnRetryButtonClick()
    {
        Debug.Log("다시하기 버튼 클릭!");
        SceneManager.LoadScene("welding_scene");
    }

    public void OnExitButtonClick()
    {
        Debug.Log("프로그램 종료 버튼 클릭!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}