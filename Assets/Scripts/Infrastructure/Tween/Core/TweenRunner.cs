using UnityEngine;

internal sealed class TweenRunner: MonoBehaviour
{
    private void Update()
    {
        float dt = Time.deltaTime;
        DoTween.Tick(dt);
    }
}