using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AchievementPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private float showSeconds = 3f;

    private Coroutine popupCoroutine;

    private void Awake()
    {
        EnsureVisuals();
        HideImmediate();
    }

    private void OnEnable()
    {
        if (AchievementManager.instance != null)
            AchievementManager.instance.onAchievementUnlocked += Show;
    }

    private void OnDisable()
    {
        if (AchievementManager.instance != null)
            AchievementManager.instance.onAchievementUnlocked -= Show;
    }

    public void Show(AchievementData achievement)
    {
        EnsureVisuals();

        if (achievement == null)
            return;

        titleText.text = LocalizationText.Translate("Achievement Unlocked");
        descriptionText.text = LocalizationText.Translate(achievement.title);
        icon.sprite = achievement.icon;
        icon.color = achievement.icon == null ? new Color(.5f, .43f, .22f, 1f) : Color.white;

        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        popupCoroutine = StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        canvasGroup.alpha = 1;
        yield return new WaitForSecondsRealtime(showSeconds);
        canvasGroup.alpha = 0;
        popupCoroutine = null;
    }

    private void HideImmediate()
    {
        canvasGroup.alpha = 0;
    }

    private void EnsureVisuals()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(.5f, 1);
        rectTransform.anchorMax = new Vector2(.5f, 1);
        rectTransform.pivot = new Vector2(.5f, 1);
        rectTransform.anchoredPosition = new Vector2(0, -32);
        rectTransform.sizeDelta = new Vector2(420, 92);

        Image background = EnsureComponent<Image>(gameObject);
        background.color = new Color(.04f, .05f, .06f, .95f);

        canvasGroup = EnsureComponent<CanvasGroup>(gameObject);
        canvasGroup.blocksRaycasts = false;

        HorizontalLayoutGroup layout = EnsureComponent<HorizontalLayoutGroup>(gameObject);
        layout.padding = new RectOffset(12, 12, 10, 10);
        layout.spacing = 12;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        GameObject iconObject = EnsureChild(transform, "Icon");
        icon = EnsureComponent<Image>(iconObject);
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(64, 64);

        LayoutElement iconLayout = EnsureComponent<LayoutElement>(iconObject);
        iconLayout.preferredWidth = 64;
        iconLayout.preferredHeight = 64;

        GameObject textColumn = EnsureChild(transform, "Text");
        VerticalLayoutGroup textLayout = EnsureComponent<VerticalLayoutGroup>(textColumn);
        textLayout.spacing = 2;

        LayoutElement textLayoutElement = EnsureComponent<LayoutElement>(textColumn);
        textLayoutElement.flexibleWidth = 1;

        titleText = EnsureTextChild(textColumn.transform, "Title", 18, FontStyles.Bold);
        descriptionText = EnsureTextChild(textColumn.transform, "Description", 22, FontStyles.Bold);
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
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = Color.white;
        text.enableWordWrapping = true;
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
