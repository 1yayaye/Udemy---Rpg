using NUnit.Framework;

public class SerializableDictionaryTests
{
    [Test]
    public void OnBeforeSerializeAndOnAfterDeserialize_PreservesEntries()
    {
        SerializableDictionary<string, int> dictionary = new SerializableDictionary<string, int>();
        dictionary.Add("sword", 1);
        dictionary.Add("potion", 3);

        dictionary.OnBeforeSerialize();
        dictionary.OnAfterDeserialize();

        Assert.AreEqual(2, dictionary.Count);
        Assert.AreEqual(1, dictionary["sword"]);
        Assert.AreEqual(3, dictionary["potion"]);
    }

    [Test]
    public void OnBeforeSerializeAndOnAfterDeserialize_PreservesEmptyDictionary()
    {
        SerializableDictionary<string, int> dictionary = new SerializableDictionary<string, int>();

        dictionary.OnBeforeSerialize();
        dictionary.OnAfterDeserialize();

        Assert.AreEqual(0, dictionary.Count);
    }
}
