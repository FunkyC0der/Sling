using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sling.Infrastructure.Authentication
{
  public interface IPlayerAuthenticationService
  {
    UniTask LoginAsync(CancellationToken cancellationToken);
    UniTask CacheOrCreateNameAsync(CancellationToken cancellationToken);
    string GetName();
    bool IsSignedIn();
  }
}
