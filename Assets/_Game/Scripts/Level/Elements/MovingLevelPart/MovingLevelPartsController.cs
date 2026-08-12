using System;
using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Common.Views;
using Sling.Infrastructure;
using Sling.Level.Player;
using Sling.Level.Session;
using UnityEngine;

namespace Sling.Level.Elements.MovingLevelPart
{
  public class MovingLevelPartsController : ControllerBase
  {
    private sealed class PartState
    {
      public MovingLevelPartView View;
      public Vector2 StartPosition;
      public Vector2 TargetPosition;
      public Vector2 ResetStartPosition;
      public Action<Collider2D> TriggerHandler;
      public float ResetElapsed;
      public bool IsActivated;
      public bool IsResetting;
    }

    private readonly IReadOnlyList<MovingLevelPartView> _views;
    private readonly LevelEvents _levelEvents;
    private readonly UpdateEvents _updateEvents;
    private readonly List<PartState> _parts = new();

    public MovingLevelPartsController(
      IControllerFactory controllerFactory,
      IOptionalViewProvider optionalViewProvider,
      LevelEvents levelEvents,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _views = optionalViewProvider.GetAll<MovingLevelPartView>();
      _levelEvents = levelEvents;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      foreach (MovingLevelPartView view in _views)
      {
        var part = new PartState { View = view, StartPosition = view.Rigidbody.position };
        _parts.Add(part);
        part.TriggerHandler = collider => OnTriggered(part, collider);
        view.TriggerZone.OnEnter += part.TriggerHandler;
        this.AddDisposableAction(() => view.TriggerZone.OnEnter -= part.TriggerHandler);
      }

      _levelEvents.OnPlayerDeathStarted += ResetActivatedParts;
      this.AddDisposableAction(() => _levelEvents.OnPlayerDeathStarted -= ResetActivatedParts);

      _updateEvents.OnFixedUpdate += FixedUpdate;
      this.AddDisposableAction(() => _updateEvents.OnFixedUpdate -= FixedUpdate);
    }

    private void OnTriggered(PartState part, Collider2D collider)
    {
      if (part.IsActivated || part.IsResetting || collider.GetComponentInParent<PlayerView>() == null)
        return;

      part.IsActivated = true;
      part.TargetPosition = part.View.Target.position;
    }

    private void ResetActivatedParts()
    {
      foreach (PartState part in _parts)
      {
        if (!part.IsActivated)
          continue;

        part.ResetStartPosition = part.View.Rigidbody.position;
        part.ResetElapsed = 0;
        part.IsResetting = true;
      }
    }

    private void FixedUpdate()
    {
      foreach (PartState part in _parts)
      {
        if (part.IsResetting)
          Reset(part);
        else if (part.IsActivated)
          MoveToTarget(part);
      }
    }

    private static void MoveToTarget(PartState part)
    {
      float maxDistanceDelta = part.View.Config.MoveSpeed * Time.fixedDeltaTime;
      part.View.Rigidbody.MovePosition(
        Vector2.MoveTowards(part.View.Rigidbody.position, part.TargetPosition, maxDistanceDelta));
    }

    private static void Reset(PartState part)
    {
      float resetDuration = part.View.Config.ResetDuration;
      if (resetDuration <= 0)
      {
        CompleteReset(part);
        return;
      }

      part.ResetElapsed = Mathf.Min(part.ResetElapsed + Time.fixedDeltaTime, resetDuration);
      part.View.Rigidbody.MovePosition(
        Vector2.Lerp(part.ResetStartPosition, part.StartPosition, part.ResetElapsed / resetDuration));

      if (part.ResetElapsed >= resetDuration)
        CompleteReset(part);
    }

    private static void CompleteReset(PartState part)
    {
      part.View.Rigidbody.MovePosition(part.StartPosition);
      part.IsActivated = false;
      part.IsResetting = false;
    }
  }
}
