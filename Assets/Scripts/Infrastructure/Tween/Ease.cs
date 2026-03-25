using System;

public enum EaseType {
    Linear
};

internal static class Ease {
    public static float Calculate(float t, EaseType easeType) {
        if (t < 0 || t > 1) throw new ArgumentOutOfRangeException(nameof(t), "t must be between 0 and 1");

        return easeType switch
        {
            EaseType.Linear => t,
            _ => throw new NotSupportedException($"Ease type {easeType} is not supported"),
        };
    }
}