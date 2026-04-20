using System;
using System.IO;
using UnityEngine;

internal class JsonFileSaveStore : AbstractSaveStore
{
    private const string DEFAULT_FILE_NAME = "MyES3.json";
    private readonly string directory = Application.persistentDataPath;

    public JsonFileSaveStore(ISerializer serializer): base(serializer) {}

    public override bool HasKey(string key, string file = null)
    {
        string filePath = GetFilePath(file);
        if (!File.Exists(filePath))
        {
            return false;
        }
        SaveFileData fileData = ReadContainer(file);
        return FindEntry(fileData, key) != null;
    }

    // 读数据时文件不存在就用默认数据
    protected override SaveFileData ReadContainer(string file)
    {
        string filePath = GetFilePath(file);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Ensure the file exists.", filePath);
        }

        string json = File.ReadAllText(filePath);
        try
        {
            return JsonUtility.FromJson<SaveFileData>(json) ?? new SaveFileData();
        } catch (Exception e)
        {
            throw new InvalidOperationException("Deserialize file data failed.", e);
        }
    }

    protected override bool ContainerExists(string file)
    {
        string filePath = GetFilePath(file);
        return File.Exists(filePath);
    }

    // 安全写入
    protected override void WriteContainer(string file, SaveFileData data)
    {
        string filePath = GetFilePath(file);
        Directory.CreateDirectory(directory);
        string json;
        try
        {
            json = JsonUtility.ToJson(data);
        } catch(Exception e)
        {
            throw new InvalidOperationException("Serialize file data failed.", e);
        }
        
        string tmp = filePath + ".tmp";
        File.WriteAllText(tmp, json);
        if (File.Exists(filePath))
        {
            // 先备份原文件，然后再将其替换
            string backup = filePath + ".bak";
            File.Replace(tmp, filePath, backup);
        } else
        {
            // 类似linux的命令mov，可以用来改文件名
            File.Move(tmp, filePath);
        }
    }

    private string GetFilePath(string file = null)
    {
        return directory + '/' + (file ?? DEFAULT_FILE_NAME);
    }
}