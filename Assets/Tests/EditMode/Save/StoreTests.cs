using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

// AI生成，review过，添加了注释
public class StoreTests
{
    private sealed class TestPayload
    {
        public string Key;
        public string FileName;
        public string FilePath;
        public string BackupPath;
    }

    // 测试步骤：SetUp(准备测试环境) -> Test(实际测试) -> TearDown(消除测试影响)
    // 因为测试的是存储系统，有数据持久化，会在本地留下记录，影响测试，所以有SetUp和TearDown
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
        DeleteTestFiles("json-store");
    }

    [Test]
    public void PlayerPrefsSaveStore_SaveAndLoad_UsesSingleContainerEntryPerFile()
    {
        TestPayload payload = CreatePayload("prefs-store");
        PlayerPrefsSaveStore store = new(new JsonSerializer());

        store.Save(payload.Key, 123, payload.FileName);

        int value = store.Load<int>(payload.Key, payload.FileName);
        string rawContainer = PlayerPrefs.GetString(payload.FileName);

        Assert.That(value, Is.EqualTo(123));
        Assert.That(PlayerPrefs.HasKey(payload.FileName), Is.True);
        Assert.That(rawContainer, Does.Contain("\"Key\":\"" + payload.Key + "\""));
        Assert.That(rawContainer, Does.Contain("\\\"data\\\":123"));
    }

    [Test]
    public void PlayerPrefsSaveStore_Load_WithDefaultValue_ReturnsDefaultWhenContainerMissing()
    {
        TestPayload payload = CreatePayload("prefs-default");
        PlayerPrefsSaveStore store = new(new JsonSerializer());

        int value = store.Load(payload.Key, 777, payload.FileName);

        Assert.That(value, Is.EqualTo(777));
    }

    [Test]
    public void JsonFileSaveStore_SaveAndLoad_RoundTripsDataAndCreatesBackupOnOverwrite()
    {
        TestPayload payload = CreatePayload("json-store");
        JsonFileSaveStore store = new(new JsonSerializer());

        store.Save(payload.Key, 10, payload.FileName);
        store.Save(payload.Key, 20, payload.FileName);

        int value = store.Load<int>(payload.Key, payload.FileName);
        string fileContents = File.ReadAllText(payload.FilePath);

        Assert.That(value, Is.EqualTo(20));
        Assert.That(File.Exists(payload.FilePath), Is.True);
        Assert.That(File.Exists(payload.BackupPath), Is.True);
        Assert.That(fileContents, Does.Contain("\"Key\":\"" + payload.Key + "\""));
        Assert.That(fileContents, Does.Contain("\\\"data\\\":20"));
    }

    [Test]
    public void JsonFileSaveStore_Load_ThrowsWhenFileMissing()
    {
        TestPayload payload = CreatePayload("json-store");
        JsonFileSaveStore store = new(new JsonSerializer());

        Assert.Throws<FileNotFoundException>(() => store.Load<int>(payload.Key, payload.FileName));
    }

    [Test]
    public void JsonFileSaveStore_Load_WithDefaultValue_ReturnsDefaultWhenKeyMissing()
    {
        TestPayload payload = CreatePayload("json-store");
        JsonFileSaveStore store = new(new JsonSerializer());
        store.Save("other-key", 5, payload.FileName);

        int value = store.Load(payload.Key, 55, payload.FileName);

        Assert.That(value, Is.EqualTo(55));
    }

    [Test]
    public void JsonFileSaveStore_HasKey_ThrowsWhenContainerCorrupted()
    {
        TestPayload payload = CreatePayload("json-store");
        Directory.CreateDirectory(Path.GetDirectoryName(payload.FilePath));
        File.WriteAllText(payload.FilePath, "{bad json");
        JsonFileSaveStore store = new(new JsonSerializer());

        Assert.Throws<InvalidOperationException>(() => store.HasKey(payload.Key, payload.FileName));
    }

    // prefix用来区分测试目标
    private static TestPayload CreatePayload(string prefix)
    {
        // "N"表示转成没有横杠的字符串，如8f4a2a7d4f7d4f4d9c0c6e8b12345678
        // 每次拿到的不一样，用来区分同prefix下的不同测试
        // TestPayload示例
        // {
        //      Key = "key-8f4a2a7d4f7d4f4d9c0c6e8b12345678",
        //      FileName = "json-store-8f4a2a7d4f7d4f4d9c0c6e8b12345678.json",
        //      FilePath = "C:/Users/xxx/AppData/LocalLow/Company/Product/json-store-8f4a2a7d4f7d4f4d9c0c6e8b12345678.json"
        //      BackupPath = FilePath + ".bak"
        // }
        string id = Guid.NewGuid().ToString("N");
        string fileName = $"{prefix}-{id}.json";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        return new TestPayload
        {
            Key = $"key-{id}",
            FileName = fileName,
            FilePath = filePath,
            BackupPath = filePath + ".bak"
        };
    }

    private static void DeleteTestFiles(string prefix)
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, $"{prefix}-*");
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
    }
}
