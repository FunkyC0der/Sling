using Newtonsoft.Json;
using UnityEngine;

namespace Sling.Infrastructure.Progress
{
  public class PlayerPrefsPlayerProgressStorage : IPlayerProgressStorage
  {
    private const string _kProgressKey = "Sling.Progress";

    public PlayerProgressData Load()
    {
      if (!PlayerPrefs.HasKey(_kProgressKey))
        return new PlayerProgressData();

      string json = PlayerPrefs.GetString(_kProgressKey);
      PlayerProgressData data;
      try
      {
        data = JsonConvert.DeserializeObject<PlayerProgressData>(json);
      }
      catch (JsonException exception)
      {
        Debug.LogWarning($"Failed to deserialize player progress. Creating new progress data. {exception}");
        data = new PlayerProgressData();
      }

      data ??= new PlayerProgressData();
      data.EnsureInitialized();

      return data;
    }

    public void Save(PlayerProgressData data)
    {
      string json = JsonConvert.SerializeObject(data);
      PlayerPrefs.SetString(_kProgressKey, json);
      PlayerPrefs.Save();
    }
  }
}
