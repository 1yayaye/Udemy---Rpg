using System.Collections.Generic;
using UnityEngine;

public static class AchievementDefaults
{
    public static List<AchievementData> CreateDefaultAchievements()
    {
        return new List<AchievementData>
        {
            Create(AchievementIds.FirstBlood, "First Blood", "Defeat your first enemy.", 1),
            Create(AchievementIds.MonsterHunter, "Monster Hunter", "Defeat 10 enemies.", 10),
            Create(AchievementIds.DeathBringerDown, "DeathBringer Down", "Defeat the DeathBringer.", 1),
            Create(AchievementIds.SoulCollector, "Soul Collector", "Collect 1000 souls.", 1000),
            Create(AchievementIds.SafePlace, "Safe Place", "Activate your first checkpoint.", 1),
            Create(AchievementIds.SkillAwakened, "Skill Awakened", "Unlock your first skill.", 1),
            Create(AchievementIds.WellEquipped, "Well Equipped", "Equip your first item.", 1),
            Create(AchievementIds.FirstCraft, "First Craft", "Craft your first item.", 1),
            Create(AchievementIds.NotToday, "Not Today", "Use a flask for the first time.", 1),
            Create(AchievementIds.LessonLearned, "Lesson Learned", "Die for the first time.", 1)
        };
    }

    private static AchievementData Create(string id, string title, string description, int targetProgress)
    {
        AchievementData achievement = ScriptableObject.CreateInstance<AchievementData>();
        achievement.achievementId = id;
        achievement.title = title;
        achievement.description = description;
        achievement.targetProgress = targetProgress;
        return achievement;
    }
}
