using System;
using System.Globalization;
using UnityEngine;

internal class JsonSerializer : ISerializer
{
    public T Deserialize<T>(string data)
    {
        Type type = typeof(T);
        if (type == typeof(int))
        {
            return (T)(object)int.Parse(data);
        }
        if (type == typeof(float))
        {
            return (T)(object)float.Parse(data, CultureInfo.InvariantCulture);
        }
        if (type == typeof(bool))
        {
            return (T)(object)(data == "1");
        }
        if (type == typeof(string))
        {
            return (T)(object)data;
        }

        return JsonUtility.FromJson<Wrapper<T>>(data).value;
    }

    public string Serialize<T>(T data)
    {
        Type type = typeof(T);

        if (type == typeof(int))
        {
            return ((int)(object)data).ToString();
        }
        if (type == typeof(float))
        {
            // 不同地区小数点可能不一样，需要固定写法，不受地区影响
            return ((float)(object)data).ToString(CultureInfo.InvariantCulture);
        }
        if (type == typeof(string))
        {
            return (string)(object)data ?? string.Empty;
        }
        if (type == typeof(bool))
        {
            return ((bool)(object)data) ? "1" : "0";
        }

        return JsonUtility.ToJson(new Wrapper<T>(data));
    }

    // 目前复杂类型通过在将其装在Wrapper中，然后加上[Serializable]来使用JsonUtility序列化
    // 这样有一定的限制，支持没那么完善，但是基本够用，主要是不用手动实现
    [Serializable]
    private class Wrapper<T>
    {
        public T value;
        public Wrapper(T value) {
            this.value = value;
        }
    }
}