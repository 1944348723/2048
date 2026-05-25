public sealed class DelayTween: TweenBase
{
    private readonly float duration;

    internal DelayTween(float duration)
    {
        this.duration = duration;
    }

    internal override void Tick(float dt)
    {
        if (IsCompleted || IsKilled) return;

        Elapsed += dt;
        Updated?.Invoke(dt);

        if (Elapsed >= duration)
        {
            IsCompleted = true;
            Completed?.Invoke();
        }
    }
}