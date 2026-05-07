using System.Collections.Generic;
using Playtika.Controllers;
using Sling.Level.Player.Views;
using UnityEngine;

namespace Sling.Level.StickyWall
{
  public class StickyWallsController : ControllerBase
  {
    private const float _kMaxFallSpeed = float.PositiveInfinity;
    
    private readonly IReadOnlyList<StickyWallView> _stickyWalls;
    private readonly PlayerView _playerView;

    private float _maxFallSpeed = _kMaxFallSpeed;

    public StickyWallsController(IControllerFactory controllerFactory,
      StickyWallView[] stickyWalls,
      PlayerView playerView)
      : base(controllerFactory)
    {
      _stickyWalls = stickyWalls;
      _playerView = playerView;
    }

    protected override void OnStart()
    {
      _playerView.OnFixedTick += OnFixedTick;
      foreach (StickyWallView wall in _stickyWalls)
      {
        wall.OnPlayerEnter += OnEnterStickyWall;
        wall.OnPlayerExit += OnExitStickyWall;
      }
    }

    protected override void OnStop()
    {
      _playerView.OnFixedTick -= OnFixedTick;
      foreach (StickyWallView wall in _stickyWalls)
      {
        wall.OnPlayerEnter -= OnEnterStickyWall;
        wall.OnPlayerExit -= OnExitStickyWall;
      }
    }

    private void OnFixedTick()
    {
      if(_playerView.BodyType != RigidbodyType2D.Static)
        _playerView.LinearVelocityY = Mathf.Max(_playerView.LinearVelocityY, -_maxFallSpeed);
    }

    private void OnEnterStickyWall(StickyWallConfig config) =>
      _maxFallSpeed = config.MaxFallSpeed;

    private void OnExitStickyWall() =>
      _maxFallSpeed = _kMaxFallSpeed;
  }
}
