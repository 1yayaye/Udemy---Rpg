using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Data/Achievement")]
public class AchievementData : ScriptableObject
{
    public string achievementId;
    public string title;
    [TextArea]
    public string description;
    public Sprite icon;
    public int targetProgress = 1;
    public bool hidden;

    public int TargetProgress => Mathf.Max(1, targetProgress);

#if UNITY_EDITOR
    public void ConfigureForTests(string testId, string testTitle, string testDescription, int testTargetProgress)
    {
        achievementId = testId;
        title = testTitle;
        description = testDescription;
        targetProgress = testTargetProgress;
        hidden = false;
    }
#endif
}
