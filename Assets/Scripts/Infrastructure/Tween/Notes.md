# 架构

# 碰到的问题或难点
## Tweener中Tick可见性和ITween冲突的问题
我一开始的设计是
```C#
internal interface ITween {
    void Tick(float dt);
    bool IsCompleted { get; }
    bool IsKilled { get; }
}
```

```C#
public sealed class Tweener<T>: ITween
{
    internal void Tick(float dt) {}
    public bool IsCompleted { get; private set; } = false;
    public bool IsKilled { get; private set; } = false;
}
```

`Tweener`是会暴露给用户的，所以我想着把`Tick`设置为`internal`，不让用户访问，更新权抓在我们自己手里。但是发现这跟实现接口冲突了，实现接口必须是`public`

我想着拆成`ITick`和`ITween`这类的两个接口，但是`TweenManager`中`Tick`、`IsCompleted`、`IsKilled`都用到了，意味着这三个必须在同一个接口中了。所以我干脆不用接口了，直接改成基类，这样就能将`Tick`设置成`internal`了

```C#
public abstract class TweenBase {
    public bool IsCompleted { get; protected set;} = false;
    public bool IsKilled { get; protected set; } = false;
    public float Elapsed { get; protected set; } = 0f;
    internal abstract void Tick(float dt);
}
```

## 插值和缓动
设`y`是某种抽象的值，已知起始值`y0`和最终值`y1`，再设`t'`表示从`y0`到`y1`的进度，0 <= t' <= 1。我们认为`y`随`t'`的变化满足规律`y = f(t)`，并从中取值，就是插值。

设`t`为时间进度(0 <= t <= 1)，`ease`就是时间进度`t`到实际进度`t'`的映射方式，`t' = ease(t)`，一般都有ease(0) = 0，ease(1) = 1。而导数`ease'(t)`就是实际进度随时间进度变化的速度。比如`Linear`，`t' = t`，实际进度就等于时间进度，`QuadIn`，`t' = ease(t) = t^2'，`ease'(t) = 2t`，所以时间进度越大，实际进度随时间进度变化的速度越快。


## CRTP解决链式调用问题