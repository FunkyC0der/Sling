namespace Sling.Level.Boss
{
  public class BossModel
  {
    public int CurrentPhaseIndex = -1;

    public bool IsFirstRun => CurrentPhaseIndex < 0;
  }
}
