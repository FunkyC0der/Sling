using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using Sling.Common.Tweeners;
using UnityEngine;

namespace Sling.Level.Boss
{
  [Serializable]
  public class MeshBlinkBossHitReaction : BossHitReaction
  {
    public List<MeshRenderer> _renderers = new();
    public Material _blinkMaterial;
    public string _blinkAmountProperty = "_BlinkAmount";
    public int _blinkCount = 2;
    public float _duration = 0.5f;
    public float _blinkAmount = 0.8f;

    private Sequence _sequence;
    private Material[] _originalMaterials;

    public override async UniTask Play(CancellationToken cancellationToken)
    {
      Stop();

      _originalMaterials = new Material[_renderers.Count];
      for (int i = 0; i < _renderers.Count; i++)
      {
        _originalMaterials[i] = _renderers[i].sharedMaterial;
        _renderers[i].sharedMaterial = _blinkMaterial;
      }

      try
      {
        _sequence = BlinkSequence.Create(_blinkMaterial, Shader.PropertyToID(_blinkAmountProperty),
          _blinkCount, _duration, _blinkAmount);
        await _sequence.ToUniTask(cancellationToken: cancellationToken);
      }
      finally
      {
        Restore();
      }
    }

    public override void Stop()
    {
      if (_sequence.isAlive)
        _sequence.Stop();

      Restore();
    }

    private void Restore()
    {
      if (_originalMaterials == null)
        return;

      for (int i = 0; i < _renderers.Count; i++)
        if (_renderers[i] != null)
          _renderers[i].sharedMaterial = _originalMaterials[i];

      _originalMaterials = null;
    }
  }
}
