using Sling.Common;

namespace Sling.Settings
{
  public class SettingsModel
  {
    public const int kMaxVolume = 10;

    public readonly Observable<int> MusicVolume = new();
    public readonly Observable<int> SfxVolume = new();
    public readonly Observable<bool> IsCursorVisible = new();

    public static float ToVolumeMultiplier(int volume) =>
      (float)volume / kMaxVolume;
  }
}
