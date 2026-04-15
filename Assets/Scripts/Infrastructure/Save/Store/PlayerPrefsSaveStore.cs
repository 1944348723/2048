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
        string data = PlayerPrefs.GetString(key);
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }
        return serializer.Deserialize<T>(data);
    }

    public void Save<T>(string key, T data)
    {
        PlayerPrefs.SetString(key, serializer.Serialize<T>(data));
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}