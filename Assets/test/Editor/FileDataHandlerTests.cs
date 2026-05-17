using System.IO;
using NUnit.Framework;

public class FileDataHandlerTests
{
    private string tempDirectory;

    [SetUp]
    public void SetUp()
    {
        tempDirectory = Path.Combine(Path.GetTempPath(), "RpgEditModeTests", Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }
    }

    [Test]
    public void Save_WritesFileAndLoadReadsGameData()
    {
        FileDataHandler handler = new FileDataHandler(tempDirectory, "save.json", false);
        GameData data = new GameData();
        data.currency = 42;

        handler.Save(data);
        GameData loadedData = handler.Load();

        Assert.IsTrue(File.Exists(Path.Combine(tempDirectory, "save.json")));
        Assert.NotNull(loadedData);
        Assert.AreEqual(42, loadedData.currency);
    }

    [Test]
    public void Save_WithEncryption_WritesEncryptedContentAndLoadReadsGameData()
    {
        FileDataHandler handler = new FileDataHandler(tempDirectory, "encrypted-save.json", true);
        GameData data = new GameData();
        data.currency = 77;

        handler.Save(data);
        string savedContent = File.ReadAllText(Path.Combine(tempDirectory, "encrypted-save.json"));
        GameData loadedData = handler.Load();

        Assert.IsFalse(savedContent.Contains("\"currency\": 77"));
        Assert.NotNull(loadedData);
        Assert.AreEqual(77, loadedData.currency);
    }

    [Test]
    public void Delete_RemovesExistingSaveFile()
    {
        FileDataHandler handler = new FileDataHandler(tempDirectory, "save-to-delete.json", false);
        handler.Save(new GameData());

        handler.Delete();

        Assert.IsFalse(File.Exists(Path.Combine(tempDirectory, "save-to-delete.json")));
    }

    [Test]
    public void Load_ReturnsNull_WhenSaveFileDoesNotExist()
    {
        FileDataHandler handler = new FileDataHandler(tempDirectory, "missing-save.json", false);

        GameData loadedData = handler.Load();

        Assert.IsNull(loadedData);
    }
}
