using System;

public static class DOVirtual
{
    public static DelayTween DelayedCall(float delay, Action callback)
    {
        var tween = new DelayTween(delay);
        tween.OnComplete(callback);
        TweenManager.Add(tween);
        return tween;
    }
}