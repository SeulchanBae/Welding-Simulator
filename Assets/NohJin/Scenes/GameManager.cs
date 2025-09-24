using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int accuracyScore = 100;
    private int proficiencyScore = 100;
    
    private int sparkCount = 0;
    private int totalSparkCount = 66;
    private bool isGameFinished = false;

    private float startTime;
    private float weldingTimeLimit = 20.0f;

    private ScoreScreenManager scoreScreenManager;

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
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "scn_result") 
        {
            scoreScreenManager = FindObjectOfType<ScoreScreenManager>();
            if (scoreScreenManager != null)
            {
                ShowScoreScreen();
            }
        }
    }

    public void OnSparkFired()
    {
        if (isGameFinished) return;
        
        sparkCount++;
        Debug.Log($"스파크 발생: {sparkCount} / {totalSparkCount}");

        if (sparkCount >= totalSparkCount)
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

        SceneManager.LoadScene("scn_result");
    }

    private void ShowScoreScreen()
    {
        if (scoreScreenManager != null)
        {
            scoreScreenManager.SetScores(accuracyScore, proficiencyScore);
        }
    }
}