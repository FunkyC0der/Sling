using System;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Player
{
  [RequireComponent(typeof(LineRenderer))]
  public class LaunchTrajectoryView : MonoBehaviour, IGameObjectView
  {
    [SerializeField] private int _frameRate = 24;

    private LineRenderer _line;

    private void Awake()
    {
      _line = GetComponent<LineRenderer>();

      Hide();
    }

    public void Show(float totalTime, Func<float, Vector3> samplePosition)
    {
      int pointCount = Mathf.RoundToInt(totalTime * _frameRate);
      float timeStep = 1f / _frameRate;

      _line.positionCount = pointCount;
      for (int i = 0; i < pointCount; i++)
        _line.SetPosition(i, transform.position + samplePosition(i * timeStep));

      _line.enabled = true;
    }

    public void Hide() =>
      _line.enabled = false;
  }
}
