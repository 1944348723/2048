using System;

public sealed class Tweener<T>: TweenBase
{
    private readonly Func<T> getter;
    private readonly Action<T> setter;
    private readonly T startValue;
    private readonly T endValue;
    private readonly float duration;

    private T currentValue;
    private EaseType easeType = EaseType.Linear;

    internal Tweener(Func<T> getter, Action<T> setter, T endValue, float duration)
    {
        this.getter = getter;
        this.setter = setter;
        this.startValue = getter();
        this.currentValue = startValue;
        this.endValue = endValue;
        this.duration = duration;
    }

    internal override void Tick(float dt)
    {
        if (IsCompleted || IsKilled) return;

        Elapsed += dt;
        if (Elapsed >= duration) {
            currentValue = endValue;
        } else
        {
            float t = Elapsed / duration;
            float tEased = Ease.Calculate(t, easeType);

            // 目前默认且只支持线性插值
            // TODO: 支持更多插值方式
            currentValue = Interpolator.Lerp(startValue, endValue, tEased);
        }

        setter(currentValue);
        Updated?.Invoke(dt);

        if (Elapsed >= duration)
        {
            IsCompleted = true;
            Completed?.Invoke();
        }
    }

    public void SetEase(EaseType easeType)
    {
        this.easeType = easeType;
    }
}