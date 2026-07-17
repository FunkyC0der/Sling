using Newtonsoft.Json;
using UnityEngine;

namespace Sling.Infrastructure.Progress
{
  public class PlayerPrefsPlayerProgressStorage : IPlayerProgressStorage
  {
    private const string _kProgressKey = "Sling.Progress";

    private string _currentProgressKey = _kProgressKey;

    public void SetProfile(string profile)
    {
      _currentProgressKey = string.IsNullOrEmpty(profile)
        ? _kProgressKey
        : $"{_kProgressKey}.{profile}";
    }

    public PlayerProgressData Load()
    {
      if (!PlayerPrefs.HasKey(_currentProgressKey))
        return new PlayerProgressData();

      string json = PlayerPrefs.GetString(_currentProgressKey);
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
      PlayerPrefs.SetString(_currentProgressKey, json);
      PlayerPrefs.Save();
    }
  }
}
