using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsTabController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject audioContent;
    [SerializeField] private GameObject controlContent;
    [SerializeField] private GameObject saveContent;

    public void ShowAudio()
    {
        titleText.text = "오디오";
        audioContent.SetActive(true);
        controlContent.SetActive(false);
        saveContent.SetActive(false);
    }

    public void ShowControls()
    {
        titleText.text = "조작환경";
        audioContent.SetActive(false);
        controlContent.SetActive(true);
        saveContent.SetActive(false);
    }

    public void ShowSave()
    {
        titleText.text = "데이터 저장";
        audioContent.SetActive(false);
        controlContent.SetActive(false);
        saveContent.SetActive(true);
    }
}
