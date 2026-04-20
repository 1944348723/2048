internal interface ISaveStore
{
    void Save<T>(string key, T data, string file = null);
    T Load<T>(string key, string file = null);
    T Load<T>(string key, T defaultValue, string file = null);
    bool HasKey(string key, string file = null);
}