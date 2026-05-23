using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Achievements : MonoBehaviour
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private TextMeshProUGUI summaryText;

    private readonly List<UI_AchievementSlot> slots = new List<UI_AchievementSlot>();

    private void Awake()
    {
        EnsureVisuals();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        if (AchievementManager.instance != null)
            AchievementManager.instance.onAchievementUnlocked += OnAchievementUnlocked;

        Refresh();
    }

    private void OnDestroy()
    {
        if (AchievementManager.instance != null)
            AchievementManager.instance.onAchievementUnlocked -= OnAchievementUnlocked;
    }

    public void Refresh()
    {
        EnsureVisuals();

        AchievementManager manager = AchievementManager.instance;
        if (manager == null)
            return;

        ClearSlots();

        int unlockedCount = 0;
        IReadOnlyList<AchievementData> achievements = manager.Achievements;

        foreach (AchievementData achievement in achievements)
        {
            if (achievement == null)
                continue;

            if (manager.IsUnlocked(achievement.achievementId))
                unlockedCount++;

            UI_AchievementSlot slot = CreateSlot();
            slot.Setup(achievement, manager);
            slots.Add(slot);
        }

        summaryText.text = LocalizationText.Translate("Achievements") + " " + unlockedCount + "/" + achievements.Count;
    }

    private void OnAchievementUnlocked(AchievementData achievement)
    {
        Refresh();
    }

    private UI_AchievementSlot CreateSlot()
    {
        GameObject slotObject = new GameObject("AchievementSlot", typeof(RectTransform));
        slotObject.transform.SetParent(slotParent, false);
        return slotObject.AddComponent<UI_AchievementSlot>();
    }

    private void ClearSlots()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i] != null)
                Destroy(slots[i].gameObject);
        }

        slots.Clear();
    }

    private void EnsureVisuals()
    {
        RectTransform panelRect = GetComponent<RectTransform>();
        if (panelRect == null)
            panelRect = gameObject.AddComponent<RectTransform>();

        panelRect.anchorMin = new Vector2(.1f, .1f);
        panelRect.anchorMax = new Vector2(.9f, .9f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = EnsureComponent<Image>(gameObject);
        panelImage.color = new Color(.03f, .035f, .045f, .96f);

        VerticalLayoutGroup panelLayout = EnsureComponent<VerticalLayoutGroup>(gameObject);
        panelLayout.padding = new RectOffset(18, 18, 18, 18);
        panelLayout.spacing = 12;
        panelLayout.childControlWidth = true;
        panelLayout.childControlHeight = true;
        panelLayout.childForceExpandWidth = true;
        panelLayout.childForceExpandHeight = false;

        summaryText = EnsureTextChild(transform, "Summary", 26, FontStyles.Bold);
        summaryText.alignment = TextAlignmentOptions.MidlineLeft;

        GameObject viewport = EnsureChild(transform, "Viewport");
        Image viewportImage = EnsureComponent<Image>(viewport);
        viewportImage.color = Color.clear;
        EnsureComponent<Mask>(viewport).showMaskGraphic = false;

        LayoutElement viewportLayout = EnsureComponent<LayoutElement>(viewport);
        viewportLayout.flexibleHeight = 1;

        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.sizeDelta = new Vector2(0, 0);

        GameObject content = EnsureChild(viewport.transform, "Content");
        slotParent = content.transform;

        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);

        VerticalLayoutGroup contentLayout = EnsureComponent<VerticalLayoutGroup>(content);
        contentLayout.spacing = 8;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        ContentSizeFitter contentFitter = EnsureComponent<ContentSizeFitter>(content);
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = EnsureComponent<ScrollRect>(gameObject);
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

    private static GameObject EnsureChild(Transform parent, string childName)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
            return existing.gameObject;

        GameObject child = new GameObject(childName, typeof(RectTransform));
        child.transform.SetParent(parent, false);
        return child;
    }

    private static TextMeshProUGUI EnsureTextChild(Transform parent, string childName, int fontSize, FontStyles style)
    {
        GameObject child = EnsureChild(parent, childName);
        TextMeshProUGUI text = EnsureComponent<TextMeshProUGUI>(child);
        text.text = "";
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        return text;
    }

    private static T EnsureComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();

        if (component == null)
            component = target.AddComponent<T>();

        return component;
    }
}
