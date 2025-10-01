using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelSettings;
    public GameObject panelGuide;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelSettings.SetActive(false);
        panelGuide.SetActive(false);
    }

    public void ShowSettings()
    {
        panelMainMenu.SetActive(false);
        panelSettings.SetActive(true);
        panelGuide.SetActive(false);
    }

    public void ShowGuide()
    {
        panelMainMenu.SetActive(false);
        panelSettings.SetActive(false);
        panelGuide.SetActive(true);
    }

    public void OnStartClicked()
    {
        Debug.Log("Start 버튼 클릭됨 - 용접 씬 로드 예정");
        // 나중에 용접 씬 연결 시: SceneManager.LoadScene("WeldingScene");
    }
}