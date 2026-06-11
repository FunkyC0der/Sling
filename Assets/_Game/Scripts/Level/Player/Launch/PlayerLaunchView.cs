using System;
using Sling.Common.Views;
using Sling.Level.Common;
using UnityEngine;

namespace Sling.Level.Player.Launch
{
  public class PlayerLaunchView : MonoBehaviour, ILaunchable, IGameObjectView
  {
    public delegate Vector3 SamplePositionFunc(float t);
    
    public event Action OnLaunched;
    
    [SerializeField] private int _hintTrajectoryFrameRate = 24;

    private Rigidbody2D _rb;
    private LineRenderer _line;
    
    private void Awake()
    {
      _rb = GetComponent<Rigidbody2D>();
      _line = GetComponent<LineRenderer>();
    }
    
    public void Launch(Vector2 velocity)
    {
      _rb.linearVelocity = velocity;
      OnLaunched?.Invoke();
    }
    
    public void ShowHint(float totalTime, SamplePositionFunc samplePosition)
    {
      int pointCount = Mathf.RoundToInt(totalTime * _hintTrajectoryFrameRate);
      float timeStep = 1f / _hintTrajectoryFrameRate;

      _line.positionCount = pointCount;
      for (int i = 0; i < pointCount; i++)
        _line.SetPosition(i, samplePosition(i * timeStep));

      _line.enabled = true;
    }

    public void HideHint() =>
      _line.enabled = false;
  }
}