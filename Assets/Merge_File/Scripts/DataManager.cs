using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public Button resetButton;
    public AudioManager audioManager; // AudioManager 참조 추가

    void Start()
    {
        resetButton.onClick.AddListener(ResetData);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("모든 데이터가 초기화되었습니다");

        // 즉시 슬라이더 값 0으로 변경
        if (audioManager != null)
        {
            audioManager.masterSlider.value = 0f;
            audioManager.bgmSlider.value = 0f;
            audioManager.sfxSlider.value = 0f;
        }
    }
}