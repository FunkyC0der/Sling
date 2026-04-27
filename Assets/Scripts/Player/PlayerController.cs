using Playtika.Controllers;
using UnityEngine;

public class PlayerController : ControllerBase
{
    private readonly PlayerView _view;
    private readonly PlayerConfig _config;
    
    private bool _isDragging;
    private Vector2 _dragStartPos;

    public PlayerController(IControllerFactory factory, PlayerView view, PlayerConfig config) : base(factory)
    {
        _view = view;
        _config = config;
    }

    protected override void OnStart()
    {
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
        
        Vector2 dragVector = Vector2.ClampMagnitude(_dragStartPos - worldPos, _config.MaxDragDistance);
        Vector2 force = dragVector * _config.LaunchForceMultiplier;
        
        _view.Launch(force);
    }
}
