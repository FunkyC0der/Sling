using System.Collections.Generic;
using Aarthificial.PixelGraphics.Common;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Aarthificial.PixelGraphics.Forward
{
    public class VelocityRenderPass : ScriptableRenderPass
    {
        private class PassData
        {
            public TextureHandle velocity;
            public TextureHandle previousVelocity;
            public TextureHandle cameraColor;
            public RendererListHandle layerMaskList;
            public RendererListHandle renderingLayerMaskList;
            public bool drawLayerMask;
            public bool drawRenderingLayerMask;
            public bool drawEmitters;
            public bool preview;
            public Material blitMaterial;
            public Vector4 simulationSize;
            public Vector4 cameraPositionDelta;
            public Vector4 velocitySimulationParams;
            public Vector4 velocitySimulationExtraParams;
            public Vector4 pixelScreenParams;
            public Matrix4x4 viewMatrix;
            public Matrix4x4 projectionMatrix;
        }

        private static readonly int SimulationSizeId = Shader.PropertyToID("_SimulationSize");

        private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();
        private readonly Material _emitterMaterial;
        private readonly Material _blitMaterial;

        private VelocityPassSettings _passSettings;
        private SimulationSettings _simulationSettings;
        private Vector2 _previousPosition;
        private RTHandle _velocityHandle;
        private RTHandle _previousVelocityHandle;

        public VelocityRenderPass(Material emitterMaterial, Material blitMaterial)
        {
            _emitterMaterial = emitterMaterial;
            _blitMaterial = blitMaterial;

            _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIdList.Add(new ShaderTagId("Universal2D"));
            _shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));

            profilingSampler = new ProfilingSampler(nameof(VelocityRenderPass));
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }

        public void Setup(VelocityPassSettings passSettings, SimulationSettings simulationSettings)
        {
            _passSettings = passSettings;
            _simulationSettings = simulationSettings;
        }

        public void Dispose()
        {
            _velocityHandle?.Release();
            _previousVelocityHandle?.Release();
            _velocityHandle = null;
            _previousVelocityHandle = null;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_passSettings == null || _simulationSettings == null || _blitMaterial == null)
                return;

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            int textureWidth = Mathf.FloorToInt(cameraData.camera.pixelWidth * _passSettings.textureScale);
            int textureHeight = Mathf.FloorToInt(cameraData.camera.pixelHeight * _passSettings.textureScale);
            if (textureWidth <= 0 || textureHeight <= 0)
                return;

            var descriptor = new RenderTextureDescriptor(textureWidth, textureHeight)
            {
                graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat,
                depthBufferBits = 0,
                msaaSamples = 1,
                useMipMap = false,
                autoGenerateMips = false
            };

            RenderingUtils.ReAllocateHandleIfNeeded(
                ref _velocityHandle,
                descriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_PG_VelocityTexture");
            RenderingUtils.ReAllocateHandleIfNeeded(
                ref _previousVelocityHandle,
                descriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_PG_PreviousVelocityTexture");

            float height = 2 * cameraData.camera.orthographicSize * _passSettings.pixelsPerUnit;
            float width = height * cameraData.camera.aspect;

            var cameraPosition = (Vector2)cameraData.GetViewMatrix().GetColumn(3);
            var delta = cameraPosition - _previousPosition;
            var screenDelta = cameraData.GetProjectionMatrix() * cameraData.GetViewMatrix() * delta;
            _previousPosition = cameraPosition;

            using (var builder = renderGraph.AddUnsafePass<PassData>(passName, out var passData, profilingSampler))
            {
                passData.velocity = renderGraph.ImportTexture(_velocityHandle);
                passData.previousVelocity = renderGraph.ImportTexture(_previousVelocityHandle);
                passData.cameraColor = resourceData.activeColorTexture;
                passData.blitMaterial = _blitMaterial;
                passData.preview = _passSettings.preview;
                passData.drawEmitters = !cameraData.isPreviewCamera && !cameraData.isSceneViewCamera;
                passData.simulationSize = new Vector4(1.0f / textureWidth, 1.0f / textureHeight, textureWidth, textureHeight);
                passData.cameraPositionDelta = screenDelta / 2;
                passData.velocitySimulationParams = _simulationSettings.Value;
                passData.velocitySimulationExtraParams = _simulationSettings.ExtraParams;
                passData.pixelScreenParams = new Vector4(width, height, _passSettings.pixelsPerUnit, 1 / _passSettings.pixelsPerUnit);
                passData.viewMatrix = cameraData.GetViewMatrix();
                passData.projectionMatrix = cameraData.GetProjectionMatrix();

                passData.drawLayerMask = false;
                passData.drawRenderingLayerMask = false;

                if (passData.drawEmitters)
                {
                    var sortingCriteria = SortingCriteria.CommonTransparent;
                    var drawingSettings = RenderingUtils.CreateDrawingSettings(
                        _shaderTagIdList,
                        renderingData,
                        cameraData,
                        lightData,
                        sortingCriteria);

                    if (_passSettings.layerMask != 0)
                    {
                        var filteringSettings = new FilteringSettings(RenderQueueRange.transparent, _passSettings.layerMask)
                        {
                            renderingLayerMask = uint.MaxValue
                        };
                        drawingSettings.overrideMaterial = null;
                        var param = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                        passData.layerMaskList = renderGraph.CreateRendererList(param);
                        passData.drawLayerMask = true;
                        builder.UseRendererList(passData.layerMaskList);
                    }

                    if (_passSettings.renderingLayerMask.value != 0)
                    {
                        var filteringSettings = new FilteringSettings(RenderQueueRange.transparent, -1)
                        {
                            renderingLayerMask = unchecked((uint)_passSettings.renderingLayerMask.value)
                        };
                        drawingSettings.overrideMaterial = _emitterMaterial;
                        var param = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                        passData.renderingLayerMaskList = renderGraph.CreateRendererList(param);
                        passData.drawRenderingLayerMask = true;
                        builder.UseRendererList(passData.renderingLayerMaskList);
                    }
                }

                builder.UseTexture(passData.velocity, AccessFlags.ReadWrite);
                builder.UseTexture(passData.previousVelocity, AccessFlags.ReadWrite);
                if (passData.preview && passData.cameraColor.IsValid())
                    builder.UseTexture(passData.cameraColor, AccessFlags.Write);

                builder.AllowGlobalStateModification(true);
                builder.AllowPassCulling(false);
                builder.SetGlobalTextureAfterPass(passData.velocity, ShaderIds.VelocityTexture);

                builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) => ExecutePass(data, context));
            }
        }

        private static void ExecutePass(PassData data, UnsafeGraphContext context)
        {
            CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
            RTHandle velocity = data.velocity;
            RTHandle previousVelocity = data.previousVelocity;

            cmd.SetGlobalVector(SimulationSizeId, data.simulationSize);
            cmd.SetGlobalVector(ShaderIds.CameraPositionDelta, data.cameraPositionDelta);
            cmd.SetGlobalTexture(ShaderIds.VelocityTexture, velocity);
            cmd.SetGlobalTexture(ShaderIds.PreviousVelocityTexture, previousVelocity);
            cmd.SetGlobalVector(ShaderIds.VelocitySimulationParams, data.velocitySimulationParams);
            cmd.SetGlobalVector(ShaderIds.VelocitySimulationExtraParams, data.velocitySimulationExtraParams);
            cmd.SetGlobalVector(ShaderIds.PixelScreenParams, data.pixelScreenParams);

            cmd.SetRenderTarget(velocity);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            Blitter.BlitTexture(cmd, new Vector4(1f, 1f, 0f, 0f), data.blitMaterial, 0);

            cmd.SetViewProjectionMatrices(data.viewMatrix, data.projectionMatrix);

            if (data.drawEmitters)
            {
                if (data.drawLayerMask)
                    cmd.DrawRendererList(data.layerMaskList);
                if (data.drawRenderingLayerMask)
                    cmd.DrawRendererList(data.renderingLayerMaskList);
            }

            Blitter.BlitCameraTexture(cmd, velocity, previousVelocity);

#if UNITY_EDITOR
            if (data.preview && data.cameraColor.IsValid())
                Blitter.BlitCameraTexture(cmd, velocity, data.cameraColor);
#endif
        }
    }
}
