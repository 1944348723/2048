using System;
using UnityEngine;

internal static class Interpolator
{
    public static T Lerp<T>(T start, T end, float t)
    {
        if (typeof(T) == typeof(float))
        {
            return (T)(object)Lerp((float)(object)start, (float)(object)end, t);
        }
        else if (typeof(T) == typeof(int))
        {
            return (T)(object)Lerp((int)(object)start, (int)(object)end, t);
        }
        else if (typeof(T) == typeof(Vector3))
        {
            return (T)(object)Lerp((Vector3)(object)start, (Vector3)(object)end, t);
        }
        else
        {
            throw new NotSupportedException($"Lerp does not support type {typeof(T)}");
        }
    }

    public static float Lerp(float start, float end, float t)
    {
        return start + (end - start) * t;
    }

    public static int Lerp(int start, int end, float t)
    {
        return (int)(start + (end - start) * t);
    }

    public static Vector3 Lerp(Vector3 start, Vector3 end, float t)
    {
        return start + (end - start) * t;
    }
}