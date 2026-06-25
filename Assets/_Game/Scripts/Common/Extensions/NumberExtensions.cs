using UnityEngine;

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

    public static Vector3 Multiply(this Vector3 vector, float x, float y = 1, float z = 1) =>
      new Vector3(x * vector.x, y * vector.y, z * vector.z);

    public static float RandomRange(this Vector2 range) =>
      Random.Range(range.x, range.y);
  }
}