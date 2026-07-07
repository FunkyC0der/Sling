using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.UnityConsent;

namespace Sling.Infrastructure
{
  public class InitUnityServicesFlowController : ControllerWithResultBase
  {
    public InitUnityServicesFlowController(IControllerFactory controllerFactory) : base(controllerFactory)
    {
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
