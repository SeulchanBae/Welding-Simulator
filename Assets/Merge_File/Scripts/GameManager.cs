using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine 사용을 위해 추가!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int accuracyScore = 100;
    private int proficiencyScore = 100;

    // 깊이, 품질 점수 추가
    private int depthScore = 100;
    private int qualityScore = 100;

    private int weldedGuideCount = 0;
    private int totalGuideCount = 0;
    private bool isGameFinished = false;

    private float startTime;
    private float weldingTimeLimit = 20.0f;

    // ScoreScreenManager 참조는 코루틴 내부에서 찾으므로 여기서 미리 찾을 필요 없음
    // private ScoreScreenManager scoreScreenManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        startTime = Time.time;
        InitializeWeldingGuides();
    }

    // public으로 변경하여 외부에서 호출 가능하게 함 (프리팹 소환 후 호출용)
    public void InitializeWeldingGuides()
    {
        // 씬에 있는 모든 WeldingGuideColorChanger 오브젝트를 찾아서 개수를 셉니다
        WeldingGuideColorChanger[] guides = FindObjectsOfType<WeldingGuideColorChanger>();
        totalGuideCount = guides.Length;
        weldedGuideCount = 0;
        isGameFinished = false; // 게임 상태도 초기화

        Debug.Log($"용접 가이드 초기화 완료: 총 {totalGuideCount}개의 가이드 발견");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "welding_scene")
        {
            // 용접 씬이 로드되면 가이드를 다시 초기화합니다
            startTime = Time.time;
            InitializeWeldingGuides();
        }
        else if (scene.name == "result_scene")
        {
            // 직접 함수를 호출하는 대신 코루틴을 시작합니다.
            StartCoroutine(SetupResultScene());
        }
    }

    // 결과 씬을 설정하는 코루틴
    IEnumerator SetupResultScene()
    {
        // 씬에 있는 ScoreScreenManager를 찾습니다.
        ScoreScreenManager scoreScreenManager = FindObjectOfType<ScoreScreenManager>();

        //  핵심: 다음 프레임까지 딱 한 번 기다립니다. 
        // 이 시간 동안 씬의 다른 오브젝트(카메라 등)가 초기화를 마칩니다.
        yield return null;

        if (scoreScreenManager != null)
        {
            Debug.Log("ScoreScreenManager를 찾았으므로 점수판을 표시합니다.");
            // 점수 설정과 화면 표시는 반드시 기다린 후에 호출합니다.
            scoreScreenManager.SetScores(accuracyScore, proficiencyScore, depthScore, qualityScore);
            scoreScreenManager.ShowScoreScreen();
        }
        else
        {
            Debug.LogError("result_scene 씬에서 ScoreScreenManager를 찾을 수 없습니다!");
        }
    }

    public void OnGuideWelded()
    {
        if (isGameFinished) return;

        weldedGuideCount++;
        Debug.Log($"용접 완료: {weldedGuideCount} / {totalGuideCount}");

        if (weldedGuideCount >= totalGuideCount)
        {
            EndWeldingSession();
        }
    }

    public void OnWrongContact()
    {
        if (isGameFinished) return;
        accuracyScore -= 5;
        Debug.Log($"정확도 점수 감점! 현재 점수: {accuracyScore}");
        if (accuracyScore < 0)
        {
            accuracyScore = 0;
        }
    }

    public void OnBackboardContact()
    {
        if (isGameFinished) return;
        depthScore -= 5;
        Debug.Log($"깊이 점수 감점! 현재 점수: {depthScore}");
        if (depthScore < 0)
        {
            depthScore = 0;
        }
    }

    public void OnPoorQuality()
    {
        if (isGameFinished) return;
        qualityScore -= 5;
        Debug.Log($"품질 점수 감점! 현재 점수: {qualityScore}");
        if (qualityScore < 0)
        {
            qualityScore = 0;
        }
    }

    private void EndWeldingSession()
    {
        isGameFinished = true;

        float elapsedTime = Time.time - startTime;
        if (elapsedTime > weldingTimeLimit)
        {
            int penalty = (int)((elapsedTime - weldingTimeLimit) / 1.0f) * 5;
            proficiencyScore -= penalty;
        }
        if (proficiencyScore < 0)
        {
            proficiencyScore = 0;
        }

        Debug.Log($"용접 완료! 경과 시간: {elapsedTime:F2}초");
        Debug.Log($"최종 정확도 점수: {accuracyScore}");
        Debug.Log($"최종 숙련도 점수: {proficiencyScore}");
        Debug.Log($"최종 깊이 점수: {depthScore}");
        Debug.Log($"최종 품질 점수: {qualityScore}");

        SceneManager.LoadScene("result_scene");
    }

}