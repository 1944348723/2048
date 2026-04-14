using System;

public static class EasySave
{
    private static SaveLocation location;
    private static ISaveStore saveStore;

    private static bool initialied = false;

    public static void Save<T>(string key, T data) {
        EnsureInitialized();
        saveStore.Save<T>(key, data);
    }

    public static T Load<T>(string key)
    {
        EnsureInitialized();
        return saveStore.Load<T>(key);
    }

    internal static void Init(SaveLocation location)
    {
        initialied = true;
        EasySave.location = location;

        if (location == SaveLocation.PlayerPrefs)
        {
            saveStore = new PlayerPrefsSaveStore(new JsonSerializer());
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