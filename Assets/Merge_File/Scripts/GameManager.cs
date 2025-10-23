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

    // 품질 점수를 위한 추가 변수
    private int perfectWeldCount = 0;  // 완벽한 용접(초록색) 개수
    private int wrongContactCount = 0;  // 잘못된 접촉 횟수
    private int backboardContactCount = 0;  // 백보드 접촉 횟수

    private float startTime;
    private float weldingTimeLimit = 90.0f; // 기본값: 1분 30초 (프리팹별로 변경 가능)

    // 프리팹별 데이터
    private WeldingPrefabData currentPrefabData;

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

        // 패스스루 활성화 (welding_scene에서 검정 화면 방지)
        EnablePassthrough();

        // 시작 시간은 InitializeWeldingGuides()에서 설정하도록 변경
    }

    void EnablePassthrough()
    {
        // OVRManager 찾기
        OVRManager ovrManager = FindObjectOfType<OVRManager>();
        if (ovrManager != null)
        {
            Debug.Log("[GameManager] OVRManager 발견 - 패스스루 활성화 시도");
        }
        else
        {
            Debug.LogWarning("[GameManager] OVRManager를 찾을 수 없습니다.");
        }

        // OVRPassthroughLayer 찾기 및 활성화
        OVRPassthroughLayer passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = true;
            Debug.Log("[GameManager] OVRPassthroughLayer 활성화 완료");
        }
        else
        {
            Debug.LogWarning("[GameManager] OVRPassthroughLayer를 찾을 수 없습니다. 씬에 패스스루 오브젝트가 없을 수 있습니다.");
        }

        // Camera의 Background를 SolidColor Black으로 설정 (패스스루용)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = Color.black;
            Debug.Log("[GameManager] 메인 카메라 배경을 검정색으로 설정 (패스스루용)");
        }
    }

    // 프리팹 데이터 설정 (프리팹별 시간 제한 등)
    public void SetWeldingPrefabData(WeldingPrefabData prefabData)
    {
        currentPrefabData = prefabData;
        if (prefabData != null)
        {
            weldingTimeLimit = prefabData.timeLimit;
            Debug.Log($"[GameManager] 프리팹 데이터 설정 완료 - 제한시간: {weldingTimeLimit}초");
        }
    }

    // public으로 변경하여 외부에서 호출 가능하게 함 (START 버튼 클릭 시 호출)
    public void InitializeWeldingGuides()
    {
        // 씬에 있는 모든 WeldingGuideColorChanger 오브젝트를 찾아서 개수를 셉니다
        WeldingGuideColorChanger[] guides = FindObjectsOfType<WeldingGuideColorChanger>();
        totalGuideCount = guides.Length;
        weldedGuideCount = 0;
        perfectWeldCount = 0;
        wrongContactCount = 0;
        backboardContactCount = 0;
        isGameFinished = false; // 게임 상태도 초기화

        // ★ 게임 시작 시간 기록 (START 버튼을 누른 시점)
        startTime = Time.time;

        Debug.Log($"[GameManager] ★★★ 게임 시작! 용접 가이드 초기화 완료 ★★★");
        Debug.Log($"[GameManager] 총 큐브 수: {totalGuideCount}개");
        Debug.Log($"[GameManager] 시작 시간: {startTime:F2}초");

        if (totalGuideCount == 0)
        {
            Debug.LogError("[GameManager] ⚠️⚠️⚠️ 경고: 가이드가 0개입니다! WeldingGuideColorChanger 컴포넌트가 큐브에 없을 수 있습니다!");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "welding_scene")
        {
            // 용접 씬이 로드되면 패스스루만 활성화
            // 가이드 초기화는 START 버튼을 눌렀을 때만!
            EnablePassthrough();
            Debug.Log("[GameManager] 웰딩 씬 로드 완료. 프리팹을 선택하고 START 버튼을 누르세요!");
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

    public void OnGuideWelded(WeldingGuideColorChanger guide = null)
    {
        if (isGameFinished)
        {
            Debug.LogWarning("[GameManager] 게임이 이미 종료되어 OnGuideWelded 무시됨");
            return;
        }

        weldedGuideCount++;
        int remainingCount = totalGuideCount - weldedGuideCount;
        Debug.Log($"[GameManager] 용접 완료: {weldedGuideCount} / {totalGuideCount}");
        Debug.Log($"[GameManager] 남은 개수: {remainingCount}개");

        Debug.Log($"[GameManager] 종료 체크: weldedGuideCount={weldedGuideCount}, totalGuideCount={totalGuideCount}");
        if (weldedGuideCount >= totalGuideCount)
        {
            Debug.Log("[GameManager] ★★★ 모든 가이드 완료! 게임 종료 시작 ★★★");
            EndWeldingSession();
        }
        else
        {
            Debug.Log($"[GameManager] 아직 {remainingCount}개 남음");
        }
    }

    public void OnPerfectWeld()
    {
        if (isGameFinished) return;
        perfectWeldCount++;
        Debug.Log($"★ 완벽한 용접 +1 (총 {perfectWeldCount}개)");
    }

    public void OnWrongContact()
    {
        if (isGameFinished) return;
        wrongContactCount++;
        Debug.Log($"잘못된 접촉! (총 {wrongContactCount}회)");
    }

    public void OnBackboardContact()
    {
        if (isGameFinished) return;
        backboardContactCount++;
        Debug.Log($"너무 깊이 들어감! (총 {backboardContactCount}회)");
    }

    private void EndWeldingSession()
    {
        isGameFinished = true;

        // ===== 최종 점수 계산 =====

        // 1. 품질 점수: 완벽한 용접(초록색) 비율
        if (totalGuideCount > 0)
        {
            qualityScore = (int)((float)perfectWeldCount / totalGuideCount * 100);
        }
        else
        {
            qualityScore = 0;
        }

        // 2. 정확도 점수: 잘못된 접촉에 따른 감점 (100점에서 시작)
        accuracyScore = 100 - (wrongContactCount * 2);  // 1회당 2점 감점
        if (accuracyScore < 0) accuracyScore = 0;

        // 3. 깊이 점수: 백보드 접촉에 따른 감점 (100점에서 시작)
        depthScore = 100 - (backboardContactCount * 2);  // 1회당 2점 감점
        if (depthScore < 0) depthScore = 0;

        // 4. 숙련도 점수: 시간에 따른 점수 (제한시간 내 완료하면 100점)
        float elapsedTime = Time.time - startTime;
        if (elapsedTime <= weldingTimeLimit)
        {
            // 제한 시간 내 완료: 100점
            proficiencyScore = 100;
        }
        else
        {
            // 초과된 시간: 5초마다 5점 감점
            float overtimeSeconds = elapsedTime - weldingTimeLimit;
            int penalty = (int)(overtimeSeconds / 5.0f) * 5;
            proficiencyScore = 100 - penalty;
            if (proficiencyScore < 0) proficiencyScore = 0;
        }

        Debug.Log($"===== 용접 완료! =====");
        Debug.Log($"경과 시간: {elapsedTime:F2}초 / 제한시간: {weldingTimeLimit}초");
        Debug.Log($"완벽한 용접: {perfectWeldCount} / {totalGuideCount}개");
        Debug.Log($"잘못된 접촉: {wrongContactCount}회");
        Debug.Log($"백보드 접촉: {backboardContactCount}회");
        Debug.Log($"---");
        Debug.Log($"최종 품질 점수: {qualityScore}점 (완벽한 용접 비율)");
        Debug.Log($"최종 정확도 점수: {accuracyScore}점 (잘못된 접촉 감점)");
        Debug.Log($"최종 깊이 점수: {depthScore}점 (백보드 접촉 감점)");
        Debug.Log($"최종 숙련도 점수: {proficiencyScore}점 (시간)");

        SceneManager.LoadScene("result_scene");
    }

}