using System;
using Playtika.Controllers;
using Sling.Common.Extensions;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Sling.Infrastructure.FixedViewport
{
  public class FixedViewportController : ControllerBase
  {
    private const string _kKBackgroundCameraName = "FixedViewport Background Camera";
    private const float _kKBackgroundCameraDepth = -100f;

    private readonly FixedViewportConfig _config;
    private readonly UpdateEvents _updateEvents;

    private GameObject _backgroundCameraObject;
    private Camera _backgroundCamera;
    private int _screenWidth = -1;
    private int _screenHeight = -1;

    public FixedViewportController(
      IControllerFactory controllerFactory,
      FixedViewportConfig config,
      UpdateEvents updateEvents)
      : base(controllerFactory)
    {
      _config = config;
      _updateEvents = updateEvents;
    }

    protected override void OnStart()
    {
      ValidateConfig();
      CreateBackgroundCamera();

      SceneManager.sceneLoaded += OnSceneLoaded;
      this.AddDisposableAction(() => SceneManager.sceneLoaded -= OnSceneLoaded);

      _updateEvents.OnUpdate += OnUpdate;
      this.AddDisposableAction(() => _updateEvents.OnUpdate -= OnUpdate);

      Refresh(force: true);
    }

    private void ValidateConfig()
    {
      Vector2Int referenceAspectRatio = _config.ReferenceAspectRatio;
      if (referenceAspectRatio.x <= 0 || referenceAspectRatio.y <= 0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(_config.ReferenceAspectRatio),
          referenceAspectRatio,
          "Reference aspect ratio dimensions must be greater than zero.");
      }
    }

    private void CreateBackgroundCamera()
    {
      _backgroundCameraObject = new GameObject(_kKBackgroundCameraName);
      Object.DontDestroyOnLoad(_backgroundCameraObject);
      this.AddDisposableAction(DestroyBackgroundCamera);

      _backgroundCamera = _backgroundCameraObject.AddComponent<Camera>();
      _backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
      _backgroundCamera.backgroundColor = _config.BarsColor;
      _backgroundCamera.cullingMask = 0;
      _backgroundCamera.depth = _kKBackgroundCameraDepth;
      _backgroundCamera.rect = new Rect(0f, 0f, 1f, 1f);
      _backgroundCamera.allowHDR = false;
      _backgroundCamera.allowMSAA = false;
      _backgroundCamera.allowDynamicResolution = false;
      _backgroundCamera.useOcclusionCulling = false;

      UniversalAdditionalCameraData cameraData =
        _backgroundCameraObject.AddComponent<UniversalAdditionalCameraData>();
      cameraData.renderType = CameraRenderType.Base;
      cameraData.renderShadows = false;
      cameraData.requiresDepthOption = CameraOverrideOption.Off;
      cameraData.requiresColorOption = CameraOverrideOption.Off;
      cameraData.renderPostProcessing = false;
    }

    private void OnUpdate()
    {
      if (_screenWidth == Screen.width && _screenHeight == Screen.height)
        return;

      Refresh(force: false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) =>
      Refresh(force: true);

    private void Refresh(bool force)
    {
      if (Screen.width <= 0 || Screen.height <= 0)
        return;

      if (!force && _screenWidth == Screen.width && _screenHeight == Screen.height)
        return;

      Camera worldCamera = Camera.main;
      if (!worldCamera)
        return;

      _screenWidth = Screen.width;
      _screenHeight = Screen.height;
      worldCamera.rect = CalculateViewport(_screenWidth, _screenHeight);
    }

    private Rect CalculateViewport(int screenWidth, int screenHeight)
    {
      float targetAspect =
        (float)_config.ReferenceAspectRatio.x / _config.ReferenceAspectRatio.y;
      float screenAspect = (float)screenWidth / screenHeight;

      if (screenAspect > targetAspect)
      {
        float width = targetAspect / screenAspect;
        return new Rect((1f - width) * 0.5f, 0f, width, 1f);
      }

      float height = screenAspect / targetAspect;
      return new Rect(0f, (1f - height) * 0.5f, 1f, height);
    }

    private void DestroyBackgroundCamera()
    {
      Camera worldCamera = Camera.main;
      if (worldCamera)
        worldCamera.rect = new Rect(0f, 0f, 1f, 1f);

      if (_backgroundCameraObject)
        Object.Destroy(_backgroundCameraObject);

      _backgroundCamera = null;
      _backgroundCameraObject = null;
    }
  }
}
