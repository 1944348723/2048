using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

internal abstract class AbstractSaveStore : ISaveStore
{
    protected ISerializer serializer;

    public AbstractSaveStore(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public T Load<T>(string key, string file = null)
    {
        Assert.IsNotNull(key);

        SaveFileData data = ReadContainer(file);
        SaveEntry entry = FindEntry(data, key);
        if (entry == null) {
            throw new KeyNotFoundException(string.Format("Key {0} not found", key));
        }
        try
        {
            return serializer.Deserialize<T>(entry.Value);
        } catch (Exception e)
        {
            throw new InvalidOperationException("Deserialize value failed.", e);
        }
    }

    public T Load<T>(string key, T defaultValue, string file = null)
    {
        Assert.IsNotNull(key);
        
        if (!ContainerExists(file))
        {
            return defaultValue;
        }
        SaveFileData data = ReadContainer(file);

        SaveEntry entry = FindEntry(data, key);
        if (entry == null) {
            return defaultValue;
        }

        try
        {
            return serializer.Deserialize<T>(entry.Value);
        } catch (Exception e)
        {
            throw new InvalidOperationException("Deserialize value failed.", e);
        }
    }

    public void Save<T>(string key, T data, string file = null)
    {
        Assert.IsNotNull(key);
        Assert.IsTrue(data != null);

        SaveFileData fileData;
        if (!ContainerExists(file)) {
            fileData = new();
        } else {
            fileData = ReadContainer(file);
        }

        SaveEntry entry = FindEntry(fileData, key);
        if (entry == null)
        {
            entry = new SaveEntry
            {
                Key = key
            };
            fileData.Entries.Add(entry);
        }

        try
        {
            entry.Value = serializer.Serialize<T>(data);
        } catch(Exception e)
        {
            throw new InvalidOperationException("Serialize value failed.", e);
        }
        WriteContainer(file, fileData);
    }

    public abstract bool HasKey(string key, string file = null);
    protected abstract bool ContainerExists(string file);
    protected abstract SaveFileData ReadContainer(string file);
    protected abstract void WriteContainer(string file, SaveFileData fileData);

    protected static SaveEntry FindEntry(SaveFileData data, string key)
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
}