using System;

public abstract class TweenBase {
    public bool IsCompleted { get; protected set;} = false;
    public bool IsKilled { get; protected set; } = false;
    public float Elapsed { get; protected set; } = 0f;
    public void OnUpdate(Action<float> callback) => Updated += callback;
    public void OnComplete(Action callback) => Completed += callback;
    public void OnKilled(Action callback) => Killed += callback;
    internal abstract void Tick(float dt);

    protected Action<float> Updated;
    protected Action Completed;
    protected Action Killed;
}