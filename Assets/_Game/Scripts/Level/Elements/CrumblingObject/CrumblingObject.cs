using System.Collections.Generic;
using Sling.Common.Collission;
using Sling.Common.Views;
using UnityEngine;

namespace Sling.Level.Elements.CrumblingObject
{
  public class CrumblingObject : MonoBehaviour, IViewListItem
  {
    private static readonly int _sFadeAmountId = Shader.PropertyToID("_FadeAmount");

    public CrumblingObjectConfig Config;
    public Renderer Renderer;
    public Collider2D Collider;
    public TriggerZone TriggerZone;
    public ParticleSystem CrumbleVFX;

    private readonly List<Collider2D> _overlapResults = new();
    private MaterialPropertyBlock _propertyBlock;

    public float FadeAmount { get; private set; }

    private void Awake()
    {
      _propertyBlock = new MaterialPropertyBlock();
      SetFadeAmount(Config.FullVisibleFadeAmount);
    }

    public void SetFadeAmount(float value)
    {
      FadeAmount = value;
      Renderer.GetPropertyBlock(_propertyBlock);
      _propertyBlock.SetFloat(_sFadeAmountId, value);
      Renderer.SetPropertyBlock(_propertyBlock);
    }

    public bool IsTopSurfaceContact(Collision2D collision)
    {
      for (int i = 0; i < collision.contactCount; i++)
      {
        // Contact normal points from the other collider toward this one.
        // Hitting the top surface means the other object is above → normal ≈ down.
        if (collision.GetContact(i).normal.y < -0.5f)
          return true;
      }

      return false;
    }

    public bool HasBlockingOverlap()
    {
      ContactFilter2D contactFilter = new();
      contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(Collider.gameObject.layer));
      contactFilter.useTriggers = false;

      Physics2D.OverlapCollider(Collider, contactFilter, _overlapResults);

      foreach (Collider2D other in _overlapResults)
      {
        if (other == null || other == Collider)
          continue;

        if (other.transform == transform || other.transform.IsChildOf(transform))
          continue;

        return true;
      }

      return false;
    }
  }
}
