using System;
using Sling.Core;
using UnityEngine;

namespace Sling.Level.StickyWall
{
  public class StickyWallView : BaseView
  {
    [field: SerializeField] public StickyWallConfig Config { get; private set; }

    public Action<StickyWallView> OnPlayerEnter;
    public Action<StickyWallView> OnPlayerExit;

    private void OnCollisionEnter2D()
    {
      OnPlayerEnter?.Invoke(this);
    }

    private void OnCollisionExit2D()
    {
      OnPlayerExit?.Invoke(this);
    }
  }
}