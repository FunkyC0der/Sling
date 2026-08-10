using System.Collections;
using System.Collections.Generic;
using Sling.Common.Collission;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class Switcher : MonoBehaviour
  {
    [SerializeField] private TriggerZone _interactZone;
    [SerializeField] private List<SwitcherStateChangeStrategy> _stateChangeStrategies = new();
    [SerializeField] private bool _isOnByDefault;
    [SerializeField] private bool _endlessSwitch;

    private Coroutine _setStateCoroutine;
    private bool _isOn;
    private bool _isFirstSwitch = true;

    private void Awake()
    {
      _interactZone.OnEnter += OnEnter;

      _isOn = _isOnByDefault;
      SetState(_isOn, immediate: true);
    }

    private void OnDestroy()
    {
      _interactZone.OnEnter -= OnEnter;

      if (_setStateCoroutine != null)
        StopCoroutine(_setStateCoroutine);
    }

    private void OnEnter(Collider2D collider)
    {
      if (!_endlessSwitch && !_isFirstSwitch)
        return;

      _isFirstSwitch = false;
      _isOn = !_isOn;
      SetState(_isOn, immediate: false);
    }

    private void SetState(bool isOn, bool immediate)
    {
      if (_setStateCoroutine != null)
        StopCoroutine(_setStateCoroutine);

      _setStateCoroutine = StartCoroutine(SetStateCoroutine(isOn, immediate));
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
