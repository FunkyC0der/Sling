using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Infrastructure.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UnityConsent;

namespace Sling.Infrastructure
{
  public class InitUnityServicesFlowController : ControllerWithResultBase
  {
    private readonly PlayerAuthenticationService _playerAuthenticationService;

    public InitUnityServicesFlowController(
      IControllerFactory controllerFactory,
      PlayerAuthenticationService playerAuthenticationService)
      : base(controllerFactory)
    {
      _playerAuthenticationService = playerAuthenticationService;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      var options = new InitializationOptions();
      
      SetUnityEnvironmentName(options);

      await UnityServices.InitializeAsync(options)
        .AsUniTask()
        .AttachExternalCancellation(cancellationToken);

      ConsentState consentState = EndUserConsent.GetConsentState();
      if (consentState.AnalyticsIntent != ConsentStatus.Granted)
      {
        consentState.AnalyticsIntent = ConsentStatus.Granted;
        EndUserConsent.SetConsentState(consentState);
      }

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
  }
}
