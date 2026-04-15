using System;
using UnityEngine;

internal class JsonSerializer : ISerializer
{
    public T Deserialize<T>(string data)
    {
        // null会返回null
        try
        {
            return JsonUtility.FromJson<Wrapper<T>>(data).data;
        } catch (Exception e)
        {
            throw new InvalidOperationException("Deserialize failed", e);
        }
    }

    public string Serialize<T>(T data)
    {
        // null会返回空字符串
        try
        {
            return JsonUtility.ToJson(new Wrapper<T>(data));
        } catch (Exception e)
        {
            throw new InvalidOperationException("Serialize failed", e);
        }
    }

    // JsonUtility不能直接序列化int、float这类基础类型以及数组之类的
    // 需要装在Wrapper中再序列化
    // 为了统一格式，干脆所有数据都直接装到Wrapper中
    [Serializable]
    private class Wrapper<T>
    {
        public T data;
        public Wrapper(T data) {
            this.data = data;
        }
    }
}