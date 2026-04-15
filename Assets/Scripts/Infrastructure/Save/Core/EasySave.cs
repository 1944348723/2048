using System;

public static class EasySave
{
    private static ISaveStore saveStore;

    private static bool initialied = false;

    public static void Save<T>(string key, T data) {
        if (key == null || data == null)
        {
            throw new ArgumentNullException();
        }
        EnsureInitialized();
        saveStore.Save<T>(key, data);
    }

    public static T Load<T>(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        EnsureInitialized();
        return saveStore.Load<T>(key);
    }

    public static bool HasKey(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        EnsureInitialized();
        return saveStore.HasKey(key);
    }

    internal static void Init(SaveLocation location)
    {
        initialied = true;

        if (location == SaveLocation.PlayerPrefs)
        {
            saveStore = new PlayerPrefsSaveStore(new JsonSerializer());
        } else if (location == SaveLocation.File)
        {
            saveStore = new JsonFileSaveStore(new JsonSerializer());
        }
    }

    private static void EnsureInitialized()
    {
        if (!initialied)
        {
            throw new InvalidOperationException("EasySave not initialized until Awake() ended");
        }
    }
}