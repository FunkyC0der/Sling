using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DragAndLaunch : MonoBehaviour
{
    [SerializeField] private float _maxDragDistance = 5f;
    [SerializeField] private float _launchForceMultiplier = 10f;
    [SerializeField] private InputActionReference _pointerPressActionRef;
    [SerializeField] private InputActionReference _pointerPositionActionRef;

    private Rigidbody2D _rb;
    private Camera _cam;
    private bool _isDragging;
    private Vector3 _dragStartPos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        _pointerPressActionRef.action.performed += OnPress;
        _pointerPressActionRef.action.canceled += OnRelease;

        _pointerPressActionRef.action.Enable();
        _pointerPositionActionRef.action.Enable();
    }
    
    private void OnPress(InputAction.CallbackContext context)
    {
        _isDragging = true;
        _dragStartPos = MouseWorldPosition();
    }
    
    private void OnRelease(InputAction.CallbackContext context)
    {
        if (!_isDragging) 
            return;

        _isDragging = false;
        Vector3 launchVector = MouseWorldPosition() - _dragStartPos;
        launchVector = Vector3.ClampMagnitude(launchVector, _maxDragDistance);

        _rb.AddForce(launchVector * _launchForceMultiplier, ForceMode2D.Impulse);
    }
    
    private Vector3 MouseWorldPosition()
    {
        var mousePos = _pointerPositionActionRef.action.ReadValue<Vector2>();
        Vector3 worldPos = _cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cam.nearClipPlane));
        worldPos.z = 0;
        return worldPos;
    }
}