using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Sling.Common.Rendering
{
  // Blurs the main camera image while a pop-up with a background is open (BackdropBlur.Intensity > 0).
  // Replaces UI Toolkit backdrop-filter, which relies on Graphics.CopyTexture and renders black on WebGL.
  public class BackdropBlurFeature : ScriptableRendererFeature
  {
    [Serializable]
    public class Settings
    {
      public Shader Shader;

      [Tooltip("Blur texture size relative to the camera image: 1/2^n.")]
      [Range(1, 4)] public int DownsampleSteps = 2;

      [Range(1, 6)] public int Iterations = 2;

      [Tooltip("Gaussian sample spacing in blur-texture texels.")]
      [Range(0.5f, 3f)] public float Spread = 1f;
    }

    [SerializeField] private Settings _settings = new();

    private Material _material;
    private BackdropBlurPass _pass;

    public override void Create()
    {
      _pass = new BackdropBlurPass { renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing };

      CoreUtils.Destroy(_material);
      _material = _settings.Shader != null ? CoreUtils.CreateEngineMaterial(_settings.Shader) : null;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
      if (_material == null || BackdropBlur.Intensity <= 0f)
        return;

      Camera camera = renderingData.cameraData.camera;
      if (renderingData.cameraData.cameraType != CameraType.Game || !camera.CompareTag("MainCamera"))
        return;

      _pass.Setup(_material, _settings, BackdropBlur.Intensity);
      renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing) =>
      CoreUtils.Destroy(_material);

    private class BackdropBlurPass : ScriptableRenderPass
    {
      private const int kDownsamplePass = 0;
      private const int kBlurPass = 1;
      private const int kCompositePass = 2;

      private static readonly int s_ParamsId = Shader.PropertyToID("_BackdropBlurParams");

      private Material _material;
      private Settings _settings;
      private float _intensity;

      private class PassData
      {
        public TextureHandle Source;
        public Material Material;
        public int ShaderPass;
        public Vector4 Params;
      }

      public BackdropBlurPass()
      {
        profilingSampler = new ProfilingSampler("Backdrop Blur");
        // The 2D renderer may otherwise draw straight to the back buffer, which cannot be sampled.
        requiresIntermediateTexture = true;
      }

      public void Setup(Material material, Settings settings, float intensity)
      {
        _material = material;
        _settings = settings;
        _intensity = intensity;
      }

      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        var resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer)
          return;

        TextureHandle cameraColor = resourceData.activeColorTexture;
        TextureDesc cameraDesc = renderGraph.GetTextureDesc(cameraColor);

        int divider = 1 << _settings.DownsampleSteps;
        TextureDesc blurDesc = cameraDesc;
        blurDesc.width = Mathf.Max(1, cameraDesc.width / divider);
        blurDesc.height = Mathf.Max(1, cameraDesc.height / divider);
        blurDesc.msaaSamples = MSAASamples.None;
        blurDesc.depthBufferBits = DepthBits.None;
        blurDesc.useMipMap = false;
        blurDesc.filterMode = FilterMode.Bilinear;
        blurDesc.wrapMode = TextureWrapMode.Clamp;
        blurDesc.clearBuffer = false;
        blurDesc.name = "_BackdropBlurA";
        TextureHandle blurA = renderGraph.CreateTexture(blurDesc);
        blurDesc.name = "_BackdropBlurB";
        TextureHandle blurB = renderGraph.CreateTexture(blurDesc);

        var sourceTexel = new Vector2(1f / cameraDesc.width, 1f / cameraDesc.height);
        var blurTexel = new Vector2(1f / blurDesc.width, 1f / blurDesc.height) * _settings.Spread;

        // Half-texel offsets of the source make each bilinear tap average a 2x2 block.
        AddPass(renderGraph, "Backdrop Blur Downsample", cameraColor, blurA, kDownsamplePass,
          new Vector4(sourceTexel.x * divider * 0.25f, sourceTexel.y * divider * 0.25f, 0f, 0f));

        for (int i = 0; i < _settings.Iterations; i++)
        {
          AddPass(renderGraph, "Backdrop Blur Horizontal", blurA, blurB, kBlurPass,
            new Vector4(blurTexel.x, 0f, 0f, 0f));
          AddPass(renderGraph, "Backdrop Blur Vertical", blurB, blurA, kBlurPass,
            new Vector4(0f, blurTexel.y, 0f, 0f));
        }

        AddPass(renderGraph, "Backdrop Blur Composite", blurA, cameraColor, kCompositePass,
          new Vector4(0f, 0f, _intensity, 0f));
      }

      private void AddPass(
        RenderGraph renderGraph, string passName, TextureHandle source, TextureHandle destination,
        int shaderPass, Vector4 parameters)
      {
        using IRasterRenderGraphBuilder builder =
          renderGraph.AddRasterRenderPass(passName, out PassData passData, profilingSampler);

        passData.Source = source;
        passData.Material = _material;
        passData.ShaderPass = shaderPass;
        passData.Params = parameters;

        builder.UseTexture(source);
        builder.SetRenderAttachment(destination, 0);
        builder.AllowGlobalStateModification(true);

        builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
        {
          context.cmd.SetGlobalVector(s_ParamsId, data.Params);
          Blitter.BlitTexture(context.cmd, data.Source, new Vector4(1f, 1f, 0f, 0f), data.Material, data.ShaderPass);
        });
      }
    }
  }
}
