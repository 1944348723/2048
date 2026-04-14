using System;
using UnityEngine;

internal class PlayerPrefsSaveStore : ISaveStore
{
    private readonly ISerializer serializer;

    public PlayerPrefsSaveStore(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public T Load<T>(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return default;
        }

        Type type = typeof(T);

        if (type == typeof(int))
        {
            return (T)(object)PlayerPrefs.GetInt(key);
        }
        if (type == typeof(float))
        {
            return (T)(object)PlayerPrefs.GetFloat(key);
        }
        if (type == typeof(string))
        {
            return (T)(object)PlayerPrefs.GetString(key);
        }
        if (type == typeof(bool))
        {
            return (T)(object)(PlayerPrefs.GetInt(key) != 0);
        }

        return serializer.Deserialize<T>(PlayerPrefs.GetString(key));
    }

    public void Save<T>(string key, T data)
    {
        Type type = typeof(T);
        if (type == typeof(int))
        {
            PlayerPrefs.SetInt(key, (int)(object)data);
            return;
        }
        if (type == typeof(float))
        {
            PlayerPrefs.SetFloat(key, (float)(object)data);
            return;
        }
        if (type == typeof(string))
        {
            PlayerPrefs.SetString(key, (string)(object)data ?? string.Empty);
            return;
        }
        if (type == typeof(bool))
        {
            PlayerPrefs.SetInt(key, ((bool)(object)data) ? 1 : 0);
            return;
        }

        PlayerPrefs.SetString(key, serializer.Serialize<T>(data));
    }
}