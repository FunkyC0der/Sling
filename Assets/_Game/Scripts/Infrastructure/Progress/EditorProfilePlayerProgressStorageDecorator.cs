#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Sling.Infrastructure.Progress
{
  public class EditorProfilePlayerProgressStorageDecorator : IPlayerProgressStorage
  {
    private const string _kDefaultProgressKey = "Sling.Progress";
    private const string _kProfilesKey = "Sling.Editor.LocalProfiles";

    private readonly PlayerPrefsPlayerProgressStorage _storage;

    public EditorProfilePlayerProgressStorageDecorator(PlayerPrefsPlayerProgressStorage storage)
    {
      string profile = GetSelectedProfile();
      RegisterProfile(profile);

      _storage = storage;
      _storage.SetProfile(
        profile == InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile
          ? null
          : profile);
    }

    public PlayerProgressData Load() =>
      _storage.Load();

    public void Save(PlayerProgressData data) =>
      _storage.Save(data);

    public static string GetProgressKey(string profile)
    {
      if (string.IsNullOrEmpty(profile) ||
          profile == InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile)
      {
        return _kDefaultProgressKey;
      }

      return $"{_kDefaultProgressKey}.{profile}";
    }

    public static bool HasSave(string profile) =>
      PlayerPrefs.HasKey(GetProgressKey(profile));

    public static IReadOnlyList<string> GetProfiles()
    {
      List<string> profiles = LoadProfiles();
      string defaultProfile = InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile;
      if (!profiles.Contains(defaultProfile))
        profiles.Add(defaultProfile);

      profiles.Sort(StringComparer.Ordinal);
      return profiles;
    }

    public static void RegisterProfile(string profile)
    {
      string normalizedProfile = GetNormalizedProfile(profile);
      List<string> profiles = LoadProfiles();
      if (profiles.Contains(normalizedProfile))
        return;

      profiles.Add(normalizedProfile);
      profiles.Sort(StringComparer.Ordinal);
      EditorPrefs.SetString(_kProfilesKey, JsonConvert.SerializeObject(profiles));
    }

    public static void DeleteProfile(string profile)
    {
      string normalizedProfile = GetNormalizedProfile(profile);
      if (normalizedProfile == InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile)
        return;

      List<string> profiles = LoadProfiles();
      if (!profiles.Remove(normalizedProfile))
        return;

      EditorPrefs.SetString(_kProfilesKey, JsonConvert.SerializeObject(profiles));
    }

    public static void DeleteSave(string profile)
    {
      PlayerPrefs.DeleteKey(GetProgressKey(profile));
      PlayerPrefs.Save();
    }

    private static List<string> LoadProfiles()
    {
      if (!EditorPrefs.HasKey(_kProfilesKey))
        return new List<string>();

      return DeserializeProfiles(EditorPrefs.GetString(_kProfilesKey));
    }

    private static List<string> DeserializeProfiles(string json)
    {
      try
      {
        return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
      }
      catch (JsonException exception)
      {
        Debug.LogWarning($"Failed to deserialize local profiles. Creating a new profile list. {exception}");
        return new List<string>();
      }
    }

    private static string GetNormalizedProfile(string profile) =>
      string.IsNullOrEmpty(profile)
        ? InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile
        : profile;

    private static string GetSelectedProfile() =>
      EditorPrefs.GetString(
        InitUnityServicesFlowController.kEditorAuthenticationProfilePrefsKey,
        InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile);
  }
}
#endif
