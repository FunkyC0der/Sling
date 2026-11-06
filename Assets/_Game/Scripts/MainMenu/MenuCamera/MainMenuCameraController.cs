using Playtika.Controllers;
using PrimeTween;
using Sling.Common.Extensions;
using UnityEngine;

namespace Sling.MainMenu.MenuCamera
{
  public class MainMenuCameraController : ControllerBase
  {
    private readonly MainMenuCameraView _view;
    private readonly MainMenuCameraConfig _config;
    private readonly MainMenuEvents _mainMenuEvents;
    private readonly GameModel _gameModel;

    private Tween _tween;

    public MainMenuCameraController(
      IControllerFactory controllerFactory,
      MainMenuCameraView view,
      MainMenuCameraConfig config,
      MainMenuEvents mainMenuEvents,
      GameModel gameModel)
      : base(controllerFactory)
    {
      _view = view;
      _config = config;
      _mainMenuEvents = mainMenuEvents;
      _gameModel = gameModel;
    }

    protected override void OnStart()
    {
      _view.CameraTransform.position = _view.GetAnchorPosition(_gameModel.WorldIndex);
      _view.ShowOnlyEffects(_gameModel.WorldIndex);

      _mainMenuEvents.WorldSelected += OnWorldSelected;
      this.AddDisposableAction(() => _mainMenuEvents.WorldSelected -= OnWorldSelected);
      this.AddDisposableAction(() => _tween.Stop());
    }

    private void OnWorldSelected(int worldIndex)
    {
      _tween.Stop();

      Vector3 target = _view.GetAnchorPosition(worldIndex);
      if (_view.CameraTransform.position == target)
      {
        _view.ShowOnlyEffects(worldIndex);
        return;
      }

      _view.DisableAllEffectScripts();
      _view.SetEffectsVisible(worldIndex, true);
      _tween = Tween.Position(
          _view.CameraTransform,
          target,
          _config.TransitionDuration,
          _config.TransitionEase)
        .OnComplete(() => _view.ShowOnlyEffects(worldIndex));
    }
  }
}
