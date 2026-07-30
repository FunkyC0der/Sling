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
    [SerializeField] private InputActionReference _pointerDeltaActionRef;
    [SerializeField] private InputActionReference _cancelPreLaunchActionRef;
    [SerializeField] private InputActionReference _pauseActionRef;

    public event Action OnPreLaunchStart;
    public event Action<Vector2> OnPreLaunchUpdate;
    public event Action OnPreLaunchStop;
    public event Action OnPreLaunchCancel;
    public event Action OnPauseRequested;

    private Camera _cam;
    private Vector2 _pointerWorldDelta;

    private void Awake()
    {
      _cam = Camera.main;

      _pointerPressActionRef.action.performed += HandlePress;
      _pointerPressActionRef.action.canceled += HandleRelease;
      _cancelPreLaunchActionRef.action.performed += HandleCancelPreLaunch;
      _pauseActionRef.action.performed += HandlePause;
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
      _pauseActionRef.action.performed -= HandlePause;
    }

    public void EnableInput()
    {
      _pointerPressActionRef.action.Enable();
      _pointerDeltaActionRef.action.Enable();
      _cancelPreLaunchActionRef.action.Enable();
      _pauseActionRef.action.Enable();
    }

    public void DisableInput()
    {
      _pointerPressActionRef.action.Disable();
      _pointerDeltaActionRef.action.Disable();
      _cancelPreLaunchActionRef.action.Disable();
      _pauseActionRef.action.Disable();
    }

    private void Update() =>
      PreLaunchUpdate();

    private void PreLaunchUpdate()
    {
      if (!_pointerPressActionRef.action.IsPressed())
        return;

      AccumulatePointerDelta();
      OnPreLaunchUpdate?.Invoke(_pointerWorldDelta);
    }

    private void HandlePress(InputAction.CallbackContext _)
    {
      _pointerWorldDelta = Vector2.zero;
      OnPreLaunchStart?.Invoke();
    }

    private void HandleRelease(InputAction.CallbackContext _)
    {
      AccumulatePointerDelta();
      OnPreLaunchUpdate?.Invoke(_pointerWorldDelta);
      OnPreLaunchStop?.Invoke();
    }

    private void HandleCancelPreLaunch(InputAction.CallbackContext ctx) => 
      OnPreLaunchCancel?.Invoke();

    private void HandlePause(InputAction.CallbackContext _) =>
      OnPauseRequested?.Invoke();

    private void AccumulatePointerDelta()
    {
      Vector2 screenDelta = _pointerDeltaActionRef.action.ReadValue<Vector2>();
      Vector3 screenOrigin = new Vector3(0f, 0f, _cam.nearClipPlane);
      Vector3 worldOrigin = _cam.ScreenToWorldPoint(screenOrigin);
      Vector3 worldOffset = _cam.ScreenToWorldPoint(screenOrigin + (Vector3)screenDelta) - worldOrigin;
      _pointerWorldDelta += (Vector2)worldOffset;
    }
  }
}
