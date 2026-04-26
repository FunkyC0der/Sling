using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerView : MonoBehaviour
{
    [SerializeField] private InputActionReference _pointerPressActionRef;
    [SerializeField] private InputActionReference _pointerPositionActionRef;

    private Rigidbody2D _rb;
    private Camera _cam;
    private PlayerConfig _config;
    private bool _isDragging;
    private Vector3 _dragStartPos;

    public event Action<Vector2> OnLaunchRequested;

    public void Bind(PlayerConfig config)
    {
        _config = config;
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;

        _pointerPressActionRef.action.performed += OnPress;
        _pointerPressActionRef.action.canceled += OnRelease;
        _pointerPressActionRef.action.Enable();
        _pointerPositionActionRef.action.Enable();
    }

    public void Unbind()
    {
        _pointerPressActionRef.action.performed -= OnPress;
        _pointerPressActionRef.action.canceled -= OnRelease;
    }

    public void Launch(Vector2 force)
    {
        _rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        _isDragging = true;
        _dragStartPos = PointerWorldPosition();
    }

    private void OnRelease(InputAction.CallbackContext context)
    {
        if (!_isDragging)
            return;

        _isDragging = false;
        var dragVector = PointerWorldPosition() - _dragStartPos;
        dragVector = Vector3.ClampMagnitude(dragVector, _config.MaxDragDistance);
        OnLaunchRequested?.Invoke(dragVector);
    }

    private Vector3 PointerWorldPosition()
    {
        var screenPos = _pointerPositionActionRef.action.ReadValue<Vector2>();
        var worldPos = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.nearClipPlane));
        worldPos.z = 0;
        return worldPos;
    }
}
