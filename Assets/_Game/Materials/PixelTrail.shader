Shader "Sling/Pixel Trail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _PixelsPerUnit ("Pixels Per Unit", Float) = 4
        _WidthPadding ("Width Padding (match trail width multiplier)", Float) = 3
        _MinWidthPx ("Min Width (px)", Float) = 1
        _AlphaSteps ("Alpha Steps (0 = off)", Float) = 0

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1, 1, 1, 1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    // NOTE: Same layout in every pass, as SRP batcher can not handle different layouts.
    CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half4 _Color;
        float _PixelsPerUnit;
        float _WidthPadding;
        float _MinWidthPx;
        float _AlphaSteps;
    CBUFFER_END

    struct Attributes
    {
        float3 positionOS : POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float4 color : COLOR;
        float2 uv : TEXCOORD0;
        float2 positionWS : TEXCOORD1;
    };

    Varyings PixelTrailVertex(Attributes input)
    {
        Varyings o;
        float3 positionWS = TransformObjectToWorld(input.positionOS);
        o.positionCS = TransformWorldToHClip(positionWS);
        o.positionWS = positionWS.xy;
        o.uv = input.uv;
        o.color = input.color;
        return o;
    }

    // Every screen fragment inside one art pixel re-evaluates the trail at that pixel's world-space
    // center, so the ribbon resolves into whole squares on the 1/_PixelsPerUnit grid.
    // Trail uv: x runs along the trail, y runs across it (0..1). The mesh is _WidthPadding times wider
    // than the visible line, so edge art pixels are fully covered by fragments and never come out as slivers.
    half4 PixelTrailFragment(Varyings input) : SV_Target
    {
        float2 posWS = input.positionWS;
        float2 cellCenter = (floor(posWS * _PixelsPerUnit) + 0.5) / _PixelsPerUnit;

        // Columns are d(world)/d(screen x) and d(world)/d(screen y); linear inside one triangle.
        float2 dWdx = ddx(posWS);
        float2 dWdy = ddy(posWS);
        float2 duvdx = ddx(input.uv);
        float2 duvdy = ddy(input.uv);
        float4 dColordx = ddx(input.color);
        float4 dColordy = ddy(input.color);
        float det = dWdx.x * dWdy.y - dWdy.x * dWdx.y;

        float2 uv = input.uv;
        float4 color = input.color;
        float uvYGradWS = 0;
        // Half-range of uv across one art pixel square; 0 when derivatives are unusable.
        float2 uvCellExtent = 0;

        if (abs(det) > 1e-12)
        {
            // inverse(d(world)/d(screen)) = d(screen)/d(world)
            float2x2 screenPerWorld = float2x2(dWdy.y, -dWdy.x, -dWdx.y, dWdx.x) / det;
            float2 deltaSS = mul(screenPerWorld, cellCenter - posWS);

            uv += duvdx * deltaSS.x + duvdy * deltaSS.y;
            color += dColordx * deltaSS.x + dColordy * deltaSS.y;

            // d(uv)/d(world) = d(uv)/d(screen) · d(screen)/d(world)
            float2 uvXGrad = mul(float2(duvdx.x, duvdy.x), screenPerWorld);
            float2 uvYGrad = mul(float2(duvdx.y, duvdy.y), screenPerWorld);
            uvYGradWS = length(uvYGrad);

            float cellHalf = 0.5 / _PixelsPerUnit;
            uvCellExtent = cellHalf * float2(abs(uvXGrad.x) + abs(uvXGrad.y), abs(uvYGrad.x) + abs(uvYGrad.y));
        }

        // Keep only art pixels whose whole square lies inside the mesh: a partially covered one
        // would get fragments over just part of its area and show up as a sliver.
        clip(uv.x - uvCellExtent.x);
        clip(1 - uv.x - uvCellExtent.x);
        clip(0.5 - abs(uv.y - 0.5) - uvCellExtent.y);

        // Across-trail coverage in world units, with the padding divided back out.
        float invGrad = 1 / max(uvYGradWS, 1e-6);
        float geomHalfWidth = 0.5 * invGrad;
        float halfWidth = max(geomHalfWidth / _WidthPadding, _MinWidthPx * 0.5 / _PixelsPerUnit);
        float distWS = abs(uv.y - 0.5) * invGrad;
        if (uvYGradWS > 0)
            clip(halfWidth - distWS);

        // Remap uv.y from the padded mesh onto the visible band so _MainTex spans the drawn line.
        float2 texUV = float2(uv.x, 0.5 + (uv.y - 0.5) * geomHalfWidth / halfWidth);
        texUV = saturate(texUV) * _MainTex_ST.xy + _MainTex_ST.zw;

        half4 c = saturate(color) * _Color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, texUV);
        if (_AlphaSteps > 0)
        {
            c.a = round(c.a * _AlphaSteps) / _AlphaSteps;
            clip(c.a - 0.0001);
        }
        return c;
    }
    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "PreviewType" = "Plane" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex PixelTrailVertex
            #pragma fragment PixelTrailFragment
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent" }

            HLSLPROGRAM
            #pragma vertex PixelTrailVertex
            #pragma fragment PixelTrailFragment
            ENDHLSL
        }
    }
}
