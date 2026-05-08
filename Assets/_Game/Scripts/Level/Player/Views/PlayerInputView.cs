using System;
using System.Collections;
using Sling.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sling.Level.Player.Views
{
  public class PlayerInputView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private InputActionReference _pointerPressActionRef;
    [SerializeField] private InputActionReference _pointerPositionActionRef;

    public event Action<Vector2> OnPreLaunchStart;
    public event Action<Vector2> OnPreLaunchUpdate;
    public event Action<Vector2> OnPreLaunchStop;

    private Camera _cam;

    private void Awake()
    {
      _cam = Camera.main;
      
      _pointerPressActionRef.action.performed += HandlePress;
      _pointerPressActionRef.action.canceled += HandleRelease;
    }

    private IEnumerator Start()
    {
      // HACK: to enable input on scene reload.
      // I don't know why ???
      yield return null;
      
      _pointerPressActionRef.action.Enable();
      _pointerPositionActionRef.action.Enable();
    }

    private void OnDestroy()
    {
      _pointerPressActionRef.action.performed -= HandlePress;
      _pointerPressActionRef.action.canceled -= HandleRelease;
    }

    private void Update() =>
      PreLaunchUpdate();

    private void PreLaunchUpdate()
    {
      if (_pointerPressActionRef.action.IsPressed())
        OnPreLaunchUpdate?.Invoke(PointerWorldPos());
    }

    private void HandlePress(InputAction.CallbackContext _) =>
      OnPreLaunchStart?.Invoke(PointerWorldPos());

    private void HandleRelease(InputAction.CallbackContext _) =>
      OnPreLaunchStop?.Invoke(PointerWorldPos());

    private Vector2 PointerWorldPos()
    {
      var screenPos = _pointerPositionActionRef.action.ReadValue<Vector2>();
      Vector3 worldPos = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.nearClipPlane));
      return worldPos;
    }
  }
}
