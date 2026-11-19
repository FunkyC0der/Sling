namespace Sling.Common.Rendering
{
  // Shared state between pop-up UI and BackdropBlurFeature (a renderer asset cannot be injected).
  public static class BackdropBlur
  {
    private static float _intensity;

    // 0 - no blur (the render pass is skipped), 1 - fully blurred scene.
    public static float Intensity
    {
      get => _intensity;
      set => _intensity = value < 0f ? 0f : value > 1f ? 1f : value;
    }
  }
}
