using NUnit.Framework;

public class LocalizationTextTests
{
    [Test]
    public void Translate_ReturnsChineseSkillDescription_ForExactMultilineText()
    {
        string english = "Crystal can explode.\n\n[Only one crystal upgrade can be choosen]";

        string translated = LocalizationText.Translate(english);

        Assert.AreEqual("水晶可以爆炸。\n[只能选择一个水晶强化]", translated);
    }

    [Test]
    public void Translate_ReturnsChineseSkillDescription_ForUnityIndentedMultilineText()
    {
        string english = "Crystal can explode.\n\n    [Only one crystal upgrade can be choosen]";

        string translated = LocalizationText.Translate(english);

        Assert.AreEqual("水晶可以爆炸。\n[只能选择一个水晶强化]", translated);
    }

    [Test]
    public void Translate_ReturnsChineseName_ForMixedChineseAndEnglishSkillName()
    {
        string translated = LocalizationText.Translate("闪避 mirage");

        Assert.AreEqual("闪避幻影", translated);
    }

    [Test]
    public void Translate_KeepsExistingTranslations()
    {
        Assert.AreEqual("消耗：", LocalizationText.Translate("Cost:"));
        Assert.AreEqual("爆裂", LocalizationText.Translate("Explosion"));
    }

    [Test]
    public void Translate_ReturnsChineseSettingsLabel_ForPlayerHealthBarOption()
    {
        Assert.AreEqual("显示玩家血条", LocalizationText.Translate("Show health bar avobe player"));
    }

    [Test]
    public void Translate_ReturnsOriginalText_WhenTranslationIsMissing()
    {
        string unknown = "This text is intentionally not translated.";

        string translated = LocalizationText.Translate(unknown);

        Assert.AreEqual(unknown, translated);
    }
}
