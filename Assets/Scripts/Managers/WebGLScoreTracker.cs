using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class WebGLScoreTracker : MonoBehaviour
{
    public static WebGLScoreTracker instance;

    [SerializeField] private int starsPerRun = 0;

    private float startedAt;
    private string clientRunId;
    private int score;
    private bool submitting;
    private bool submitted;

    public int CurrentScore => PlayerManager.instance != null ? PlayerManager.instance.currency : score;
    public bool HasSubmitted => submitted;
    public float CurrentElapsedSeconds => Mathf.Max(0, Time.realtimeSinceStartup - startedAt);

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        ResetRun();
    }

    public static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        GameObject tracker = new GameObject("WebGL Score Tracker");
        tracker.AddComponent<WebGLScoreTracker>();
    }

    public void ResetRun()
    {
        startedAt = Time.realtimeSinceStartup;
        clientRunId = Guid.NewGuid().ToString("N");
        score = 0;
        submitting = false;
        submitted = false;
        NotifyState();
    }

    public void AddScore(int amount)
    {
        score += Mathf.Max(0, amount);
        NotifyState();
    }

    public void SetScore(int amount)
    {
        score = Mathf.Max(0, amount);
        NotifyState();
    }

    public void NotifyState()
    {
        WebGLLeaderboardClient.UpdateGameState(new UnityGameStatePayload
        {
            score = CurrentScore,
            souls = CurrentScore,
            elapsedSeconds = CurrentElapsedSeconds
        });
    }

    public void SubmitRun(bool isCompleted)
    {
        TrySubmitRun(isCompleted ? "completed" : "death", isCompleted);
    }

    public void SubmitCurrentRunFromPageUnload()
    {
        TrySubmitRun("page-unload", false);
    }

    private void TrySubmitRun(string reason, bool isCompleted)
    {
        NotifyState();

        if (submitted)
        {
            return;
        }

        if (submitting)
        {
            return;
        }

        if (!isCompleted && CurrentScore <= 0)
        {
            return;
        }

        submitting = true;
        submitted = true;
        WebGLLeaderboardClient.SubmitScore(new GameScorePayload
        {
            clientRunId = clientRunId,
            levelId = SceneManager.GetActiveScene().buildIndex + 1,
            score = CurrentScore,
            stars = starsPerRun,
            completionTime = CurrentElapsedSeconds,
            isCompleted = isCompleted
        });
        submitting = false;
        NotifyState();
    }

    private void OnApplicationQuit()
    {
        SubmitRun(false);
    }
}
