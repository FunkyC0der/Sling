using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Level;
using UnityEngine;

namespace Sling.Hazards
{
    public class MovingSawController : ControllerBase<MovingSawView>
    {
        private readonly MovingSawConfig _config;
        private readonly LevelEvents _events;

        public MovingSawController(IControllerFactory factory, MovingSawConfig config, LevelEvents events) : base(factory)
        {
            _config = config;
            _events = events;
        }

        protected override void OnStart()
        {
            AddDisposable(new DisposableToken(() => Args.OnPlayerHit -= OnPlayerHit));
            Args.OnPlayerHit += OnPlayerHit;

            MoveLoopAsync(CancellationToken).Forget(e => Debug.LogException(e));
        }

        private void OnPlayerHit() => _events.RaisePlayerDied();

        private async UniTask MoveLoopAsync(CancellationToken cancellationToken)
        {
            var target = Args.PointB.position;

            while (!cancellationToken.IsCancellationRequested)
            {
                var newPos = Vector3.MoveTowards(Args.Position, target, _config.Speed * Time.deltaTime);
                Args.SetPosition(newPos);

                if (Vector3.Distance(newPos, target) < 0.01f)
                    target = target == Args.PointA.position ? Args.PointB.position : Args.PointA.position;

                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
