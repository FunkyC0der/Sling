using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Playtika.Controllers;
using VContainer;

namespace Sling.Common.Controllers
{
  public class OptionalViewFlowController<TView> : FlowControllerBase<Func<TView, UniTask>>
  {
    private readonly IObjectResolver _resolver;

    public OptionalViewFlowController(IControllerFactory factory, IObjectResolver resolver)
      : base(factory)
    {
      _resolver = resolver;
    }

    protected override async UniTask OnFlowAsync(CancellationToken cancellationToken)
    {
      if (_resolver.TryResolve(out TView view))
        await Args(view);
      
      Complete();
    }
  }
}