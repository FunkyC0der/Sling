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

    public static Vector2 Mirror(this Vector2 vector, Vector2 axis)
    {
      axis.Normalize();
      return 2f * Vector2.Dot(vector, axis) * axis - vector;
    }
    
    public static Vector3 Multiply(this Vector3 vector, float x, float y = 1, float z = 1) =>
      new Vector3(x * vector.x, y * vector.y, z * vector.z);
  }
}