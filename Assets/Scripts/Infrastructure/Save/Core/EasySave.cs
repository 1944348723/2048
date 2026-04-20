using System;
using System.IO;

public static class EasySave
{
    private static ISaveStore saveStore;

    private static bool initialied = false;

    public static void Save<T>(string key, T data, string file = null) {
        if (key == null || data == null)
        {
            throw new ArgumentNullException();
        }
        EnsureInitialized();
        saveStore.Save<T>(key, data, file);
    }

    /// <summary>
    /// 严格读取，文件不存在、key不存在、反序列化失败都抛异常
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public static T Load<T>(string key, string file = null)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        EnsureInitialized();
        return saveStore.Load<T>(key, file);
    }

    /// <summary>
    /// 宽松读取，文件不存在、key不存在返回默认值，反序列化失败抛异常
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Load<T>(string key, T defaultValue, string file = null)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        EnsureInitialized();
        return saveStore.Load<T>(key, defaultValue, file);
    }

    // 文件不存在也返回false
    // 反序列化失败会抛异常
    public static bool HasKey(string key, string file = null)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        EnsureInitialized();
        return saveStore.HasKey(key, file);
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