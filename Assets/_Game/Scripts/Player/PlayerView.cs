using Sling.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sling.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerView : BaseView
    {
        [SerializeField] private InputActionReference _pointerPressActionRef;
        [SerializeField] private InputActionReference _pointerPositionActionRef;

        private Rigidbody2D _rb;
        private Camera _cam;
        private PlayerInputEvents _events;

        public float Mass => _rb.mass;

        public void Bind(PlayerInputEvents events)
        {
            _events = events;

            _rb = GetComponent<Rigidbody2D>();
            _cam = Camera.main;

            _pointerPressActionRef.action.performed += HandlePress;
            _pointerPressActionRef.action.canceled += HandleRelease;

            _pointerPressActionRef.action.Enable();
            _pointerPositionActionRef.action.Enable();
        }

        public void Unbind()
        {
            _events = null;

            _pointerPressActionRef.action.performed -= HandlePress;
            _pointerPressActionRef.action.canceled -= HandleRelease;
        }

        public void Launch(Vector2 force)
        {
            _rb.AddForce(force, ForceMode2D.Impulse);
        }

        private void Update()
        {
            if (_pointerPressActionRef.action.IsPressed())
                _events?.OnPointerDragged?.Invoke(PointerWorldPos());
        }

        private void HandlePress(InputAction.CallbackContext _) =>
            _events?.OnPointerDown?.Invoke(PointerWorldPos());

        private void HandleRelease(InputAction.CallbackContext _) =>
            _events?.OnPointerUp?.Invoke(PointerWorldPos());

        private Vector2 PointerWorldPos()
        {
            var screenPos = _pointerPositionActionRef.action.ReadValue<Vector2>();
            Vector3 worldPos = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.nearClipPlane));
            return worldPos;
        }
    }
}
