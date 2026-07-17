using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace Sling.Infrastructure.Authentication
{
  public class PlayerAuthenticationService
  {
    public async UniTask LoginAsync(CancellationToken cancellationToken)
    {
      if (IsSignedIn())
        return;

      await AuthenticationService.Instance.SignInAnonymouslyAsync()
        .AsUniTask()
        .AttachExternalCancellation(cancellationToken);
    }

    public async UniTask CacheOrCreateNameAsync(CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(GetName()))
      {
        await AuthenticationService.Instance.GetPlayerNameAsync(true)
          .AsUniTask()
          .AttachExternalCancellation(cancellationToken);
      }
      
      Debug.Log($"Player auth name: {GetName()}");
    }

    public string GetName() => 
      AuthenticationService.Instance.PlayerName;

    public bool IsSignedIn() => 
      AuthenticationService.Instance.IsSignedIn;
  }
}
