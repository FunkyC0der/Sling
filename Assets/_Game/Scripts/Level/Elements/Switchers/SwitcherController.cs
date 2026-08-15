using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class SwitcherController : ControllerBase<Switcher>
  {
    private readonly LevelEvents _levelEvents;

    private Switcher _view;
    private bool _isOn;
    private bool _isFirstSwitch = true;

    public SwitcherController(
      IControllerFactory controllerFactory,
      LevelEvents levelEvents)
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
    }

    protected override void OnStart()
    {
      _view = Args;
      _isOn = _view.IsOnByDefault;
      _view.SetState(_isOn, immediate: true);

      _view.InteractZone.OnEnter += OnEnter;
      this.AddDisposableAction(() => _view.InteractZone.OnEnter -= OnEnter);

      _levelEvents.OnPlayerDeathStarted += Reset;
      this.AddDisposableAction(() => _levelEvents.OnPlayerDeathStarted -= Reset);
    }

    private void OnEnter(Collider2D collider)
    {
      if (!_view.EndlessSwitch && !_isFirstSwitch)
        return;

      _isFirstSwitch = false;
      _isOn = !_isOn;
      _view.SetState(_isOn, immediate: false);
    }

    private void Reset()
    {
      if (_view.EndlessSwitch || _isFirstSwitch)
        return;

      _isFirstSwitch = true;
      _isOn = _view.IsOnByDefault;
      _view.SetState(_isOn, immediate: false);
    }
  }
}
