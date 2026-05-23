using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AchievementSlot : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;

    [SerializeField] private Color unlockedColor = new Color(.18f, .23f, .18f, .95f);
    [SerializeField] private Color lockedColor = new Color(.12f, .12f, .14f, .95f);
    [SerializeField] private Color placeholderIconColor = new Color(.5f, .43f, .22f, 1f);

    private void Awake()
    {
        EnsureVisuals();
    }

    public void Setup(AchievementData achievement, AchievementManager manager)
    {
        EnsureVisuals();

        if (achievement == null || manager == null)
            return;

        bool unlocked = manager.IsUnlocked(achievement.achievementId);
        int progress = manager.GetProgress(achievement.achievementId);
        int targetProgress = achievement.TargetProgress;
        bool hidden = achievement.hidden && !unlocked;

        background.color = unlocked ? unlockedColor : lockedColor;
        titleText.text = hidden
            ? LocalizationText.Translate("Hidden Achievement")
            : LocalizationText.Translate(achievement.title);
        descriptionText.text = hidden
            ? LocalizationText.Translate("Keep playing to reveal this achievement.")
            : LocalizationText.Translate(achievement.description);
        progressText.text = unlocked
            ? LocalizationText.Translate("Unlocked")
            : Mathf.Clamp(progress, 0, targetProgress) + "/" + targetProgress;

        icon.sprite = hidden ? null : achievement.icon;
        icon.color = achievement.icon == null || hidden ? placeholderIconColor : Color.white;
    }

    private void EnsureVisuals()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(0, 76);

        background = EnsureComponent<Image>(gameObject);

        HorizontalLayoutGroup layout = EnsureComponent<HorizontalLayoutGroup>(gameObject);
        layout.padding = new RectOffset(10, 10, 8, 8);
        layout.spacing = 10;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        icon = EnsureImageChild("Icon", new Vector2(56, 56));

        GameObject textColumn = EnsureChild("Text");
        VerticalLayoutGroup textLayout = EnsureComponent<VerticalLayoutGroup>(textColumn);
        textLayout.spacing = 2;
        textLayout.childControlWidth = true;
        textLayout.childControlHeight = true;
        textLayout.childForceExpandWidth = true;
        textLayout.childForceExpandHeight = false;

        LayoutElement textLayoutElement = EnsureComponent<LayoutElement>(textColumn);
        textLayoutElement.flexibleWidth = 1;

        titleText = EnsureTextChild(textColumn, "Title", 20, FontStyles.Bold);
        descriptionText = EnsureTextChild(textColumn, "Description", 16, FontStyles.Normal);

        progressText = EnsureTextChild(gameObject, "Progress", 18, FontStyles.Bold);
        RectTransform progressRect = progressText.GetComponent<RectTransform>();
        progressRect.sizeDelta = new Vector2(82, 56);
        progressText.alignment = TextAlignmentOptions.MidlineRight;
    }

    private Image EnsureImageChild(string childName, Vector2 size)
    {
        GameObject child = EnsureChild(childName);
        Image image = EnsureComponent<Image>(child);
        RectTransform rectTransform = child.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        LayoutElement layoutElement = EnsureComponent<LayoutElement>(child);
        layoutElement.preferredWidth = size.x;
        layoutElement.preferredHeight = size.y;

        return image;
    }

    private TextMeshProUGUI EnsureTextChild(GameObject parent, string childName, int fontSize, FontStyles style)
    {
        GameObject child = EnsureChild(parent.transform, childName);
        TextMeshProUGUI text = EnsureComponent<TextMeshProUGUI>(child);
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.enableWordWrapping = true;
        return text;
    }

    private GameObject EnsureChild(string childName)
    {
        return EnsureChild(transform, childName);
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

    private static T EnsureComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();

        if (component == null)
            component = target.AddComponent<T>();

        return component;
    }
}
