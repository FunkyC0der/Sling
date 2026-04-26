using Playtika.Controllers;
using UnityEngine;

public class PlayerController : ControllerBase
{
    private readonly PlayerView _view;
    private readonly PlayerConfig _config;
    private PlayerModel _model;
    private bool _isDragging;
    private Vector2 _dragStartPos;

    public PlayerController(IControllerFactory factory, PlayerView view, PlayerConfig config) : base(factory)
    {
        _view = view;
        _config = config;
    }

    protected override void OnStart()
    {
        _model = new PlayerModel();
        _view.Bind();

        AddDisposable(new DisposableToken(() =>
        {
            _view.OnPointerDown -= OnPointerDown;
            _view.OnPointerUp -= OnPointerUp;
            _view.Unbind();
        }));
        _view.OnPointerDown += OnPointerDown;
        _view.OnPointerUp += OnPointerUp;
    }

    private void OnPointerDown(Vector2 worldPos)
    {
        _isDragging = true;
        _dragStartPos = worldPos;
    }

    private void OnPointerUp(Vector2 worldPos)
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        var dragVector = Vector2.ClampMagnitude(worldPos - _dragStartPos, _config.MaxDragDistance);
        var force = dragVector * _config.LaunchForceMultiplier;
        _model.SetLaunched(force);
        _view.Launch(force);
    }
}
