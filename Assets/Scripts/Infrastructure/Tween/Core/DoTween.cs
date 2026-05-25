using System;

public static class DoTween
{
    public static Tweener<T> To<T>(Func<T> getter, Action<T> setter, T endValue, float duration)
    {
        if (getter == null) throw new ArgumentNullException(nameof(getter));
        if (setter == null) throw new ArgumentNullException(nameof(setter));
        if (endValue == null) throw new ArgumentNullException(nameof(endValue));
        if (duration <= 0) throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be greater than zero.");

        var tweener = new Tweener<T>(getter, setter, endValue, duration);
        TweenManager.Add(tweener);

        return tweener;
    }

    public static bool IsTweening()
    {
        return TweenManager.IsTweening();
    }

    internal static void Tick(float dt)
    {
        if (dt < 0) throw new ArgumentOutOfRangeException(nameof(dt), "Delta time cannot be negative.");

        TweenManager.Tick(dt);
    }
}