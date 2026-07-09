namespace Sling.Level.Common
{
  public interface IFaceDirectionView
  {
    bool IsFacingLeft { get; }
    void SetFacingLeft(bool value);
  }
}