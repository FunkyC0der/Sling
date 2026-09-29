using Playtika.Controllers;
using Sling.Audio;
using Sling.Common.Extensions;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  public class SwitcherController : ControllerBase<Switcher>
  {
    private readonly LevelEvents _levelEvents;
    private readonly AudioEvents _audioEvents;

    private Switcher _view;
    private bool _isOn;
    private bool _isFirstSwitch = true;

    public SwitcherController(
      IControllerFactory controllerFactory,
      LevelEvents levelEvents,
      AudioEvents audioEvents)
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
      _audioEvents = audioEvents;
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
      _audioEvents.PlaySFX?.Invoke(AudioClipId.Switcher);
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
