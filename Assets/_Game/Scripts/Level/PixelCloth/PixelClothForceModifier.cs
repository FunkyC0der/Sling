using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sling.Level.PixelCloth
{
  public readonly struct PixelClothForceContext
  {
    public PixelClothForceContext(
      int pointIndex,
      Vector2Int gridCoordinate,
      Vector2 normalizedCoordinate,
      Vector2 worldPosition,
      Vector2 worldVelocity,
      float time,
      float deltaTime)
    {
      PointIndex = pointIndex;
      GridCoordinate = gridCoordinate;
      NormalizedCoordinate = normalizedCoordinate;
      WorldPosition = worldPosition;
      WorldVelocity = worldVelocity;
      Time = time;
      DeltaTime = deltaTime;
    }

    public int PointIndex { get; }
    public Vector2Int GridCoordinate { get; }
    public Vector2 NormalizedCoordinate { get; }
    public Vector2 WorldPosition { get; }
    public Vector2 WorldVelocity { get; }
    public float Time { get; }
    public float DeltaTime { get; }
  }

  public interface IPixelClothForceModifier
  {
    Vector2 GetAcceleration(in PixelClothForceContext context);
  }

  [Serializable]
  public abstract class PixelClothForceModifier : IPixelClothForceModifier
  {
    public abstract Vector2 GetAcceleration(in PixelClothForceContext context);
  }

  [Serializable]
  public sealed class UnityGravityPixelClothForceModifier : PixelClothForceModifier
  {
    [Tooltip("Scales Physics2D.gravity applied to free cloth points. 1 = full gravity, 0 = none, above 1 = heavier / faster sag.")]
    [FormerlySerializedAs("_multiplier")]
    [Range(0f, 10f)] public float Multiplier = 1f;

    public override Vector2 GetAcceleration(in PixelClothForceContext context) =>
      Physics2D.gravity * Multiplier;
  }

  [Serializable]
  public sealed class GravityMassPixelClothForceModifier : PixelClothForceModifier
  {
    [Tooltip("Gravitational mass of free cloth points. 1 = Physics2D.gravity as-is; higher mass sags faster and feels heavier.")]
    [FormerlySerializedAs("_mass")]
    [Range(0f, 10f)] public float Mass = 1f;

    public override Vector2 GetAcceleration(in PixelClothForceContext context) =>
      Physics2D.gravity * Mass;
  }
}
