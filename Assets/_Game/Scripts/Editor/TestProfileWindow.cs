using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sling.Infrastructure;
using Sling.Infrastructure.Progress;
using UnityEditor;
using UnityEngine;

namespace Sling.Editor
{
  public class TestProfileWindow : EditorWindow
  {
    private const string _kCreateNewProfileLabel = "Create New...";
    private const string _kValidProfilePattern = "^[a-zA-Z0-9_-]{1,30}$";

    private string _profile;
    private bool _isCreatingNewProfile;

    [MenuItem("Tools/Sling/Test Profile")]
    public static void ShowWindow()
    {
      var window = GetWindow<TestProfileWindow>();
      window.titleContent = new GUIContent("Test Profile");
      window.minSize = new Vector2(420f, 250f);
      window.Show();
    }

    private void OnEnable()
    {
      _profile = GetSelectedProfile();
      EditorProfilePlayerProgressStorageDecorator.RegisterProfile(_profile);
    }

    private void OnGUI()
    {
      bool isPlaying = EditorApplication.isPlayingOrWillChangePlaymode;

      EditorGUILayout.Space();
      bool isValidProfile = DrawAuthenticationSection(isPlaying);
      EditorGUILayout.Space();
      DrawLocalSaveSection(isPlaying, isValidProfile);
    }

    private bool DrawAuthenticationSection(bool isPlaying)
    {
      EditorGUILayout.LabelField("Authentication Profile", EditorStyles.boldLabel);
      EditorGUILayout.HelpBox(
        "The selected profile is applied the next time Play Mode starts.",
        MessageType.Info);

      bool isValidProfile;
      using (new EditorGUI.DisabledScope(isPlaying))
      {
        IReadOnlyList<string> profiles = EditorProfilePlayerProgressStorageDecorator.GetProfiles();
        DrawProfileSelector(profiles);
        isValidProfile = IsValidProfile(_profile);

        if (!isValidProfile)
        {
          EditorGUILayout.HelpBox(
            "Use 1-30 letters, digits, hyphens, or underscores.",
            MessageType.Error);
        }

        bool isDefaultProfile =
          _profile == InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile;
        bool canDeleteProfile = isValidProfile && !_isCreatingNewProfile && !isDefaultProfile;
        using (new EditorGUI.DisabledScope(!canDeleteProfile))
        {
          if (GUILayout.Button("Delete Profile"))
            DeleteProfile();
        }
      }

      if (isPlaying)
        EditorGUILayout.HelpBox("Exit Play Mode to change the profile.", MessageType.Warning);

      return isValidProfile;
    }

    private void DrawProfileSelector(IReadOnlyList<string> profiles)
    {
      int currentProfileIndex = IndexOf(profiles, _profile);
      int createNewIndex = profiles.Count;
      if (currentProfileIndex < 0)
        _isCreatingNewProfile = true;

      var options = new string[profiles.Count + 1];
      for (int profileIndex = 0; profileIndex < profiles.Count; profileIndex++)
        options[profileIndex] = profiles[profileIndex];
      options[createNewIndex] = _kCreateNewProfileLabel;

      int selectedIndex = _isCreatingNewProfile ? createNewIndex : currentProfileIndex;
      int newSelectedIndex = EditorGUILayout.Popup("Profile", selectedIndex, options);
      if (newSelectedIndex != selectedIndex)
      {
        _isCreatingNewProfile = newSelectedIndex == createNewIndex;
        _profile = _isCreatingNewProfile ? string.Empty : profiles[newSelectedIndex];
        if (!_isCreatingNewProfile)
          SelectProfile();
      }

      if (_isCreatingNewProfile)
      {
        EditorGUI.BeginChangeCheck();
        string profile = EditorGUILayout.DelayedTextField("New Profile", _profile);
        if (EditorGUI.EndChangeCheck())
        {
          _profile = profile;
          if (IsValidProfile(_profile))
            SelectProfile();
        }
      }
    }

    private void DrawLocalSaveSection(bool isPlaying, bool isValidProfile)
    {
      EditorGUILayout.LabelField("Local Save", EditorStyles.boldLabel);
      EditorGUILayout.HelpBox(
        "Each authentication profile uses an isolated local progress save.",
        MessageType.Info);

      if (!isValidProfile)
      {
        EditorGUILayout.LabelField("Status", "Enter a valid profile");
        return;
      }

      string progressKey = EditorProfilePlayerProgressStorageDecorator.GetProgressKey(_profile);
      bool hasSave = EditorProfilePlayerProgressStorageDecorator.HasSave(_profile);

      EditorGUILayout.LabelField("PlayerPrefs Key", progressKey);
      EditorGUILayout.LabelField("Status", hasSave ? "Save exists" : "No save");

      using (new EditorGUI.DisabledScope(isPlaying || !hasSave))
      {
        if (GUILayout.Button("Delete Local Save"))
          DeleteLocalSave();
      }

      if (isPlaying)
      {
        EditorGUILayout.HelpBox(
          "Exit Play Mode before deleting a save to avoid recreating it from in-memory progress.",
          MessageType.Warning);
      }
    }

    private static bool IsValidProfile(string profile) =>
      !string.IsNullOrEmpty(profile) && Regex.IsMatch(profile, _kValidProfilePattern);

    private static string GetSelectedProfile() =>
      EditorPrefs.GetString(
        InitUnityServicesFlowController.kEditorAuthenticationProfilePrefsKey,
        InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile);

    private void SelectProfile()
    {
      EditorProfilePlayerProgressStorageDecorator.RegisterProfile(_profile);
      EditorPrefs.SetString(
        InitUnityServicesFlowController.kEditorAuthenticationProfilePrefsKey,
        _profile);
      _isCreatingNewProfile = false;
      Debug.Log($"Test profile selected: '{_profile}'.");
    }

    private void DeleteLocalSave()
    {
      bool confirmed = EditorUtility.DisplayDialog(
        "Delete Local Save",
        $"Delete local progress for profile '{_profile}'?",
        "Delete",
        "Cancel");
      if (!confirmed)
        return;

      EditorProfilePlayerProgressStorageDecorator.DeleteSave(_profile);
      Debug.Log($"Deleted local save for profile '{_profile}'.");
    }

    private void DeleteProfile()
    {
      bool confirmed = EditorUtility.DisplayDialog(
        "Delete Profile",
        $"Remove profile '{_profile}' from the list? Its local save will not be deleted.",
        "Delete",
        "Cancel");
      if (!confirmed)
        return;

      string deletedProfile = _profile;
      EditorProfilePlayerProgressStorageDecorator.DeleteProfile(deletedProfile);
      _profile = InitUnityServicesFlowController.kEditorAuthenticationDefaultProfile;
      SelectProfile();
      Debug.Log($"Deleted test profile '{deletedProfile}'.");
    }

    private static int IndexOf(IReadOnlyList<string> profiles, string profile)
    {
      for (int profileIndex = 0; profileIndex < profiles.Count; profileIndex++)
      {
        if (profiles[profileIndex] == profile)
          return profileIndex;
      }

      return -1;
    }
  }
}
