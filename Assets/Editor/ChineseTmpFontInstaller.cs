#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

[InitializeOnLoad]
public static class ChineseTmpFontInstaller
{
    private const string SourceFontPath = "Assets/Graphics/Font/NotoSansSC-VF.ttf";
    private const string FontAssetPath = "Assets/Graphics/Font/NotoSansSC-VF SDF.asset";
    private const int AtlasSize = 2048;
    private const int SamplingPointSize = 90;
    private const int AtlasPadding = 9;
    private static readonly Color FontFaceColor = Color.white;
    private static readonly Color FontOutlineColor = Color.black;
    private static readonly Color FontUnderlayColor = new Color(0f, 0f, 0f, .5f);
    private const string RequiredCharacters = "的一是在不了有和人这中大为上个国我以要他时来用们生到作地于出就分对成会可主发年动同工也能下过子说产种面而方后多定行学法所民得经十三之进着等部度家电力里如水化高自二理起小物现实加量都两体制机当使点从业本去把性好应开它合还因由其些然前外天政四日那社义事平形相全表间样与关各重新线内数正心反你明看原又么利比或但质气第向道命此变条只没结解问意建月公无系军很情者最立代想已通并提直题党程展五果料象员革位入常文总次品式活设及管特件长求老头基资边流路级少图山统接知较将组见计别她手角期根论运农指几九区强放决西被干做必战先回则任取据处队南给色光门即保治北造百规热领七海口东导器压志世金增争济阶油思术极交受联";
    private static readonly string[] TextSourcePaths =
    {
        "Assets/Data",
        "Assets/Scenes",
        "Assets/Prefabs",
        "Assets/Scripts"
    };

    static ChineseTmpFontInstaller()
    {
        EditorApplication.delayCall += EnsureChineseFallback;
    }

    [MenuItem("Tools/Localization/Rebuild Chinese TMP Fallback")]
    public static void RebuildChineseFallbackFromMenu()
    {
        EnsureChineseFallback();
    }

    private static void EnsureChineseFallback()
    {
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(SourceFontPath);

        if (sourceFont == null)
            return;

        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        string characters = CollectProjectCharacters();

        if (ShouldRebuild(fontAsset, characters))
        {
            if (fontAsset != null)
                AssetDatabase.DeleteAsset(FontAssetPath);

            fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                SamplingPointSize,
                AtlasPadding,
                GlyphRenderMode.SDFAA,
                AtlasSize,
                AtlasSize,
                AtlasPopulationMode.Dynamic,
                true);

            if (fontAsset == null)
                return;

            fontAsset.name = Path.GetFileNameWithoutExtension(FontAssetPath);
            InitializePrimaryAtlasTexture(fontAsset);
            AssetDatabase.CreateAsset(fontAsset, FontAssetPath);

            string missingCharacters;

            fontAsset.TryAddCharacters(characters, out missingCharacters);
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Static;
            ConfigureReadableMaterial(fontAsset);
            AddGeneratedSubAssets(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(FontAssetPath);

            if (!string.IsNullOrEmpty(missingCharacters))
                Debug.LogWarning($"Chinese TMP fallback could not include {missingCharacters.Length} characters.");
        }
        else if (ConfigureReadableMaterial(fontAsset))
        {
            EditorUtility.SetDirty(fontAsset.material);
            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
        }

        TMP_Settings settings = TMP_Settings.instance;

        if (settings == null)
            return;

        TMP_Settings.fallbackFontAssets.RemoveAll(fallbackFont => fallbackFont == null);

        if (TMP_Settings.fallbackFontAssets.Contains(fontAsset))
            return;

        TMP_Settings.fallbackFontAssets.Add(fontAsset);
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    private static bool ShouldRebuild(TMP_FontAsset fontAsset, string requiredCharacters)
    {
        if (fontAsset == null)
            return true;

        if (fontAsset.material == null
            || fontAsset.atlasTextures == null
            || fontAsset.atlasTextures.Length == 0
            || IsInvalidAtlasTexture(fontAsset.atlasTextures[0]))
            return true;

        if (!HasReadableMaterial(fontAsset.material))
            return true;

        SerializedObject serializedFontAsset = new SerializedObject(fontAsset);
        SerializedProperty atlasTextureIndex = serializedFontAsset.FindProperty("m_AtlasTextureIndex");
        SerializedProperty atlasTextures = serializedFontAsset.FindProperty("m_AtlasTextures");

        if (atlasTextureIndex != null && atlasTextures != null && atlasTextures.isArray)
        {
            int activeAtlasCount = Mathf.Clamp(atlasTextureIndex.intValue + 1, 1, atlasTextures.arraySize);

            if (atlasTextureIndex.intValue < 0 || atlasTextureIndex.intValue >= atlasTextures.arraySize)
                return true;

            for (int i = 0; i < activeAtlasCount; i++)
            {
                SerializedProperty atlasTexture = atlasTextures.GetArrayElementAtIndex(i);

                if (atlasTexture.objectReferenceValue == null)
                    return true;

                Texture2D texture = atlasTexture.objectReferenceValue as Texture2D;

                if (IsInvalidAtlasTexture(texture))
                    return true;
            }
        }

        foreach (char character in requiredCharacters)
        {
            if (!fontAsset.HasCharacter(character))
                return true;
        }

        return false;
    }

    private static bool IsInvalidAtlasTexture(Texture2D texture)
    {
        return texture == null || texture.width <= 0 || texture.height <= 0;
    }

    private static string CollectProjectCharacters()
    {
        HashSet<char> characters = new HashSet<char>(RequiredCharacters);

        foreach (string rootPath in TextSourcePaths)
        {
            string absoluteRootPath = ToAbsoluteAssetPath(rootPath);

            if (!Directory.Exists(absoluteRootPath))
                continue;

            foreach (string filePath in Directory.EnumerateFiles(absoluteRootPath, "*.*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (extension != ".cs" && extension != ".unity" && extension != ".prefab" && extension != ".asset")
                    continue;

                string text = File.ReadAllText(filePath, Encoding.UTF8);

                foreach (char character in text)
                {
                    if (character >= 0x20 && !char.IsSurrogate(character))
                        characters.Add(character);
                }
            }
        }

        return new string(characters.OrderBy(character => character).ToArray());
    }

    private static string ToAbsoluteAssetPath(string assetPath)
    {
        if (assetPath == "Assets")
            return Application.dataPath;

        string relativePath = assetPath.StartsWith("Assets/")
            ? assetPath.Substring("Assets/".Length)
            : assetPath;

        return Path.GetFullPath(Path.Combine(Application.dataPath, relativePath));
    }

    private static void InitializePrimaryAtlasTexture(TMP_FontAsset fontAsset)
    {
        _ = fontAsset.atlasTexture;
    }

    private static bool HasReadableMaterial(Material material)
    {
        if (material == null)
            return false;

        if (!HasColor(material, ShaderUtilities.ID_FaceColor, FontFaceColor))
            return false;

        if (!HasColor(material, ShaderUtilities.ID_OutlineColor, FontOutlineColor))
            return false;

        return HasColor(material, ShaderUtilities.ID_UnderlayColor, FontUnderlayColor);
    }

    private static bool HasColor(Material material, int propertyId, Color expectedColor)
    {
        return material.HasProperty(propertyId) && ColorsMatch(material.GetColor(propertyId), expectedColor);
    }

    private static bool ColorsMatch(Color currentColor, Color expectedColor)
    {
        return Mathf.Abs(currentColor.r - expectedColor.r) < .01f
            && Mathf.Abs(currentColor.g - expectedColor.g) < .01f
            && Mathf.Abs(currentColor.b - expectedColor.b) < .01f
            && Mathf.Abs(currentColor.a - expectedColor.a) < .01f;
    }

    private static bool ConfigureReadableMaterial(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null || fontAsset.material == null)
            return false;

        bool changed = false;

        changed |= SetMaterialColor(fontAsset.material, ShaderUtilities.ID_FaceColor, FontFaceColor);
        changed |= SetMaterialColor(fontAsset.material, ShaderUtilities.ID_OutlineColor, FontOutlineColor);
        changed |= SetMaterialColor(fontAsset.material, ShaderUtilities.ID_UnderlayColor, FontUnderlayColor);

        return changed;
    }

    private static bool SetMaterialColor(Material material, int propertyId, Color color)
    {
        if (!material.HasProperty(propertyId) || ColorsMatch(material.GetColor(propertyId), color))
            return false;

        material.SetColor(propertyId, color);
        return true;
    }

    private static void AddGeneratedSubAssets(TMP_FontAsset fontAsset)
    {
        if (fontAsset.material != null)
        {
            fontAsset.material.name = fontAsset.name + " Material";

            if (!AssetDatabase.Contains(fontAsset.material))
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
        }

        if (fontAsset.atlasTextures == null)
            return;

        for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
        {
            Texture2D atlasTexture = fontAsset.atlasTextures[i];

            if (atlasTexture == null)
                continue;

            atlasTexture.name = fontAsset.name + " Atlas";

            if (!AssetDatabase.Contains(atlasTexture))
                AssetDatabase.AddObjectToAsset(atlasTexture, fontAsset);
        }
    }
}
#endif
