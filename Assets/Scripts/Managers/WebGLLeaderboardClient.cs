using System.Runtime.InteropServices;
using UnityEngine;

public static class WebGLLeaderboardClient
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ShowcaseSubmitScore(string payload);

    [DllImport("__Internal")]
    private static extern void ShowcaseSaveGame(string payload);

    [DllImport("__Internal")]
    private static extern void ShowcaseUpdateGameState(string payload);

    [DllImport("__Internal")]
    private static extern void ShowcaseUnityReady();

    [DllImport("__Internal")]
    private static extern void ShowcaseUnityError(string message);
#endif

    public static void NotifyReady()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowcaseUnityReady();
#else
        Debug.Log("Showcase Unity bridge ready.");
#endif
    }

    public static void NotifyError(string message)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowcaseUnityError(message);
#else
        Debug.LogWarning(message);
#endif
    }

    public static void SubmitScore(GameScorePayload payload)
    {
        string json = JsonUtility.ToJson(payload);
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowcaseSubmitScore(json);
#else
        Debug.Log("Showcase score payload: " + json);
#endif
    }

    public static void SaveGame(GameSavePayload payload)
    {
        string json = JsonUtility.ToJson(payload);
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowcaseSaveGame(json);
#else
        Debug.Log("Showcase save payload: " + json);
#endif
    }

    public static void UpdateGameState(UnityGameStatePayload payload)
    {
        string json = JsonUtility.ToJson(payload);
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowcaseUpdateGameState(json);
#else
        Debug.Log("Showcase game state payload: " + json);
#endif
    }
}

[System.Serializable]
public class GameScorePayload
{
    public string clientRunId;
    public int levelId;
    public int score;
    public int stars;
    public float completionTime;
    public bool isCompleted;
}

[System.Serializable]
public class GameSavePayload
{
    public string saveData;
}

[System.Serializable]
public class UnityGameStatePayload
{
    public int score;
    public int souls;
    public float elapsedSeconds;
}
