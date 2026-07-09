namespace Sling.Infrastructure.Progress
{
  public class PlayerProgressService
  {
    private readonly IPlayerProgressStorage _storage;
    private PlayerProgressData _data = new();

    public PlayerProgressService(IPlayerProgressStorage storage) => 
      _storage = storage;

    public void Load() =>
      _data = _storage.Load();

    public bool TryGetBestResult(string levelId, out LevelBestResult result) =>
      _data.LevelBestResults.TryGetValue(levelId, out result);

    public void SetBestResult(string levelId, LevelBestResult result) =>
      _data.LevelBestResults[levelId] = result;

    public void Save() =>
      _storage.Save(_data);
  }
}
