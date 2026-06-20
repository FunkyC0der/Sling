using Sling.Common.Views;

namespace Sling.Level.Common
{
  public interface IFaceDirectionView : IGameObjectView
  {
    bool IsFacingLeft { get; }
    void SetFacingLeft(bool value);
  }
}