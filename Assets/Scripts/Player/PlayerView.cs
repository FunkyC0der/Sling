using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerView : BaseView
{
    [SerializeField] private InputActionReference _pointerPressActionRef;
    [SerializeField] private InputActionReference _pointerPositionActionRef;

    private Rigidbody2D _rb;
    private Camera _cam;

    public event Action<Vector2> OnPointerDown;
    public event Action<Vector2> OnPointerUp;

    public void Bind()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        _pointerPressActionRef.action.performed += HandlePress;
        _pointerPressActionRef.action.canceled += HandleRelease;
        _pointerPressActionRef.action.Enable();
        _pointerPositionActionRef.action.Enable();
    }

    public void Unbind()
    {
        _pointerPressActionRef.action.performed -= HandlePress;
        _pointerPressActionRef.action.canceled -= HandleRelease;
    }

    public void Launch(Vector2 force)
    {
        _rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void HandlePress(InputAction.CallbackContext _) =>
        OnPointerDown?.Invoke(PointerWorldPos());

    private void HandleRelease(InputAction.CallbackContext _) =>
        OnPointerUp?.Invoke(PointerWorldPos());

    private Vector2 PointerWorldPos()
    {
        var screenPos = _pointerPositionActionRef.action.ReadValue<Vector2>();
        var worldPos = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.nearClipPlane));
        return worldPos;
    }
}
