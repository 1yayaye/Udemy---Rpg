using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

public class CraftingDefaultsTests
{
    private const string IronGuid = "6835c34bd80fc9c468d2f549f4b6220a";

    [Test]
    public void MainScene_NewGameStartingItems_IncludeTenIronMaterials()
    {
        string[] lines = File.ReadAllLines("Assets/Scenes/MainScene.unity");
        bool insideStartingItems = false;
        int ironCount = 0;

        foreach (string line in lines)
        {
            if (line.Trim() == "startingItems:")
            {
                insideStartingItems = true;
                continue;
            }

            if (insideStartingItems && line.StartsWith("  equipment:"))
                break;

            if (insideStartingItems && line.Contains(IronGuid))
                ironCount++;
        }

        Assert.AreEqual(10, ironCount);
    }

    [Test]
    public void GoldenRing_UsesExistingIronMaterialWithoutChangingRequirement()
    {
        ItemData_Equipment goldenRing = AssetDatabase.LoadAssetAtPath<ItemData_Equipment>(
            "Assets/Data/Items/Equipment/Amulet/Golden ring.asset");

        Assert.NotNull(goldenRing);
        Assert.AreEqual(1, goldenRing.craftingMaterials.Count);
        Assert.NotNull(goldenRing.craftingMaterials[0].data);
        Assert.AreEqual(IronGuid, goldenRing.craftingMaterials[0].data.itemId);
        Assert.AreEqual(5, goldenRing.craftingMaterials[0].stackSize);
    }

    [Test]
    public void CraftingMaterials_DoNotContainMissingOrZeroStackEntries()
    {
        string[] equipmentGuids = AssetDatabase.FindAssets("t:ItemData_Equipment", new[] { "Assets/Data/Items/Equipment" });

        foreach (string guid in equipmentGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData_Equipment equipment = AssetDatabase.LoadAssetAtPath<ItemData_Equipment>(path);

            foreach (InventoryItem material in equipment.craftingMaterials)
            {
                Assert.NotNull(material.data, $"{path} has a missing crafting material reference.");
                Assert.Greater(material.stackSize, 0, $"{path} has a non-positive crafting material stack.");
            }
        }
    }

    [Test]
    public void RemoveRequiredMaterialStack_RemovesRequiredStackSize()
    {
        ItemData iron = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/Materials/Iron.asset");
        InventoryItem stashItem = new InventoryItem(iron) { stackSize = 10 };
        var stash = new System.Collections.Generic.List<InventoryItem> { stashItem };
        var stashDictionary = new System.Collections.Generic.Dictionary<ItemData, InventoryItem>
        {
            { iron, stashItem }
        };

        MethodInfo removeStack = typeof(Inventory).GetMethod(
            "RemoveRequiredMaterialStack",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(removeStack);
        removeStack.Invoke(null, new object[] { stash, stashDictionary, iron, 5 });

        Assert.AreEqual(5, stashItem.stackSize);
    }
}
