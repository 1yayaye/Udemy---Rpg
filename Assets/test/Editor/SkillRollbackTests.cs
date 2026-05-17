using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class SkillRollbackTests
{
    [SetUp]
    public void SetUp()
    {
        UI_SkillTreeSlot.ClearRegisteredSlotsForTests();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (UI_SkillTreeSlot slot in Object.FindObjectsOfType<UI_SkillTreeSlot>())
        {
            Object.DestroyImmediate(slot.gameObject);
        }

        foreach (SkillManager skillManager in Object.FindObjectsOfType<SkillManager>())
        {
            Object.DestroyImmediate(skillManager.gameObject);
        }

        SkillManager.instance = null;
        UI_SkillTreeSlot.ClearRegisteredSlotsForTests();
    }

    [Test]
    public void RollbackUnlockedUpgrade_RefundsCostAndLocksSlot()
    {
        UI_SkillTreeSlot baseSlot = CreateSlot("Base", 100, true);
        UI_SkillTreeSlot upgradeSlot = CreateSlot("Upgrade", 50, true, baseSlot);

        int refunded = upgradeSlot.RollbackSkillSlotForTests();

        Assert.AreEqual(50, refunded);
        Assert.IsTrue(baseSlot.unlocked);
        Assert.IsFalse(upgradeSlot.unlocked);
    }

    [Test]
    public void RollbackUpgrade_CascadesToDependentUpgradesAndRefundsAllCosts()
    {
        UI_SkillTreeSlot baseSlot = CreateSlot("Base", 100, true);
        UI_SkillTreeSlot upgradeSlot = CreateSlot("Upgrade", 50, true, baseSlot);
        UI_SkillTreeSlot dependentSlot = CreateSlot("Dependent", 25, true, upgradeSlot);

        int refunded = upgradeSlot.RollbackSkillSlotForTests();

        Assert.AreEqual(75, refunded);
        Assert.IsTrue(baseSlot.unlocked);
        Assert.IsFalse(upgradeSlot.unlocked);
        Assert.IsFalse(dependentSlot.unlocked);
    }

    [Test]
    public void RollbackBaseSkill_DoesNotRefundOrLock()
    {
        UI_SkillTreeSlot baseSlot = CreateSlot("Base", 100, true);

        int refunded = baseSlot.RollbackSkillSlotForTests();

        Assert.AreEqual(0, refunded);
        Assert.IsTrue(baseSlot.unlocked);
    }

    [Test]
    public void RollbackZeroCostUpgrade_RefreshesSkillUnlocks()
    {
        UI_SkillTreeSlot dashSlot = CreateSlot("Dash", 0, true);
        UI_SkillTreeSlot cloneOnDashSlot = CreateSlot("CloneOnDash", 0, true, dashSlot);
        UI_SkillTreeSlot cloneOnArrivalSlot = CreateSlot("CloneOnArrival", 0, false, dashSlot);
        SkillManager skillManager = new GameObject("SkillManager").AddComponent<SkillManager>();
        Dash_Skill dashSkill = skillManager.gameObject.AddComponent<Dash_Skill>();

        SkillManager.instance = skillManager;
        SetPrivateField(skillManager, "<dash>k__BackingField", dashSkill);
        SetPrivateField(dashSkill, "dashUnlockButton", dashSlot);
        SetPrivateField(dashSkill, "cloneOnDashUnlockButton", cloneOnDashSlot);
        SetPrivateField(dashSkill, "cloneOnArrivalUnlockButton", cloneOnArrivalSlot);

        dashSkill.RefreshUnlocks();
        Assert.IsTrue(dashSkill.cloneOnDashUnlocked);

        int refunded = cloneOnDashSlot.RollbackSkillSlotForTests();

        Assert.AreEqual(0, refunded);
        Assert.IsFalse(cloneOnDashSlot.unlocked);
        Assert.IsFalse(dashSkill.cloneOnDashUnlocked);
    }

    [Test]
    public void SwordRefreshUnlocks_AfterSpinRollback_RestoresRegularSwordAndDefaultGravity()
    {
        UI_SkillTreeSlot baseSlot = CreateSlot("Sword", 100, true);
        UI_SkillTreeSlot bounceSlot = CreateSlot("Bounce", 50, false, baseSlot);
        UI_SkillTreeSlot pierceSlot = CreateSlot("Pierce", 50, false, baseSlot);
        UI_SkillTreeSlot spinSlot = CreateSlot("Spin", 50, false, baseSlot);
        UI_SkillTreeSlot timeStopSlot = CreateSlot("TimeStop", 50, false, baseSlot);
        UI_SkillTreeSlot vulnerableSlot = CreateSlot("Vulnerable", 50, false, baseSlot);
        Sword_Skill swordSkill = new GameObject("SwordSkill").AddComponent<Sword_Skill>();
        const float defaultGravity = 3f;
        const float spinGravity = .25f;

        SetPrivateField(swordSkill, "swordUnlockButton", baseSlot);
        SetPrivateField(swordSkill, "bounceUnlockButton", bounceSlot);
        SetPrivateField(swordSkill, "pierceUnlockButton", pierceSlot);
        SetPrivateField(swordSkill, "spinUnlockButton", spinSlot);
        SetPrivateField(swordSkill, "timeStopUnlockButton", timeStopSlot);
        SetPrivateField(swordSkill, "vulnerableUnlockButton", vulnerableSlot);
        SetPrivateField(swordSkill, "swordGravity", spinGravity);
        SetPrivateField(swordSkill, "spinGravity", spinGravity);
        SetPrivateField(swordSkill, "defaultSwordGravity", defaultGravity);
        swordSkill.swordType = SwordType.Spin;

        swordSkill.RefreshUnlocks();

        Assert.AreEqual(SwordType.Regular, swordSkill.swordType);
        Assert.AreEqual(defaultGravity, GetPrivateField<float>(swordSkill, "swordGravity"));
    }

    private UI_SkillTreeSlot CreateSlot(string skillName, int skillCost, bool unlocked, params UI_SkillTreeSlot[] shouldBeUnlocked)
    {
        GameObject gameObject = new GameObject(skillName);
        gameObject.AddComponent<Image>();
        gameObject.AddComponent<Button>();

        UI_SkillTreeSlot slot = gameObject.AddComponent<UI_SkillTreeSlot>();
        slot.ConfigureForTests(skillName, skillCost, unlocked, shouldBeUnlocked);
        UI_SkillTreeSlot.RegisterSlotForTests(slot);

        return slot;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = FindField(target.GetType(), fieldName);

        Assert.NotNull(field, $"Missing field: {fieldName}");
        field.SetValue(target, value);
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        FieldInfo field = FindField(target.GetType(), fieldName);

        Assert.NotNull(field, $"Missing field: {fieldName}");
        return (T)field.GetValue(target);
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
}
