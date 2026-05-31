namespace Sling.Common.Extensions
{
  public static class NumberExtensions
  {
    public static int AddFlag(this int mask, int flag) =>
      mask | flag;

    public static int RemoveFlag(this int mask, int flag) =>
      mask & ~flag;

    public static bool HasFlag(this int mask, int flag) =>
      (mask & flag) != 0;
  }
}