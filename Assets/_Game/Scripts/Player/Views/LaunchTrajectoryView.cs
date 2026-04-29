using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Player.Views
{
  [RequireComponent(typeof(LineRenderer))]
  public class LaunchTrajectoryView : BaseView
  {
    [SerializeField] private int _pointCount = 30;
    [SerializeField] private float _timeStep = 0.05f;
    [SerializeField] private float _lineWidth = 0.1f;
    [SerializeField] private Gradient _colorGradient;

    private LineRenderer _line;

    private void Awake()
    {
      _line = GetComponent<LineRenderer>();
      
      _line.widthMultiplier = _lineWidth;
      _line.colorGradient = _colorGradient;

      Hide();
    }

    public void Show(Func<float, Vector3> samplePosition)
    {
      _line.positionCount = _pointCount;
      
      for (var i = 0; i < _pointCount; i++)
        _line.SetPosition(i, transform.position + samplePosition(i * _timeStep));
      
      _line.enabled = true;
    }

    public void Hide() => 
      _line.enabled = false;
  }
}