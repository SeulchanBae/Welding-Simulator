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
        
        //HideScoreScreen();

        ShowScoreScreen();
    }
    
    public void SetScores(int accuracy, int proficiency)
    {
        if (accuracyText != null)
        {
            accuracyText.text = $"accuracy : {accuracy}";
        }
        if (proficiencyText != null)
        {
            proficiencyText.text = $"proficiency : {proficiency}";
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
        SceneManager.LoadScene("scn_Welding");
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