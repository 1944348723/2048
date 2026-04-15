using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
internal class SaveEntry
{
    public string Key;
    public string Value;
}

[Serializable]
internal class SaveFileData
{
    public List<SaveEntry> Entries = new();
}

internal class JsonFileSaveStore : ISaveStore
{
    private const string DEFAULT_FILE_NAME = "save.json";
    private readonly string directory = Application.persistentDataPath;
    private readonly ISerializer serializer;

    public JsonFileSaveStore(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public T Load<T>(string key, string file = null)
    {
        string filePath = GetFilePath(file);
        SaveFileData data = ReadFile(filePath);
        SaveEntry entry = FindEntry(data, key);
        if (entry == null)
        {
            return default;
        }
        return serializer.Deserialize<T>(entry.Value);
    }

    public void Save<T>(string key, T data, string file = null)
    {
        string filePath = GetFilePath(file);
        SaveFileData fileData = ReadFile(filePath);
        SaveEntry entry = FindEntry(fileData, key);
        
        if (entry == null)
        {
            entry = new SaveEntry{ Key = key };
            fileData.Entries.Add(entry);
        }

        entry.Value = serializer.Serialize<T>(data);
        WriteFile(filePath, fileData);
    }

    public bool HasKey(string key, string file = null)
    {
        string filePath = GetFilePath(file);
        SaveFileData data = ReadFile(filePath);
        SaveEntry entry = FindEntry(data, key);
        return entry != null;
    }

    private SaveFileData ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new SaveFileData();
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SaveFileData>(json) ?? new SaveFileData();
    }

    private void WriteFile(string filePath, SaveFileData data)
    {
        Directory.CreateDirectory(directory);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    private SaveEntry FindEntry(SaveFileData data, string key)
    {
        int count = data.Entries.Count;
        for (int i = 0; i < count; ++i)
        {
            if (data.Entries[i].Key == key)
            {
                return data.Entries[i];
            }
        }
        return null;
    }

    private string GetFilePath(string file = null)
    {
        return directory + '/' + (file ?? DEFAULT_FILE_NAME);
    }
}