using System;
using PrimeTween;

namespace Sling.MainMenu.MenuCamera
{
  [Serializable]
  public class MainMenuCameraConfig
  {
    public float TransitionDuration = 0.6f;
    public Ease TransitionEase = Ease.InOutCubic;
  }
}
