using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject GuidePanel;
    public GameObject SettingsPanel;

    public void OpenGuide()
    {
        MainMenuPanel.SetActive(false);
        GuidePanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        MainMenuPanel.SetActive(false);
        GuidePanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void BackToMain()
    {
        MainMenuPanel.SetActive(true);
        GuidePanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }
}
