using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler , IPointerClickHandler ,ISaveManager
{
    private static readonly List<UI_SkillTreeSlot> registeredSlots = new List<UI_SkillTreeSlot>();

    private UI ui;
    private Image skillImage;

    [SerializeField] private int skillCost;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;


    public bool unlocked;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        RegisterSlot(this);

        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() => UnlockSkillSlot());
    }

    private void OnDestroy()
    {
        registeredSlots.Remove(this);
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();
        ui = GetComponentInParent<UI>();

        ApplyVisualState();
    }

    public void UnlockSkillSlot()
    {
        if (unlocked)
            return;

        if (PlayerManager.instance.HaveEnoughMoney(skillCost) == false)
            return;

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i].unlocked == false)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }


        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked == true)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        ApplyVisualState();
        SkillManager.instance?.RefreshSkillUnlocks();
        AchievementManager.instance?.RecordSkillUnlocked();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            RollbackSkillSlot();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDescription,skillName,skillCost, CanRollbackSkill());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
            _data.skillTree.Add(skillName, unlocked);
    }

    public bool CanRollbackSkill()
    {
        return unlocked && IsUpgradeSlot();
    }

    public int RollbackSkillSlot()
    {
        if (!CanRollbackSkill())
            return 0;

        bool wasUnlocked = unlocked;
        int refundAmount = RollbackSkillSlot(new HashSet<UI_SkillTreeSlot>());
        bool rolledBack = wasUnlocked && !unlocked;

        if (refundAmount > 0)
            PlayerManager.instance?.AddCurrency(refundAmount);

        if (rolledBack)
            SkillManager.instance?.RefreshSkillUnlocks();

        return refundAmount;
    }

    private int RollbackSkillSlot(HashSet<UI_SkillTreeSlot> visitedSlots)
    {
        if (!visitedSlots.Add(this) || !CanRollbackSkill())
            return 0;

        int refundAmount = 0;
        foreach (UI_SkillTreeSlot dependentSlot in GetDependentSlots())
        {
            refundAmount += dependentSlot.RollbackSkillSlot(visitedSlots);
        }

        unlocked = false;
        ApplyVisualState();
        return refundAmount + skillCost;
    }

    private IEnumerable<UI_SkillTreeSlot> GetDependentSlots()
    {
        foreach (UI_SkillTreeSlot slot in GetRegisteredSlots())
        {
            if (slot != null && slot != this && slot.DependsOn(this) && slot.unlocked)
                yield return slot;
        }
    }

    private bool DependsOn(UI_SkillTreeSlot requiredSlot)
    {
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (shouldBeUnlocked[i] == requiredSlot)
                return true;
        }

        return false;
    }

    private bool IsUpgradeSlot()
    {
        return shouldBeUnlocked != null && shouldBeUnlocked.Length > 0;
    }

    private void ApplyVisualState()
    {
        if (skillImage == null)
            skillImage = GetComponent<Image>();

        if (skillImage == null)
            return;

        skillImage.color = unlocked ? Color.white : lockedSkillColor;
    }

    private static void RegisterSlot(UI_SkillTreeSlot slot)
    {
        if (slot != null && !registeredSlots.Contains(slot))
            registeredSlots.Add(slot);
    }

    private static List<UI_SkillTreeSlot> GetRegisteredSlots()
    {
        List<UI_SkillTreeSlot> slots = new List<UI_SkillTreeSlot>(registeredSlots);

        foreach (UI_SkillTreeSlot slot in FindObjectsOfType<UI_SkillTreeSlot>(true))
        {
            if (!slots.Contains(slot))
                slots.Add(slot);
        }

        return slots;
    }

#if UNITY_EDITOR
    public void ConfigureForTests(string testSkillName, int testSkillCost, bool testUnlocked, UI_SkillTreeSlot[] testShouldBeUnlocked)
    {
        skillName = testSkillName;
        skillCost = testSkillCost;
        unlocked = testUnlocked;
        shouldBeUnlocked = testShouldBeUnlocked;
        shouldBeLocked = new UI_SkillTreeSlot[0];
        ApplyVisualState();
    }

    public int RollbackSkillSlotForTests() => RollbackSkillSlot();

    public static void RegisterSlotForTests(UI_SkillTreeSlot slot) => RegisterSlot(slot);

    public static void ClearRegisteredSlotsForTests() => registeredSlots.Clear();
#endif
}
