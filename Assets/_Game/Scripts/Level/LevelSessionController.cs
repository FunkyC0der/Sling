using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using VContainer;
using VContainer.Unity;

namespace Sling.Level
{
    public class LevelSessionController : ControllerWithResultBase<LevelSessionResult>
    {
        public LevelSessionController(IControllerFactory factory)
            : base(factory)
        {
        }

        protected override async UniTask OnFlowAsync(CancellationToken ct)
        {
            LifetimeScope levelScope = await ExecuteAndWaitResultAsync<BuildLevelFactoryController, LifetimeScope>(ct);
            AddDisposable(levelScope);

            while (true)
            {
                var levelFactory = levelScope.Container.Resolve<IControllerFactory>();
                
                GameplayOutcome outcome =
                    await ExecuteAndWaitResultAsync<LevelLoopController, GameplayOutcome>(levelFactory, ct);
            }
        }
    }
}
