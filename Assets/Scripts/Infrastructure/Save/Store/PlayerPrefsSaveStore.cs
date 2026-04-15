using UnityEngine;

internal class PlayerPrefsSaveStore : ISaveStore
{
    private const string DEFAULT_FILE_NAME = "MyES3";
    private readonly ISerializer serializer;

    public PlayerPrefsSaveStore(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public T Load<T>(string key, string file = null)
    {
        string fullKey = GetFullKey(key, file);
        if (!PlayerPrefs.HasKey(fullKey))
        {
            return default;
        }
        string data = PlayerPrefs.GetString(fullKey);
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }
        return serializer.Deserialize<T>(data);
    }

    public void Save<T>(string key, T data, string file = null)
    {
        PlayerPrefs.SetString(GetFullKey(key, file), serializer.Serialize<T>(data));
    }

    public bool HasKey(string key, string file = null)
    {
        return PlayerPrefs.HasKey(GetFullKey(key, file));
    }

    private string GetFullKey(string key, string file = null)
    {
        return (file ?? DEFAULT_FILE_NAME) + "." + key;
    }
}