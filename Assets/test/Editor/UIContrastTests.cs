using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIContrastTests
{
    [Test]
    public void PauseBackdrop_DefaultColorIsOpaqueBlack()
    {
        UI ui = CreateInactiveUI();
        Color backdropColor = GetPrivateColor(ui, "pauseBackdropColor");

        Assert.AreEqual(0f, backdropColor.r);
        Assert.AreEqual(0f, backdropColor.g);
        Assert.AreEqual(0f, backdropColor.b);
        Assert.AreEqual(1f, backdropColor.a);

        Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void PauseMenuText_AppliesLocalizationToStaticLabels()
    {
        UI ui = CreateInactiveUI();
        GameObject optionsRoot = new GameObject("OptionsRoot");
        optionsRoot.transform.SetParent(ui.transform, false);

        TextMeshProUGUI label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI))
            .GetComponent<TextMeshProUGUI>();
        label.transform.SetParent(optionsRoot.transform);
        label.text = "Show health bar avobe player";

        InvokePrivate(ui, "ApplyReadableTextColors", optionsRoot);

        Assert.AreEqual("显示玩家血条", label.text);

        Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void DeathScreenText_AppliesReadableTextColors()
    {
        UI ui = CreateInactiveUI();
        GameObject endTextRoot = new GameObject("EndText");
        GameObject restartButtonRoot = new GameObject("RestartButton");
        endTextRoot.transform.SetParent(ui.transform, false);
        restartButtonRoot.transform.SetParent(ui.transform, false);

        TextMeshProUGUI endText = CreateText(endTextRoot.transform, Color.black);
        TextMeshProUGUI restartText = CreateText(restartButtonRoot.transform, Color.black);
        SetPrivateField(ui, "endText", endTextRoot);
        SetPrivateField(ui, "restartButton", restartButtonRoot);

        InvokePrivate(ui, "ApplyReadablePauseTextColors");

        AssertReadable(endText.color);
        AssertReadable(restartText.color);

        Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void SwitchOnEndScreen_EnablesNonBlockingBlackBackdrop()
    {
        UI ui = CreateInactiveUI();
        GameObject endTextRoot = new GameObject("EndText");
        GameObject restartButtonRoot = new GameObject("RestartButton");
        UI_FadeScreen fadeScreen = new GameObject("FadeScreen").AddComponent<UI_FadeScreen>();
        Animator animator = fadeScreen.gameObject.AddComponent<Animator>();
        endTextRoot.transform.SetParent(ui.transform, false);
        restartButtonRoot.transform.SetParent(ui.transform, false);
        fadeScreen.transform.SetParent(ui.transform, false);

        SetPrivateField(ui, "endText", endTextRoot);
        SetPrivateField(ui, "restartButton", restartButtonRoot);
        SetPrivateField(ui, "fadeScreen", fadeScreen);
        SetPrivateField(fadeScreen, "anim", animator);
        ui.gameObject.SetActive(true);

        ui.SwitchOnEndScreen();

        GameObject backdrop = ui.transform.Find("PauseBackdrop").gameObject;
        Image backdropImage = backdrop.GetComponent<Image>();
        Assert.IsTrue(backdrop.activeSelf);
        Assert.IsFalse(backdropImage.raycastTarget);
        Assert.AreEqual(Color.black, backdropImage.color);

        Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void SkillTooltip_DefaultTextColorsAreReadableOnDarkPanels()
    {
        UI_SkillToolTip tooltip = new GameObject("SkillTooltip").AddComponent<UI_SkillToolTip>();

        AssertReadable(GetPrivateColor(tooltip, "skillNameColor"));
        AssertReadable(GetPrivateColor(tooltip, "skillTextColor"));
        AssertReadable(GetPrivateColor(tooltip, "skillCostColor"));

        Object.DestroyImmediate(tooltip.gameObject);
    }

    [Test]
    public void CraftSlot_SetupWithNullData_ClearsVisualsAndIgnoresClick()
    {
        UI ui = CreateInactiveUI();
        UI_CraftSlot slot = CreateCraftSlot(ui.transform);
        InvokePrivate(slot, "Start");

        Assert.DoesNotThrow(() => slot.SetupCraftSlot(null));
        Assert.DoesNotThrow(() => slot.OnPointerDown(new PointerEventData(EventSystem.current)));

        Image itemImage = GetPrivateField<Image>(slot, "itemImage");
        TextMeshProUGUI itemText = GetPrivateField<TextMeshProUGUI>(slot, "itemText");

        Assert.IsNull(slot.item);
        Assert.IsNull(itemImage.sprite);
        Assert.AreEqual(0f, itemImage.color.a);
        Assert.AreEqual(string.Empty, itemText.text);

        Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void CraftWindow_SetupWithNullData_ClearsWindowWithoutThrowing()
    {
        UI_CraftWindow craftWindow = CreateCraftWindow(out Image itemIcon, out TextMeshProUGUI itemName, out TextMeshProUGUI itemDescription, out Image materialImage);

        Assert.DoesNotThrow(() => craftWindow.SetupCraftWindow(null));

        Assert.IsNull(itemIcon.sprite);
        Assert.AreEqual(0f, itemIcon.color.a);
        Assert.AreEqual(string.Empty, itemName.text);
        Assert.AreEqual(string.Empty, itemDescription.text);
        Assert.IsNull(materialImage.sprite);
        Assert.AreEqual(0f, materialImage.color.a);

        Object.DestroyImmediate(craftWindow.gameObject);
    }

    [Test]
    public void CraftList_SetupCraftList_SkipsNullEquipmentEntries()
    {
        GameObject uiRoot = new GameObject("UI");
        uiRoot.SetActive(false);
        uiRoot.AddComponent<UI>();
        GameObject listRoot = new GameObject("CraftList");
        listRoot.transform.SetParent(uiRoot.transform, false);

        UI_CraftList craftList = listRoot.AddComponent<UI_CraftList>();
        Transform slotParent = new GameObject("SlotParent").transform;
        slotParent.SetParent(listRoot.transform, false);
        GameObject craftSlotPrefab = CreateCraftSlotPrefab();

        SetPrivateField(craftList, "craftSlotParent", slotParent);
        SetPrivateField(craftList, "craftSlotPrefab", craftSlotPrefab);
        SetPrivateField(craftList, "craftEquipment", new System.Collections.Generic.List<ItemData_Equipment>
        {
            null,
            CreateEquipment("Iron Sword"),
            null
        });

        craftList.SetupCraftList();

        Assert.AreEqual(1, slotParent.childCount);

        Object.DestroyImmediate(uiRoot);
        Object.DestroyImmediate(craftSlotPrefab);
    }

    private static Color GetPrivateColor(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field, $"Missing field: {fieldName}");
        return (Color)field.GetValue(target);
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = FindField(target.GetType(), fieldName);

        Assert.NotNull(field, $"Missing field: {fieldName}");
        return (T)field.GetValue(target);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = FindField(target.GetType(), fieldName);

        Assert.NotNull(field, $"Missing field: {fieldName}");
        field.SetValue(target, value);
    }

    private static FieldInfo FindField(System.Type type, string fieldName)
    {
        while (type != null)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
                return field;

            type = type.BaseType;
        }

        return null;
    }

    private static UI CreateInactiveUI()
    {
        GameObject uiRoot = new GameObject("UI");
        uiRoot.SetActive(false);

        return uiRoot.AddComponent<UI>();
    }

    private static TextMeshProUGUI CreateText(Transform parent, Color color)
    {
        TextMeshProUGUI text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI))
            .GetComponent<TextMeshProUGUI>();
        text.transform.SetParent(parent, false);
        text.text = "Readable text";
        text.color = color;

        return text;
    }

    private static void InvokePrivate(object target, string methodName, params object[] parameters)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method, $"Missing method: {methodName}");
        method.Invoke(target, parameters);
    }

    private static UI_CraftSlot CreateCraftSlot(Transform parent)
    {
        GameObject slotRoot = new GameObject("CraftSlot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        slotRoot.transform.SetParent(parent, false);
        UI_CraftSlot slot = slotRoot.AddComponent<UI_CraftSlot>();

        Image itemImage = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        itemImage.transform.SetParent(slotRoot.transform, false);
        itemImage.color = Color.white;

        TextMeshProUGUI itemText = new GameObject("ItemName", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        itemText.transform.SetParent(slotRoot.transform, false);
        itemText.text = "Placeholder";

        SetPrivateField(slot, "itemImage", itemImage);
        SetPrivateField(slot, "itemText", itemText);

        return slot;
    }

    private static GameObject CreateCraftSlotPrefab()
    {
        GameObject prefab = new GameObject("CraftSlotPrefab", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        UI_CraftSlot slot = prefab.AddComponent<UI_CraftSlot>();

        Image itemImage = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        itemImage.transform.SetParent(prefab.transform, false);

        TextMeshProUGUI itemText = new GameObject("ItemName", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        itemText.transform.SetParent(prefab.transform, false);

        SetPrivateField(slot, "itemImage", itemImage);
        SetPrivateField(slot, "itemText", itemText);

        return prefab;
    }

    private static UI_CraftWindow CreateCraftWindow(out Image itemIcon, out TextMeshProUGUI itemName, out TextMeshProUGUI itemDescription, out Image materialImage)
    {
        UI_CraftWindow craftWindow = new GameObject("CraftWindow").AddComponent<UI_CraftWindow>();
        itemIcon = new GameObject("ItemIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        itemName = new GameObject("ItemName", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        itemDescription = new GameObject("ItemDescription", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        Button craftButton = new GameObject("CraftButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<Button>();
        materialImage = new GameObject("Material", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        TextMeshProUGUI materialText = new GameObject("MaterialText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();

        itemIcon.transform.SetParent(craftWindow.transform, false);
        itemName.transform.SetParent(craftWindow.transform, false);
        itemDescription.transform.SetParent(craftWindow.transform, false);
        craftButton.transform.SetParent(craftWindow.transform, false);
        materialImage.transform.SetParent(craftWindow.transform, false);
        materialText.transform.SetParent(materialImage.transform, false);

        itemIcon.color = Color.white;
        materialImage.color = Color.white;
        itemName.text = "Name";
        itemDescription.text = "Description";
        materialText.text = "1";

        SetPrivateField(craftWindow, "itemIcon", itemIcon);
        SetPrivateField(craftWindow, "itemName", itemName);
        SetPrivateField(craftWindow, "itemDescription", itemDescription);
        SetPrivateField(craftWindow, "craftButton", craftButton);
        SetPrivateField(craftWindow, "materialImage", new[] { materialImage });

        return craftWindow;
    }

    private static ItemData_Equipment CreateEquipment(string itemName)
    {
        ItemData_Equipment equipment = ScriptableObject.CreateInstance<ItemData_Equipment>();
        equipment.itemName = itemName;
        equipment.itemEffects = new ItemEffect[0];
        equipment.craftingMaterials = new System.Collections.Generic.List<InventoryItem>();

        return equipment;
    }

    private static void AssertReadable(Color color)
    {
        float luminance = color.r * 0.2126f + color.g * 0.7152f + color.b * 0.0722f;

        Assert.GreaterOrEqual(luminance, 0.75f);
        Assert.GreaterOrEqual(color.a, 0.95f);
    }
}
