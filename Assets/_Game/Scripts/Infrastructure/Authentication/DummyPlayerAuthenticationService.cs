using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Infrastructure.Authentication
{
  public class DummyPlayerAuthenticationService : IPlayerAuthenticationService
  {
    public UniTask LoginAsync(CancellationToken cancellationToken) =>
      UniTask.CompletedTask;

    public UniTask CacheOrCreateNameAsync(CancellationToken cancellationToken) =>
      UniTask.CompletedTask;

    public string GetName() =>
      string.Empty;

    public bool IsSignedIn() =>
      false;
  }
}
