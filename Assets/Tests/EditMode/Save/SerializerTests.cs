using System;
using NUnit.Framework;

// AI生成，review并微调过
public class SerializerTests
{
    [Serializable]
    private sealed class SaveData
    {
        public int Score;
        public string Name;
    }

    // 原生类型
    [Test]
    public void JsonSerializer_RoundTripsPrimitive()
    {
        JsonSerializer serializer = new();

        string json = serializer.Serialize(42);
        int value = serializer.Deserialize<int>(json);

        Assert.That(json, Is.EqualTo("{\"data\":42}"));
        Assert.That(value, Is.EqualTo(42));
    }

    // 类对象
    [Test]
    public void JsonSerializer_RoundTripsObject()
    {
        JsonSerializer serializer = new();
        SaveData original = new() { Score = 99, Name = "Alice" };

        string json = serializer.Serialize(original);
        SaveData loaded = serializer.Deserialize<SaveData>(json);

        // 这一行AI生成时没有写，可能是序列化时key不保序，不过我加上后测试一直能过，就保留了
        Assert.That(json, Is.EqualTo("{\"data\":{\"Score\":99,\"Name\":\"Alice\"}}"));
        Assert.That(loaded.Score, Is.EqualTo(original.Score));
        Assert.That(loaded.Name, Is.EqualTo(original.Name));
    }
}
