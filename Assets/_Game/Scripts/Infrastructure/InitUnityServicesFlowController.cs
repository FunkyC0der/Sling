using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
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
      
      if(!string.IsNullOrEmpty(UnityServicesOverrides.Name))  
        options.SetEnvironmentName(UnityServicesOverrides.Name);

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
  }
}
