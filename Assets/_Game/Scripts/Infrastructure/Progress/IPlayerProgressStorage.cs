namespace Sling.Infrastructure.Progress
{
  public interface IPlayerProgressStorage
  {
    PlayerProgressData Load();
    void Save(PlayerProgressData data);
  }
}
