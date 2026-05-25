using System.Collections.Generic;

/*
    TweenMagager只负责更新推进Tweener，回调由Tweener自行处理

    1.Tick遍历过程中检查是否完成并删除的话会有问题，所以使用pendingRemove来暂存，结束后统一删除
    对于这个问题也可以使用倒序遍历或swap-move(要删除的元素和末尾元素swap，然后删除末尾元素)来解决
    2.Tick过程中的Updated和Completed之类的回调也可能导致Add、Remove被调用，所以也需要在Tick过程中暂存，结束后统一调用
*/
internal static class TweenManager
{
    private static readonly List<TweenBase> tweeners = new();
    private static readonly List<TweenBase> pendingAdd = new();
    private static readonly List<TweenBase> pendingRemove = new();

    private static bool isTicking = false;

    public static void Add(TweenBase tweener)
    {
        if (tweener == null) return;

        if (isTicking) pendingAdd.Add(tweener);
        else tweeners.Add(tweener);
    }

    public static void Remove(TweenBase tweener) {
        if (tweener == null) return;

        if (isTicking) pendingRemove.Add(tweener);
        else tweeners.Remove(tweener);
    }

    public static void Tick(float dt) {
        isTicking = true;

        for (int i = 0; i < tweeners.Count; ++i)
        {
            var tweener = tweeners[i];
            if (tweener == null || tweener.IsKilled)
            {
                pendingRemove.Add(tweener);
                continue;
            }

            tweener.Tick(dt);

            if (tweener.IsCompleted || tweener.IsKilled)
            {
                pendingRemove.Add(tweener);
            }
        }

        isTicking = false;

        for (int i = 0; i < pendingRemove.Count; ++i)
        {
            tweeners.Remove(pendingRemove[i]);
        }
        pendingRemove.Clear();

        tweeners.AddRange(pendingAdd);
        pendingAdd.Clear();
    }

    public static bool IsTweening()
    {
        return tweeners.Count > 0 || pendingAdd.Count > 0;
    }
}