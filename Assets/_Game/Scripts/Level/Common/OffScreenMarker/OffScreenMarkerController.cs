using Playtika.Controllers;
using Sling.Common.Extensions;
using Sling.Infrastructure;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sling.Level.Common.OffScreenMarker
{
  public class OffScreenMarkerController : ControllerBase
  {
    private readonly OffScreenMarkerView _view;
    private readonly UpdateEvents _updateEvents;
    private readonly Camera _camera;

    private GameObject _marker;

    public OffScreenMarkerController(IControllerFactory controllerFactory, OffScreenMarkerView view, UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _view = view;
      _updateEvents = updateEvents;
      _camera = Camera.main;
    }

    protected override void OnStart()
    {
      _updateEvents.OnPostLateUpdate += MonitorObjectPosition;
      this.AddDisposableAction(() => _updateEvents.OnPostLateUpdate -= MonitorObjectPosition);
      
      this.AddDisposableAction(DestroyMarker);
    }

    private void MonitorObjectPosition()
    {
      if (IsTargetVisible())
      {
        HideMarker();
        return;
      }

      ShowMarker();
      UpdateMarkerPosition();
    }

    private bool IsTargetVisible()
    {
      Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(_camera);
      return GeometryUtility.TestPlanesAABB(cameraPlanes, _view.Renderer.bounds);
    }

    private void ShowMarker()
    {
      if (!_marker)
        _marker = Object.Instantiate(_view.MarkerPrefab);

      if (!_marker.activeSelf)
        _marker.SetActive(true);
    }

    private void HideMarker()
    {
      if (_marker && _marker.activeSelf)
        _marker.SetActive(false);
    }

    private void UpdateMarkerPosition()
    {
      Vector3 targetCenter = _view.Renderer.bounds.center;
      Vector3 targetViewportPosition = _camera.WorldToViewportPoint(targetCenter);
      Rect safeViewportArea = GetSafeViewportArea();
      
      var markerViewportPosition = new Vector3(
        Mathf.Clamp(targetViewportPosition.x, safeViewportArea.xMin, safeViewportArea.xMax),
        Mathf.Clamp(targetViewportPosition.y, safeViewportArea.yMin, safeViewportArea.yMax),
        targetViewportPosition.z);

      Vector3 markerWorldPosition = _camera.ViewportToWorldPoint(markerViewportPosition);
      markerWorldPosition.z = _marker.transform.position.z;

      _marker.transform.position = markerWorldPosition;

      Vector2 direction = targetCenter - markerWorldPosition;
      if (direction.sqrMagnitude > 0)
      {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _marker.transform.rotation = Quaternion.Euler(0, 0, angle);
      }
    }

    private Rect GetSafeViewportArea()
    {
      Rect safeArea = Screen.safeArea;

      float xMin = safeArea.xMin / Screen.width;
      float yMin = safeArea.yMin / Screen.height;
      float xMax = safeArea.xMax / Screen.width;
      float yMax = safeArea.yMax / Screen.height;

      return Rect.MinMaxRect(
        xMin,
        yMin,
        xMax,
        yMax);
    }

    private void DestroyMarker()
    {
      if (!_marker)
        return;

      Object.Destroy(_marker);
      _marker = null;
    }
  }
}
