using System;
using UnityEngine;

public class PlayerModel
{
    public Vector2 Velocity { get; private set; }
    public bool IsLaunched { get; private set; }
    public int LaunchCount { get; private set; }

    public event Action<Vector2> OnLaunched;

    public void SetLaunched(Vector2 force)
    {
        IsLaunched = true;
        Velocity = force;
        LaunchCount++;
        OnLaunched?.Invoke(force);
    }

    public void Reset()
    {
        Velocity = Vector2.zero;
        IsLaunched = false;
        LaunchCount = 0;
    }
}
