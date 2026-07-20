using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Infrastructure.Analytics;
using Sling.Infrastructure.Authentication;
#if USE_UNITY_SERVICES
using Unity.Services.Core;
using Unity.Services.Core.Environments;
#if UNITY_EDITOR
using Unity.Services.Authentication;
using UnityEditor;
#endif
#endif
using UnityEngine;

namespace Sling.Infrastructure
{
  public class InitUnityServicesFlowController : ControllerWithResultBase
  {
#if UNITY_EDITOR
    public const string kEditorAuthenticationProfilePrefsKey =
      "Sling.UnityServices.Authentication.Profile";
    public const string kEditorAuthenticationDefaultProfile = "default";
#endif

    private readonly IAnalyticsService _analyticsService;
    private readonly IPlayerAuthenticationService _playerAuthenticationService;

    public InitUnityServicesFlowController(
      IControllerFactory controllerFactory,
      IAnalyticsService analyticsService,
      IPlayerAuthenticationService playerAuthenticationService)
      : base(controllerFactory)
    {
      _analyticsService = analyticsService;
      _playerAuthenticationService = playerAuthenticationService;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
#if USE_UNITY_SERVICES
      var options = new InitializationOptions();

#if UNITY_EDITOR
      SetEditorAuthenticationProfile(options);
#endif
      SetUnityEnvironmentName(options);

      await UnityServices.InitializeAsync(options)
        .AsUniTask()
        .AttachExternalCancellation(cancellationToken);

      _analyticsService.GrantConsent();
#endif

      try
      {
        await _playerAuthenticationService.LoginAsync(cancellationToken);
        await _playerAuthenticationService.CacheOrCreateNameAsync(cancellationToken);
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
        throw;
      }
      catch (Exception exception)
      {
        Debug.LogException(exception);
      }

      Complete();
    }

#if USE_UNITY_SERVICES
#if UNITY_EDITOR
    private static void SetEditorAuthenticationProfile(InitializationOptions options)
    {
      string profile = EditorPrefs.GetString(
        kEditorAuthenticationProfilePrefsKey,
        kEditorAuthenticationDefaultProfile);
      options.SetProfile(profile);
    }
#endif

    private static void SetUnityEnvironmentName(InitializationOptions options)
    {
      if (string.IsNullOrEmpty(UnityServicesOverrides.Name))
        return;

#if UNITY_EDITOR
      if (Application.isEditor && UnityServicesOverrides.Name != "development")
        throw new InvalidOperationException(
          $"Unity Services environment must be 'development' in Editor, but was '{UnityServicesOverrides.Name}'.");
#endif

      options.SetEnvironmentName(UnityServicesOverrides.Name);
    }
#endif
  }
}
