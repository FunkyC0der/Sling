using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Elements.MovingLevelPart
{
  public class MovingLevelPartController : ControllerBase<MovingLevelPartView>
  {
    private readonly LevelEvents _levelEvents;
    private readonly UpdateEvents _updateEvents;

    private MovingLevelPartView _view;
    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private Vector2 _resetStartPosition;
    private float _resetElapsed;
    private bool _isActivated;
    private bool _isResetting;

    public MovingLevelPartController(
      IControllerFactory controllerFactory,
      LevelEvents levelEvents,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _levelEvents = levelEvents;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      _view = Args;
      _startPosition = _view.Rigidbody.position;

      _view.TriggerZone.OnEnter += OnTriggered;
      this.AddDisposableAction(() => _view.TriggerZone.OnEnter -= OnTriggered);

      _levelEvents.OnPlayerDeathStarted += ResetActivated;
      this.AddDisposableAction(() => _levelEvents.OnPlayerDeathStarted -= ResetActivated);

      _updateEvents.OnFixedUpdate += FixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= FixedUpdate);
    }

    private void OnTriggered(Collider2D collider)
    {
      if (_isActivated || _isResetting || collider.GetComponentInParent<PlayerView>() == null)
        return;

      _isActivated = true;
      _targetPosition = _view.Target.position;
    }

    private void ResetActivated()
    {
      if (!_isActivated)
        return;

      _resetStartPosition = _view.Rigidbody.position;
      _resetElapsed = 0;
      _isResetting = true;
    }

    private void FixedUpdate()
    {
      if (_isResetting)
        Reset();
      else if (_isActivated)
        MoveToTarget();
    }

    private void MoveToTarget()
    {
      float maxDistanceDelta = _view.Config.MoveSpeed * Time.fixedDeltaTime;
      _view.Rigidbody.MovePosition(
        Vector2.MoveTowards(_view.Rigidbody.position, _targetPosition, maxDistanceDelta));
    }

    private void Reset()
    {
      float resetDuration = _view.Config.ResetDuration;
      if (resetDuration <= 0)
      {
        CompleteReset();
        return;
      }

      _resetElapsed = Mathf.Min(_resetElapsed + Time.fixedDeltaTime, resetDuration);
      _view.Rigidbody.MovePosition(
        Vector2.Lerp(_resetStartPosition, _startPosition, _resetElapsed / resetDuration));

      if (_resetElapsed >= resetDuration)
        CompleteReset();
    }

    private void CompleteReset()
    {
      _view.Rigidbody.MovePosition(_startPosition);
      _isActivated = false;
      _isResetting = false;
    }
  }
}
