using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("End screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    [Header("Pause backdrop")]
    [SerializeField] private Color pauseBackdropColor = new Color(0f, 0f, 0f, 1f);
    [SerializeField] private Color pauseTextColor = new Color(0.95f, 0.96f, 0.98f, 1f);
    private GameObject pauseBackdrop;

    public UI_SkillToolTip skillToolTip;
    public UI_ItemTooltip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;

    private void Awake()
    {
        EnsurePauseBackdrop();
        ApplyReadablePauseTextColors();

        SwitchTo(skillTreeUI); // we need this to assign events on skill tree slots before we asssign events on skill scripts
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);

        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchWithEscape();
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(charcaterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);


        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);


    }

    private void SwitchWithEscape()
    {
        if (IsEndScreenActive())
            return;

        if (IsAnyMenuOpen())
            SwitchTo(inGameUI);
        else
            SwitchTo(optionsUI);
    }

    private bool IsAnyMenuOpen()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (IsMenuObject(child) && child.activeSelf)
                return true;
        }

        return false;
    }

    private bool IsEndScreenActive()
    {
        return endText.activeSelf || restartButton.activeSelf;
    }


    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (ShouldKeepActiveDuringMenuSwitch(child) == false)
                child.SetActive(false);
        }

        SetPauseBackdropActive(_menu != null && _menu != inGameUI);


        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(5, null);
            _menu.SetActive(true);
        }


        if (GameManager.instance != null)
        {
            if (_menu == inGameUI)
                GameManager.instance.PauseGame(false);
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (IsMenuObject(child) && child.activeSelf)
                return;
        }

        SwitchTo(inGameUI);
    }

    private void EnsurePauseBackdrop()
    {
        Transform existingBackdrop = transform.Find("PauseBackdrop");

        if (existingBackdrop != null)
            pauseBackdrop = existingBackdrop.gameObject;
        else
            pauseBackdrop = new GameObject("PauseBackdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

        pauseBackdrop.transform.SetParent(transform, false);
        pauseBackdrop.transform.SetSiblingIndex(0);

        RectTransform backdropRect = pauseBackdrop.GetComponent<RectTransform>();
        backdropRect.anchorMin = Vector2.zero;
        backdropRect.anchorMax = Vector2.one;
        backdropRect.anchoredPosition = Vector2.zero;
        backdropRect.sizeDelta = Vector2.zero;
        backdropRect.pivot = new Vector2(.5f, .5f);

        Image backdropImage = pauseBackdrop.GetComponent<Image>();
        backdropImage.color = pauseBackdropColor;
        backdropImage.raycastTarget = false;

        pauseBackdrop.SetActive(false);
    }

    private void ApplyReadablePauseTextColors()
    {
        ApplyReadableTextColors(charcaterUI);
        ApplyReadableTextColors(skillTreeUI);
        ApplyReadableTextColors(craftUI);
        ApplyReadableTextColors(optionsUI);
    }

    private void ApplyReadableTextColors(GameObject root)
    {
        if (root == null)
            return;

        TextMeshProUGUI[] texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI text in texts)
        {
            text.text = LocalizationText.Translate(text.text);
            Color textColor = text.color;

            if (!IsReadable(textColor))
            {
                textColor = pauseTextColor;
                text.color = textColor;
            }

            UseReadableChineseFontIfNeeded(text);
            text.faceColor = textColor;
        }
    }

    private void UseReadableChineseFontIfNeeded(TextMeshProUGUI text)
    {
        if (!ContainsNonAscii(text.text))
            return;

        TMP_FontAsset fontAsset = FindReadableChineseFont();

        if (fontAsset != null)
            text.font = fontAsset;
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

    private TMP_FontAsset FindReadableChineseFont()
    {
        if (TMP_Settings.fallbackFontAssets == null)
            return null;

        foreach (TMP_FontAsset fontAsset in TMP_Settings.fallbackFontAssets)
        {
            if (fontAsset != null && fontAsset.name.Contains("NotoSansSC"))
                return fontAsset;
        }

        return null;
    }

    private bool IsReadable(Color color)
    {
        float luminance = color.r * 0.2126f + color.g * 0.7152f + color.b * 0.0722f;

        return color.a >= .95f && luminance >= .55f;
    }

    private void SetPauseBackdropActive(bool active)
    {
        if (pauseBackdrop == null)
            EnsurePauseBackdrop();

        pauseBackdrop.SetActive(active);

        if (active)
            pauseBackdrop.transform.SetSiblingIndex(0);
    }

    private bool IsMenuObject(GameObject child)
    {
        return child != inGameUI
            && child != pauseBackdrop
            && child.GetComponent<UI_FadeScreen>() == null;
    }

    private bool ShouldKeepActiveDuringMenuSwitch(GameObject child)
    {
        return child == pauseBackdrop || child.GetComponent<UI_FadeScreen>() != null;
    }

    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);

    }

    public void RestartGameButton() => GameManager.instance.RestartScene();

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (UI_VolumeSlider item in volumeSettings)
            {
                if (item.parametr == pair.Key)
                    item.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
