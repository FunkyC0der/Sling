using System.Collections;
using System.Collections.Generic;
using Sling.Common.Collission;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class Switcher : MonoBehaviour, IViewListItem
  {
    [SerializeField] private TriggerZone _interactZone;
    [SerializeField] private List<SwitcherStateChangeStrategy> _stateChangeStrategies = new();
    [SerializeField] private bool _isOnByDefault;
    [SerializeField] private bool _endlessSwitch;

    private Coroutine _setStateCoroutine;

    public TriggerZone InteractZone => _interactZone;
    public bool IsOnByDefault => _isOnByDefault;
    public bool EndlessSwitch => _endlessSwitch;

    private void OnDestroy()
    {
      if (_setStateCoroutine != null)
        StopCoroutine(_setStateCoroutine);
    }

    public void SetState(bool isOn, bool immediate)
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
