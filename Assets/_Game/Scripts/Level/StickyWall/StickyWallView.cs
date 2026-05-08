using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Level.StickyWall
{
  public class StickyWallView : MonoBehaviour, IView
  {
    [field: SerializeField] public StickyWallConfig Config { get; private set; }

    public Action<StickyWallConfig> OnPlayerEnter;
    public Action OnPlayerExit;

    private void OnCollisionEnter2D() => OnPlayerEnter?.Invoke(Config);
    private void OnCollisionExit2D() => OnPlayerExit?.Invoke();
  }
}
