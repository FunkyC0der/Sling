using System;
using System.Collections;
using Sling.Common.Views;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sling.Level.Player
{
  public class PlayerInputView : MonoBehaviour, IUniqueView
  {
    [SerializeField] private InputActionReference _pointerPressActionRef;
    [SerializeField] private InputActionReference _pointerPositionActionRef;
    [SerializeField] private InputActionReference _cancelPreLaunchActionRef;

    public event Action<Vector2> OnPreLaunchStart;
    public event Action<Vector2> OnPreLaunchUpdate;
    public event Action<Vector2> OnPreLaunchStop;
    public event Action OnPreLaunchCancel;

    private Camera _cam;

    private void Awake()
    {
      _cam = Camera.main;

      _pointerPressActionRef.action.performed += HandlePress;
      _pointerPressActionRef.action.canceled += HandleRelease;
      _cancelPreLaunchActionRef.action.performed += HandleCancelPreLaunch;
    }

    private IEnumerator Start()
    {
      // HACK: to enable input on scene reload.
      // I don't know why ???
      yield return null;

      EnableInput();
    }

    private void OnDestroy()
    {
      _pointerPressActionRef.action.performed -= HandlePress;
      _pointerPressActionRef.action.canceled -= HandleRelease;
      _cancelPreLaunchActionRef.action.performed -= HandleCancelPreLaunch;
    }

    public void EnableInput()
    {
      _pointerPressActionRef.action.Enable();
      _pointerPositionActionRef.action.Enable();
      _cancelPreLaunchActionRef.action.Enable();
    }

    public void DisableInput()
    {
      _pointerPressActionRef.action.Disable();
      _pointerPositionActionRef.action.Disable();
      _cancelPreLaunchActionRef.action.Disable();
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

    private void HandleCancelPreLaunch(InputAction.CallbackContext ctx) => 
      OnPreLaunchCancel?.Invoke();

    private Vector2 PointerWorldPos()
    {
      var screenPos = _pointerPositionActionRef.action.ReadValue<Vector2>();
      Vector3 worldPos = _cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _cam.nearClipPlane));
      return worldPos;
    }
  }
}
