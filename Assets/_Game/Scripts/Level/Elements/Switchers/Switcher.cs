using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sling.Common.Collission;
using Sling.Common.Views;
using UnityEngine;


namespace Sling.Level.Elements.Switchers
{
  public class Switcher : MonoBehaviour, IViewListItem
  {
    [SerializeField] private TriggerZone _interactZone;
    [SerializeReference] private List<SwitcherStateChangeStrategy> _stateChangeStrategies = new();
    [SerializeField] private bool _isOnByDefault;
    [SerializeField] private bool _endlessSwitch;

    private Coroutine _setStateCoroutine;
    private bool _currentState;
    private bool _hasCurrentState;

    public TriggerZone InteractZone => _interactZone;
    public bool IsOnByDefault => _isOnByDefault;
    public bool EndlessSwitch => _endlessSwitch;

    private void OnDestroy()
    {
      StopStateChange();
    }

    public void SetState(bool isOn, bool immediate)
    {
      StopStateChange();

      _currentState = isOn;
      _hasCurrentState = true;
      _setStateCoroutine = StartCoroutine(SetStateCoroutine(isOn, immediate));
    }

    private void StopStateChange()
    {
      if (_setStateCoroutine != null)
      {
        StopCoroutine(_setStateCoroutine);
        _setStateCoroutine = null;
      }

      foreach (SwitcherStateChangeStrategy strategy in _stateChangeStrategies)
        strategy?.Stop();
    }

    [Button("Debug Switch")]
    private void DebugSwitch()
    {
      if (!Application.isPlaying)
        return;

      bool currentState = _hasCurrentState ? _currentState : _isOnByDefault;
      SetState(!currentState, immediate: false);
    }

    private IEnumerator SetStateCoroutine(bool isOn, bool immediate)
    {
      foreach (SwitcherStateChangeStrategy strategy in _stateChangeStrategies)
      {
        if (strategy != null)
          yield return strategy.SetState(isOn, immediate);
      }

      _setStateCoroutine = null;
    }
  }
}
