using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
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
      await Unity.Services.Core.UnityServices.InitializeAsync()
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