using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillCost;
    [SerializeField] private float defaultNameFontSize;
    [SerializeField] private Color skillNameColor = new Color(0.96f, 0.97f, 0.98f, 1f);
    [SerializeField] private Color skillTextColor = new Color(0.87f, 0.89f, 0.92f, 1f);
    [SerializeField] private Color skillCostColor = new Color(0.95f, 0.96f, 0.98f, 1f);

    public void ShowToolTip(string _skillDescprtion,string _skillName,int _price, bool canRollback = false)
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return; // this hides tooltip if you hide left control

        skillName.text = LocalizationText.Translate(_skillName);
        skillText.text = LocalizationText.Translate(_skillDescprtion);
        skillCost.text = LocalizationText.Translate("Cost:") + " " + _price;

        if (canRollback)
            skillCost.text += "\n右键回退，返还 " + _price + " 灵魂";

        ApplyReadableColors();

        AdjustPosition();

        AdjustFontSize(skillName);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        skillName.fontSize = defaultNameFontSize;
        gameObject.SetActive(false);
    }

    private void ApplyReadableColors()
    {
        ApplyReadableColor(skillName, skillNameColor);
        ApplyReadableColor(skillText, skillTextColor);
        ApplyReadableColor(skillCost, skillCostColor);
    }

    private void ApplyReadableColor(TextMeshProUGUI text, Color color)
    {
        if (text == null)
            return;

        text.overrideColorTags = true;
        text.color = color;
        UseReadableChineseFontIfNeeded(text);
        text.faceColor = color;
    }

    private void UseReadableChineseFontIfNeeded(TextMeshProUGUI text)
    {
        if (!ContainsNonAscii(text.text) || TMP_Settings.fallbackFontAssets == null)
            return;

        foreach (TMP_FontAsset fontAsset in TMP_Settings.fallbackFontAssets)
        {
            if (fontAsset != null && fontAsset.name.Contains("NotoSansSC"))
            {
                text.font = fontAsset;
                return;
            }
        }
    }

    private bool ContainsNonAscii(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        foreach (char character in text)
        {
            if (character > 127)
                return true;
        }

        return false;
    }

}
