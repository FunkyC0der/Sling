using System;
using UnityEngine;

namespace Sling.Level.PixelCloth
{
  [Serializable]
  public abstract class PixelClothAnchorMotionStrategy
  {
    public abstract void FillPinnedPositions(
      Vector2 initialAnchorPosition,
      Vector2 currentAnchorPosition,
      Vector2[] restPinnedWorldPositions,
      Vector2[] output);
  }

  [Serializable]
  public sealed class TranslationOnlyAnchorMotionStrategy : PixelClothAnchorMotionStrategy
  {
    public override void FillPinnedPositions(
      Vector2 initialAnchorPosition,
      Vector2 currentAnchorPosition,
      Vector2[] restPinnedWorldPositions,
      Vector2[] output)
    {
      Vector2 translation = currentAnchorPosition - initialAnchorPosition;

      for (int i = 0; i < restPinnedWorldPositions.Length; i++)
        output[i] = restPinnedWorldPositions[i] + translation;
    }
  }
}
