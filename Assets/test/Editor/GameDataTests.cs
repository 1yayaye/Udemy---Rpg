using NUnit.Framework;

public class GameDataTests
{
    [Test]
    public void Constructor_InitializesDefaultCurrency()
    {
        GameData data = new GameData();

        Assert.AreEqual(2000, data.currency);
    }

    [Test]
    public void Constructor_InitializesCollections()
    {
        GameData data = new GameData();

        Assert.NotNull(data.skillTree);
        Assert.NotNull(data.inventory);
        Assert.NotNull(data.equipmentId);
        Assert.NotNull(data.checkpoints);
        Assert.NotNull(data.volumeSettings);
    }

    [Test]
    public void Constructor_InitializesCheckpointIdToEmptyString()
    {
        GameData data = new GameData();

        Assert.AreEqual(string.Empty, data.closestCheckpointId);
    }

    [Test]
    public void Constructor_InitializesLostCurrencyFieldsToZero()
    {
        GameData data = new GameData();

        Assert.AreEqual(0f, data.lostCurrencyX);
        Assert.AreEqual(0f, data.lostCurrencyY);
        Assert.AreEqual(0, data.lostCurrencyAmount);
    }
}
