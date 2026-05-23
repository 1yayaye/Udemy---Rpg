using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AchievementManager : MonoBehaviour, ISaveManager
{
    public static AchievementManager instance;

    [SerializeField] private List<AchievementData> achievements = new List<AchievementData>();

    private readonly Dictionary<string, AchievementData> achievementById = new Dictionary<string, AchievementData>();
    private readonly Dictionary<string, bool> unlockedById = new Dictionary<string, bool>();
    private readonly Dictionary<string, int> progressById = new Dictionary<string, int>();

    public System.Action<AchievementData> onAchievementUnlocked;

    public IReadOnlyList<AchievementData> Achievements => achievements;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildLookup();
    }

    public static void EnsureInstance()
    {
        if (instance != null)
            return;

        GameObject managerObject = new GameObject("Achievement Manager");
        managerObject.AddComponent<AchievementManager>();
    }

    public void RecordProgress(string achievementId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(achievementId))
            return;

        BuildLookup();

        if (!achievementById.TryGetValue(achievementId, out AchievementData achievement))
        {
            Debug.LogWarning("Achievement not found: " + achievementId);
            return;
        }

        if (IsUnlocked(achievementId))
            return;

        int currentProgress = GetProgress(achievementId);
        int nextProgress = Mathf.Clamp(currentProgress + Mathf.Max(0, amount), 0, achievement.TargetProgress);
        progressById[achievementId] = nextProgress;

        if (nextProgress >= achievement.TargetProgress)
            Unlock(achievement);
    }

    public void RecordEnemyKilled(Enemy enemy, int souls)
    {
        RecordProgress(AchievementIds.FirstBlood);
        RecordProgress(AchievementIds.MonsterHunter);
        RecordProgress(AchievementIds.SoulCollector, souls);

        if (enemy is Enemy_DeathBringer)
            RecordProgress(AchievementIds.DeathBringerDown);
    }

    public void RecordCheckpointActivated() => RecordProgress(AchievementIds.SafePlace);

    public void RecordSkillUnlocked() => RecordProgress(AchievementIds.SkillAwakened);

    public void RecordEquipmentEquipped() => RecordProgress(AchievementIds.WellEquipped);

    public void RecordItemCrafted() => RecordProgress(AchievementIds.FirstCraft);

    public void RecordFlaskUsed() => RecordProgress(AchievementIds.NotToday);

    public void RecordPlayerDeath() => RecordProgress(AchievementIds.LessonLearned);

    public bool IsUnlocked(string achievementId)
    {
        return unlockedById.TryGetValue(achievementId, out bool unlocked) && unlocked;
    }

    public int GetProgress(string achievementId)
    {
        return progressById.TryGetValue(achievementId, out int progress) ? progress : 0;
    }

    public AchievementData GetAchievement(string achievementId)
    {
        BuildLookup();
        achievementById.TryGetValue(achievementId, out AchievementData achievement);
        return achievement;
    }

    public void LoadData(GameData data)
    {
        BuildLookup();
        unlockedById.Clear();
        progressById.Clear();

        if (data.achievements != null)
        {
            foreach (KeyValuePair<string, bool> pair in data.achievements)
            {
                unlockedById[pair.Key] = pair.Value;
            }
        }

        if (data.achievementProgress != null)
        {
            foreach (KeyValuePair<string, int> pair in data.achievementProgress)
            {
                progressById[pair.Key] = Mathf.Max(0, pair.Value);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.achievements == null)
            data.achievements = new SerializableDictionary<string, bool>();

        if (data.achievementProgress == null)
            data.achievementProgress = new SerializableDictionary<string, int>();

        data.achievements.Clear();
        data.achievementProgress.Clear();

        foreach (AchievementData achievement in achievements)
        {
            if (achievement == null || string.IsNullOrWhiteSpace(achievement.achievementId))
                continue;

            string achievementId = achievement.achievementId;
            data.achievements[achievementId] = IsUnlocked(achievementId);
            data.achievementProgress[achievementId] = GetProgress(achievementId);
        }
    }

    private void Unlock(AchievementData achievement)
    {
        string achievementId = achievement.achievementId;
        unlockedById[achievementId] = true;
        progressById[achievementId] = achievement.TargetProgress;
        onAchievementUnlocked?.Invoke(achievement);
    }

    private void BuildLookup()
    {
        if (achievements == null)
            achievements = new List<AchievementData>();

        if (achievements.Count == 0)
            achievements.AddRange(AchievementDefaults.CreateDefaultAchievements());

        achievementById.Clear();

        foreach (AchievementData achievement in achievements)
        {
            if (achievement == null || string.IsNullOrWhiteSpace(achievement.achievementId))
                continue;

            achievementById[achievement.achievementId] = achievement;
        }
    }

#if UNITY_EDITOR
    public void ConfigureForTests(IEnumerable<AchievementData> testAchievements)
    {
        achievements = new List<AchievementData>(testAchievements);
        unlockedById.Clear();
        progressById.Clear();
        BuildLookup();
    }

    [ContextMenu("Fill up achievement data base")]
    private void FillUpAchievementDataBase()
    {
        achievements = new List<AchievementData>();
        string[] assetNames = AssetDatabase.FindAssets("t:AchievementData", new[] { "Assets/Data/Achievements" });

        foreach (string assetName in assetNames)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetName);
            AchievementData achievement = AssetDatabase.LoadAssetAtPath<AchievementData>(assetPath);

            if (achievement != null)
                achievements.Add(achievement);
        }

        BuildLookup();
    }
#endif
}
