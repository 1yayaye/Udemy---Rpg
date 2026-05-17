using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WebGLScoreTrackerTests
{
    private GameObject trackerObject;
    private WebGLScoreTracker tracker;

    [SetUp]
    public void SetUp()
    {
        WebGLScoreTracker.instance = null;
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));
        trackerObject = new GameObject("WebGL Score Tracker Test");
        tracker = trackerObject.AddComponent<WebGLScoreTracker>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(trackerObject);
        WebGLScoreTracker.instance = null;
    }

    [Test]
    public void AddScore_AccumulatesScoreAndPublishesState()
    {
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));

        tracker.AddScore(100);
        tracker.AddScore(41);

        Assert.AreEqual(141, tracker.CurrentScore);
    }

    [Test]
    public void ResetRun_ClearsSubmittedFlagAfterDeathSubmission()
    {
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase score payload: .*"));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));
        LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex("Showcase game state payload: .*"));

        tracker.AddScore(100);
        tracker.SubmitRun(false);

        Assert.IsTrue(tracker.HasSubmitted);

        tracker.ResetRun();

        Assert.AreEqual(0, tracker.CurrentScore);
        Assert.IsFalse(tracker.HasSubmitted);
    }
}
