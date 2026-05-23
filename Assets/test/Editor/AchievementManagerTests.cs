using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AchievementManagerTests
{
    private GameObject managerObject;
    private AchievementManager manager;

    [SetUp]
    public void SetUp()
    {
        AchievementManager.instance = null;
        managerObject = new GameObject("Achievement Manager Test");
        manager = managerObject.AddComponent<AchievementManager>();
        manager.ConfigureForTests(new[]
        {
            CreateAchievement("first_blood", 1),
            CreateAchievement("monster_hunter", 10)
        });
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(managerObject);
        AchievementManager.instance = null;
    }

    [Test]
    public void RecordProgress_WhenProgressReachesTarget_UnlocksAchievement()
    {
        manager.RecordProgress("first_blood");

        Assert.AreEqual(1, manager.GetProgress("first_blood"));
        Assert.IsTrue(manager.IsUnlocked("first_blood"));
    }

    [Test]
    public void RecordProgress_WhenAchievementAlreadyUnlocked_DoesNotUnlockAgain()
    {
        int unlockCount = 0;
        manager.onAchievementUnlocked += _ => unlockCount++;

        manager.RecordProgress("first_blood");
        manager.RecordProgress("first_blood");

        Assert.AreEqual(1, unlockCount);
        Assert.AreEqual(1, manager.GetProgress("first_blood"));
    }

    [Test]
    public void LoadData_RestoresUnlockedStateAndProgress()
    {
        GameData data = new GameData();
        data.achievements.Add("first_blood", true);
        data.achievementProgress.Add("monster_hunter", 6);

        manager.LoadData(data);

        Assert.IsTrue(manager.IsUnlocked("first_blood"));
        Assert.AreEqual(6, manager.GetProgress("monster_hunter"));
    }

    [Test]
    public void SaveData_WhenAchievementDictionariesAreMissing_CreatesThem()
    {
        GameData data = new GameData();
        data.achievements = null;
        data.achievementProgress = null;

        manager.RecordProgress("first_blood");
        manager.SaveData(ref data);

        Assert.NotNull(data.achievements);
        Assert.NotNull(data.achievementProgress);
        Assert.IsTrue(data.achievements["first_blood"]);
        Assert.AreEqual(1, data.achievementProgress["first_blood"]);
    }

    [Test]
    public void RecordProgress_WithUnknownAchievement_DoesNotThrow()
    {
        LogAssert.Expect(LogType.Warning, "Achievement not found: missing");

        Assert.DoesNotThrow(() => manager.RecordProgress("missing"));
    }

    private static AchievementData CreateAchievement(string id, int targetProgress)
    {
        AchievementData achievement = ScriptableObject.CreateInstance<AchievementData>();
        achievement.ConfigureForTests(id, id, id, targetProgress);
        return achievement;
    }
}
