using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ChineseTmpFontInstallerTests
{
    [Test]
    public void ChineseFallbackFontAsset_HasValidAtlasTexture()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Graphics/Font/NotoSansSC-VF SDF.asset");

        Assert.NotNull(fontAsset);
        Assert.NotNull(fontAsset.atlasTextures);
        Assert.Greater(fontAsset.atlasTextures.Length, 0);

        Texture2D atlasTexture = fontAsset.atlasTextures[0];

        Assert.NotNull(atlasTexture);
        Assert.Greater(atlasTexture.width, 0);
        Assert.Greater(atlasTexture.height, 0);
    }

    [Test]
    public void ChineseFallbackFontAsset_ContainsTooltipChineseCharacters()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Graphics/Font/NotoSansSC-VF SDF.asset");

        Assert.NotNull(fontAsset);

        foreach (char character in "爆裂技能闪避")
        {
            Assert.IsTrue(fontAsset.HasCharacter(character), $"Missing character: {character}");
        }
    }

    [Test]
    public void ChineseFallbackFontAsset_ContainsSkillTooltipSentenceCharacters()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Graphics/Font/NotoSansSC-VF SDF.asset");

        Assert.NotNull(fontAsset);

        foreach (char character in "生成造成魔法伤害的魔法水晶。再次使用技能可以传送到水晶处。")
        {
            Assert.IsTrue(fontAsset.HasCharacter(character), $"Missing character: {character}");
        }
    }

    [Test]
    public void ChineseFallbackFontAsset_ContainsDataAssetCharacters()
    {
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Graphics/Font/NotoSansSC-VF SDF.asset");

        Assert.NotNull(fontAsset);

        foreach (char character in "药瓶冻结恢复生命冷却")
        {
            Assert.IsTrue(fontAsset.HasCharacter(character), $"Missing character: {character}");
        }
    }
}
