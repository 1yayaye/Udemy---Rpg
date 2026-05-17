using System.Collections.Generic;
using NUnit.Framework;

public class StatTests
{
    [Test]
    public void GetValue_ReturnsBaseValue_WhenNoModifiersExist()
    {
        Stat stat = CreateStatWithBaseValue(10);

        int value = stat.GetValue();

        Assert.AreEqual(10, value);
    }

    [Test]
    public void GetValue_ReturnsBaseValuePlusAllModifiers()
    {
        Stat stat = CreateStatWithBaseValue(10);

        stat.AddModifier(5);
        stat.AddModifier(-2);

        int value = stat.GetValue();

        Assert.AreEqual(13, value);
    }

    [Test]
    public void RemoveModifier_RemovesOneMatchingModifierFromValue()
    {
        Stat stat = CreateStatWithBaseValue(10);
        stat.AddModifier(5);
        stat.AddModifier(3);

        stat.RemoveModifier(5);

        Assert.AreEqual(13, stat.GetValue());
    }

    private static Stat CreateStatWithBaseValue(int baseValue)
    {
        Stat stat = new Stat();
        stat.modifiers = new List<int>();
        stat.SetDefaultValue(baseValue);
        return stat;
    }
}
