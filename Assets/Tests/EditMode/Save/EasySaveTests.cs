using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// AI生成，review过
// 只是单纯让AI生成测试的话，生成的测试明显不完善
// 得要专门强调尽可能完善、覆盖率多高之类的
public class EasySaveTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void Load_WithDefaultValue_ReturnsDefaultWhenFileMissing()
    {
        string fileName = $"facade-{Guid.NewGuid():N}.json";
        string key = $"key-{Guid.NewGuid():N}";
        EasySave.Init(SaveLocation.File);

        int value = EasySave.Load(key, 888, fileName);

        Assert.That(value, Is.EqualTo(888));
    }

    [Test]
    public void Load_ThrowsWhenKeyMissingWithoutDefaultValue()
    {
        string fileName = $"facade-{Guid.NewGuid():N}.json";
        string key = $"key-{Guid.NewGuid():N}";
        EasySave.Init(SaveLocation.File);
        EasySave.Save("other-key", 1, fileName);

        Assert.Throws<KeyNotFoundException>(() => EasySave.Load<int>(key, fileName));
    }

    [Test]
    public void Save_ThrowsWhenDataIsNull()
    {
        EasySave.Init(SaveLocation.PlayerPrefs);

        Assert.Throws<ArgumentNullException>(() => EasySave.Save<string>("null-key", null));
    }
}
