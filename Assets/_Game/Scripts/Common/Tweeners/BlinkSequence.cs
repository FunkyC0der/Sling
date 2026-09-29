using PrimeTween;
using UnityEngine;

namespace Sling.Common.Tweeners
{
  public static class BlinkSequence
  {
    public static Sequence Create(Material material, int propertyId, int blinkCount, float duration, float blinkAmount)
    {
      float halfBlinkDuration = duration / blinkCount * 0.5f;

      Sequence sequence = Sequence.Create();
      for (int i = 0; i < blinkCount; i++)
      {
        sequence.Chain(Tween.MaterialProperty(material, propertyId,
          startValue: 0f,
          endValue: blinkAmount,
          halfBlinkDuration));

        sequence.Chain(Tween.MaterialProperty(material, propertyId,
          endValue: 0f,
          halfBlinkDuration));
      }

      return sequence;
    }
  }
}
