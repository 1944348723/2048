using System;
using System.IO;
using UnityEngine;

internal class PlayerPrefsSaveStore : AbstractSaveStore
{
    private const string DEFAULT_FILE_NAME = "MyES3";

    public PlayerPrefsSaveStore(ISerializer serializer) : base(serializer) {}

    public override bool HasKey(string key, string file = null)
    {
        if (!ContainerExists(file))
        {
            return false;
        }

        SaveFileData fileData = ReadContainer(file);
        return FindEntry(fileData, key) != null;
    }

    protected override SaveFileData ReadContainer(string file)
    {
        string fileName = GetFileName(file);
        if (!PlayerPrefs.HasKey(fileName))
        {
            throw new FileNotFoundException("Ensure the file exists.", fileName);
        }
        
        string json = PlayerPrefs.GetString(fileName);
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
        string fileName = GetFileName(file);
        return PlayerPrefs.HasKey(fileName);
    }

    protected override void WriteContainer(string file, SaveFileData fileData)
    {
        string fileName = GetFileName(file);
        string json;
        try
        {
            json = JsonUtility.ToJson(fileData);
        } catch (Exception e)
        {
            throw new InvalidOperationException("Serialize file data failed.", e);
        }

        PlayerPrefs.SetString(fileName, json);
    }

    private string GetFileName(string file)
    {
        return file ?? DEFAULT_FILE_NAME;
    }
}