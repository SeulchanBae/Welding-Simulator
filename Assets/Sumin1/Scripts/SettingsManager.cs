using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject panelAudioContent;
    public GameObject panelControlsContent;
    public GameObject panelDataContent;

    void Start()
    {
        ShowAudio(); // 기본으로 오디오 표시
    }

    public void ShowAudio()
    {
        panelAudioContent.SetActive(true);
        panelControlsContent.SetActive(false);
        panelDataContent.SetActive(false);
    }

    public void ShowControls()
    {
        panelAudioContent.SetActive(false);
        panelControlsContent.SetActive(true);
        panelDataContent.SetActive(false);
    }

    public void ShowData()
    {
        panelAudioContent.SetActive(false);
        panelControlsContent.SetActive(false);
        panelDataContent.SetActive(true);
    }
}