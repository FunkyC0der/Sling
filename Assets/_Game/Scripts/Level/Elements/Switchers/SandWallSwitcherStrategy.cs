using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Sling.Level.Elements.Switchers
{
  [Serializable]
  public class SandWallSwitcherStrategy : SwitcherStateChangeStrategy
  {
    private static readonly int _sFadeYId = Shader.PropertyToID("_FadeY");
    private static readonly int _sInvertId = Shader.PropertyToID("_Invert");

    public SpriteRenderer MainSpriteRenderer;
    public BoxCollider2D Collider;
    public List<SpriteRenderer> AdditionalSpriteRenderers = new();
    public Material FadeMaterial;
    [Min(0f)] public float EdgePaddingY;
    public TweenSettings TweenSettings = new(0.5f, Ease.InOutSine);

    private readonly List<RendererState> _rendererStates = new();

    private Tween _tween;
    private ColliderGeometry _colliderGeometry;
    private float _fadeTopY;
    private float _fadeBottomY;
    private bool _isAppearing;

    public override void Stop() =>
      StopTransition();

    public override IEnumerator SetState(bool isOn, bool immediate)
    {
      StopTransition();

      if (immediate || !CanAnimate())
      {
        ApplyImmediateState(isOn);
        yield break;
      }

      CaptureGeometry();
      CaptureAndApplyFadeMaterial();

      _isAppearing = isOn;
      ApplyTransitionValue(0f);

      _tween = Tween.Custom(
        this,
        0f,
        1f,
        TweenSettings,
        (strategy, value) => strategy.ApplyTransitionValue(value));

      while (_tween.isAlive)
        yield return null;

      CompleteTransition(isOn);
    }

    private bool CanAnimate() =>
      MainSpriteRenderer != null &&
      Collider != null &&
      FadeMaterial != null;

    private void CaptureGeometry()
    {
      Bounds bounds = MainSpriteRenderer.bounds;
      float padding = Mathf.Max(0f, EdgePaddingY);

      _fadeTopY = bounds.max.y + padding;
      _fadeBottomY = bounds.min.y - padding;
      _colliderGeometry = CalculateColliderGeometry(bounds);
    }

    private ColliderGeometry CalculateColliderGeometry(Bounds rendererBounds)
    {
      Transform colliderTransform = Collider.transform;
      float minX = float.PositiveInfinity;
      float maxX = float.NegativeInfinity;
      float minY = float.PositiveInfinity;
      float maxY = float.NegativeInfinity;

      IncludePoint(rendererBounds.min.x, rendererBounds.min.y);
      IncludePoint(rendererBounds.min.x, rendererBounds.max.y);
      IncludePoint(rendererBounds.max.x, rendererBounds.min.y);
      IncludePoint(rendererBounds.max.x, rendererBounds.max.y);

      return new ColliderGeometry(minX, maxX, minY, maxY);

      void IncludePoint(float worldX, float worldY)
      {
        Vector3 localPoint = colliderTransform.InverseTransformPoint(
          new Vector3(worldX, worldY, colliderTransform.position.z));
        minX = Mathf.Min(minX, localPoint.x);
        maxX = Mathf.Max(maxX, localPoint.x);
        minY = Mathf.Min(minY, localPoint.y);
        maxY = Mathf.Max(maxY, localPoint.y);
      }
    }

    private void CaptureAndApplyFadeMaterial()
    {
      AddRendererState(MainSpriteRenderer);

      foreach (SpriteRenderer spriteRenderer in AdditionalSpriteRenderers)
        AddRendererState(spriteRenderer);

      foreach (RendererState state in _rendererStates)
      {
        state.Renderer.enabled = true;
        state.Renderer.sharedMaterial = FadeMaterial;
      }
    }

    private void AddRendererState(SpriteRenderer spriteRenderer)
    {
      if (spriteRenderer == null || ContainsRenderer(spriteRenderer))
        return;

      MaterialPropertyBlock originalPropertyBlock = new();
      bool hadPropertyBlock = spriteRenderer.HasPropertyBlock();
      spriteRenderer.GetPropertyBlock(originalPropertyBlock);

      MaterialPropertyBlock fadePropertyBlock = new();
      spriteRenderer.GetPropertyBlock(fadePropertyBlock);

      _rendererStates.Add(new RendererState(
        spriteRenderer,
        spriteRenderer.sharedMaterial,
        hadPropertyBlock,
        originalPropertyBlock,
        fadePropertyBlock));
    }

    private bool ContainsRenderer(SpriteRenderer spriteRenderer)
    {
      foreach (RendererState state in _rendererStates)
      {
        if (state.Renderer == spriteRenderer)
          return true;
      }

      return false;
    }

    private void ApplyTransitionValue(float value)
    {
      float progress = Mathf.Clamp01(value);
      float fadeY = Mathf.Lerp(_fadeTopY, _fadeBottomY, progress);

      foreach (RendererState state in _rendererStates)
      {
        if (state.Renderer == null)
          continue;

        state.FadePropertyBlock.SetFloat(_sFadeYId, fadeY);
        state.FadePropertyBlock.SetFloat(_sInvertId, _isAppearing ? 1f : 0f);
        state.Renderer.SetPropertyBlock(state.FadePropertyBlock);
      }

      ApplyColliderProgress(progress);
    }

    private void ApplyColliderProgress(float progress)
    {
      float minY;
      float maxY;

      if (_isAppearing)
      {
        minY = Mathf.Lerp(_colliderGeometry.MaxY, _colliderGeometry.MinY, progress);
        maxY = _colliderGeometry.MaxY;
      }
      else
      {
        minY = _colliderGeometry.MinY;
        maxY = Mathf.Lerp(_colliderGeometry.MaxY, _colliderGeometry.MinY, progress);
      }

      float height = Mathf.Max(0f, maxY - minY);
      Collider.size = new Vector2(_colliderGeometry.Width, height);
      Collider.offset = new Vector2(_colliderGeometry.CenterX, (minY + maxY) * 0.5f);
      Collider.enabled = height > Mathf.Epsilon;
    }

    private void CompleteTransition(bool isOn)
    {
      if (isOn)
        ApplyFullCollider();
      else
        ApplyHiddenCollider();

      RestoreRendererStates(isOn);
      _tween = default;
    }

    private void ApplyImmediateState(bool isOn)
    {
      SetRenderersEnabled(isOn);

      if (MainSpriteRenderer == null || Collider == null)
        return;

      _colliderGeometry = CalculateColliderGeometry(MainSpriteRenderer.bounds);

      if (isOn)
        ApplyFullCollider();
      else
        ApplyHiddenCollider();
    }

    private void ApplyFullCollider()
    {
      Collider.size = new Vector2(_colliderGeometry.Width, _colliderGeometry.Height);
      Collider.offset = new Vector2(_colliderGeometry.CenterX, _colliderGeometry.CenterY);
      Collider.enabled = true;
    }

    private void ApplyHiddenCollider()
    {
      Collider.size = new Vector2(_colliderGeometry.Width, 0f);
      Collider.offset = new Vector2(_colliderGeometry.CenterX, _colliderGeometry.MinY);
      Collider.enabled = false;
    }

    private void SetRenderersEnabled(bool isEnabled)
    {
      SetRendererEnabled(MainSpriteRenderer, isEnabled);

      foreach (SpriteRenderer spriteRenderer in AdditionalSpriteRenderers)
        SetRendererEnabled(spriteRenderer, isEnabled);
    }

    private static void SetRendererEnabled(SpriteRenderer spriteRenderer, bool isEnabled)
    {
      if (spriteRenderer != null)
        spriteRenderer.enabled = isEnabled;
    }

    private void StopTransition()
    {
      if (_tween.isAlive)
        _tween.Stop();

      RestoreRendererStates();
      _tween = default;
    }

    private void RestoreRendererStates(bool? isEnabled = null)
    {
      foreach (RendererState state in _rendererStates)
      {
        if (state.Renderer == null)
          continue;

        state.Renderer.sharedMaterial = state.OriginalMaterial;
        state.Renderer.SetPropertyBlock(
          state.HadPropertyBlock ? state.OriginalPropertyBlock : null);

        if (isEnabled.HasValue)
          state.Renderer.enabled = isEnabled.Value;
      }

      _rendererStates.Clear();
    }

    private sealed class RendererState
    {
      public RendererState(
        SpriteRenderer renderer,
        Material originalMaterial,
        bool hadPropertyBlock,
        MaterialPropertyBlock originalPropertyBlock,
        MaterialPropertyBlock fadePropertyBlock)
      {
        Renderer = renderer;
        OriginalMaterial = originalMaterial;
        HadPropertyBlock = hadPropertyBlock;
        OriginalPropertyBlock = originalPropertyBlock;
        FadePropertyBlock = fadePropertyBlock;
      }

      public SpriteRenderer Renderer { get; }
      public Material OriginalMaterial { get; }
      public bool HadPropertyBlock { get; }
      public MaterialPropertyBlock OriginalPropertyBlock { get; }
      public MaterialPropertyBlock FadePropertyBlock { get; }
    }

    private readonly struct ColliderGeometry
    {
      public ColliderGeometry(float minX, float maxX, float minY, float maxY)
      {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
      }

      public float MinX { get; }
      public float MaxX { get; }
      public float MinY { get; }
      public float MaxY { get; }
      public float Width => MaxX - MinX;
      public float Height => MaxY - MinY;
      public float CenterX => (MinX + MaxX) * 0.5f;
      public float CenterY => (MinY + MaxY) * 0.5f;
    }
  }
}
