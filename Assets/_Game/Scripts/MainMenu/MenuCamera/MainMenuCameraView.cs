using System.Collections.Generic;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.MainMenu.MenuCamera
{
  public class MainMenuCameraView : MonoBehaviour, IUniqueView
  {
    public Transform _camera;
    public List<Transform> _worldAnchors = new();
    public List<GameObject> _worldEffects = new();

    private readonly Dictionary<int, ParticleSystem[]> _effectSystems = new();

    public Transform CameraTransform => _camera;

    public Vector3 GetAnchorPosition(int worldIndex)
    {
      Vector3 anchor = _worldAnchors[Mathf.Clamp(worldIndex, 0, _worldAnchors.Count - 1)].position;
      return new Vector3(anchor.x, anchor.y, _camera.position.z);
    }

    // pause instead of SetActive: re-enabling a GameObject restarts its particle systems
    public void SetEffectsVisible(int worldIndex, bool isVisible)
    {
      if (worldIndex < 0 || worldIndex >= _worldEffects.Count)
        return;

      foreach (ParticleSystem effect in GetEffectSystems(worldIndex))
      {
        effect.GetComponent<ParticleSystemRenderer>().enabled = isVisible;

        if (isVisible)
          effect.Play(false);
        else
          effect.Pause(false);
      }

      if (!isVisible)
        SetEffectScriptsEnabled(worldIndex, false);
    }

    public void ShowOnlyEffects(int worldIndex)
    {
      for (int i = 0; i < _worldEffects.Count; i++)
        SetEffectsVisible(i, i == worldIndex);

      SetEffectScriptsEnabled(worldIndex, true);
    }

    public void DisableAllEffectScripts()
    {
      for (int i = 0; i < _worldEffects.Count; i++)
        SetEffectScriptsEnabled(i, false);
    }

    // scripted effects (e.g. tumbleweed spawner) scan the camera view, so they run only once the camera has arrived
    private void SetEffectScriptsEnabled(int worldIndex, bool isEnabled)
    {
      if (worldIndex < 0 || worldIndex >= _worldEffects.Count)
        return;

      foreach (MonoBehaviour script in _worldEffects[worldIndex].GetComponentsInChildren<MonoBehaviour>(true))
        script.enabled = isEnabled;
    }

    private ParticleSystem[] GetEffectSystems(int worldIndex)
    {
      if (!_effectSystems.TryGetValue(worldIndex, out ParticleSystem[] systems))
      {
        systems = _worldEffects[worldIndex].GetComponentsInChildren<ParticleSystem>(true);
        _effectSystems.Add(worldIndex, systems);
      }

      return systems;
    }
  }
}
