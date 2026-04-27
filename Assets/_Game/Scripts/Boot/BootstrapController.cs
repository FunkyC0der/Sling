using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;

namespace Sling.Boot
{
    public class BootstrapController : ControllerWithResultBase
    {
        public BootstrapController(IControllerFactory factory) : base(factory) { }

        protected override UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            // Load configs, initialize services here as the game grows.
            Complete();
            return UniTask.CompletedTask;
        }
    }
}
