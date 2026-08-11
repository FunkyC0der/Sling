namespace Sling.Levels
{
  public readonly struct LevelAddress
  {
    public readonly int WorldIndex;
    public readonly int LevelIndex;

    public LevelAddress(int worldIndex, int levelIndex)
    {
      WorldIndex = worldIndex;
      LevelIndex = levelIndex;
    }
  }
}
