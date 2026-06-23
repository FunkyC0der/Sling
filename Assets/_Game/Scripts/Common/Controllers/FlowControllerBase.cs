using Playtika.Controllers;

namespace Sling.Common.Controllers
{
  public abstract class FlowControllerBase<TArg> : ControllerWithResultBase<TArg, EmptyControllerResult>
  {
    protected FlowControllerBase(IControllerFactory controllerFactory)
      : base(controllerFactory)
    {
    }

    /// <summary>
    /// Complete controller with default result.
    /// </summary>
    protected void Complete()
    {
      Complete(default);
    }
  }
}