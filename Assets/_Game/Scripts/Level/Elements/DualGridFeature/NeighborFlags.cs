namespace Sling.Level.Elements.DualGridFeature
{
  public class NeighborFlags
  {
    public const int kNone = 0;
    public const int kBottomLeft = 0b0001;
    public const int kBottomRight = 0b0010;
    public const int kTopRight = 0b0100;
    public const int kTopLeft = 0b1000;
    public const int kFull = kBottomLeft | kBottomRight | kTopRight | kTopLeft;
  }
}
