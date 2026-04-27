using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using Sling.Core;
using Sling.Hazards;
using Sling.Player;
using Sling.UI;
using UnityEngine.SceneManagement;

namespace Sling.Level
{
    public class LoadLevelController : ControllerWithResultBase<int, IControllerFactory>
    {
        private readonly PlayerConfig _playerConfig;
        private readonly MovingSawConfig _movingSawConfig;
        private readonly HudView _hudView;
        private readonly LevelResultView _levelResultView;

        public LoadLevelController(
            IControllerFactory factory,
            PlayerConfig playerConfig,
            MovingSawConfig movingSawConfig,
            HudView hudView,
            LevelResultView levelResultView) : base(factory)
        {
            _playerConfig = playerConfig;
            _movingSawConfig = movingSawConfig;
            _hudView = hudView;
            _levelResultView = levelResultView;
        }

        protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
        {
            var sceneName = $"Level_{Args:00}";
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single)
                .ToUniTask(cancellationToken: cancellationToken);

            Scene scene = SceneManager.GetSceneByName(sceneName);
            ViewsCollector views = new();
            views.CollectViews(scene);

            if (!views.GetOne<PlayerView>() || !views.GetOne<FinishView>())
            {
                Fail(new System.Exception(
                    $"Required view missing in scene '{sceneName}': PlayerView and FinishView are mandatory"));
                return;
            }

            var events = new LevelEvents();
            IControllerFactory levelFactory = BuildLevelFactory(views, events);
            Complete(levelFactory);
        }

        private IControllerFactory BuildLevelFactory(ViewsCollector views, LevelEvents events)
        {
            var f = new ControllerFactory();
            f.Register(() => new LevelLoopController(f, events));
            f.Register(() => new PlayerController(f, views.GetOne<PlayerView>(), _playerConfig));
            f.Register(() => new HazardsController(f, views.GetAll<HazardView>(), events));
            f.Register(() => new FinishController(f, views.GetOne<FinishView>(), events));
            f.Register(() => new HudController(f, _hudView, events));
            f.Register(() => new LevelResultController(f, _levelResultView));
            f.Register(() => new MovingSawController(f, _movingSawConfig, events));
            return f;
        }
    }
}