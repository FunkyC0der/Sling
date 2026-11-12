Shader "Sling/Foliage Sway"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        [MaterialToggle] _ZWrite ("ZWrite", Float) = 0

        [Header(Shape)]
        _SwayAmountPx ("Sway Amount (px)", Float) = 2
        _CellSizePx ("Sprite Cell Size (px)", Float) = 16

        [Header(Wind)]
        _WindSpeed ("Sway Speed", Float) = 1.5
        _WindDirection ("Wind Direction (-1 left, 0 none, 1 right)", Range(-1, 1)) = 0
        _GustScale ("Gust Size (1 / world units)", Float) = 0.15
        _GustSpeed ("Gust Travel Speed", Float) = 0.4
        _CalmAmount ("Sway in Calm (0..1)", Range(0, 1)) = 0.35
        _TipLag ("Tip Lag (s)", Range(0, 1)) = 0.25
        _Variation ("Per-Plant Variation", Range(0, 1)) = 0.2

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1, 1, 1, 1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1, 1, 1, 1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

    // NOTE: Same layout in every pass, as SRP batcher can not handle different layouts.
    CBUFFER_START(UnityPerMaterial)
        half4 _Color;
        float _SwayAmountPx;
        float _CellSizePx;
        float _WindSpeed;
        float _WindDirection;
        float _GustScale;
        float _GustSpeed;
        float _CalmAmount;
        float _TipLag;
        float _Variation;
    CBUFFER_END

    float FoliageHash11(float p)
    {
        p = frac(p * 0.1031);
        p *= p + 33.33;
        p *= p + p;
        return frac(p);
    }

    float FoliageHash21(float2 p)
    {
        float3 p3 = frac(float3(p.xyx) * 0.1031);
        p3 += dot(p3, p3.yzx + 33.33);
        return frac((p3.x + p3.y) * p3.z);
    }

    // Smooth 1D value noise in [0, 1]; procedural, so no texture tiling seams.
    float FoliageNoise1D(float x)
    {
        float i = floor(x);
        float f = frac(x);
        float u = f * f * (3.0 - 2.0 * f);
        return lerp(FoliageHash11(i), FoliageHash11(i + 1.0), u);
    }

    // Wind in roughly [-1, 1]: gusts are world-space noise travelling downwind, so a gust
    // rolls across the level and plants alternate between calm and strong sway.
    float FoliageWind(float plantX, float t, float phase)
    {
        float travel = _WindDirection != 0 ? sign(_WindDirection) : 1.0;
        float p = plantX * _GustScale - t * _GustSpeed * travel;
        float gust = FoliageNoise1D(p) * 0.65 + FoliageNoise1D(p * 2.3 + 17.0) * 0.35;
        gust = smoothstep(0.2, 0.85, gust);

        float osc = sin(t * _WindSpeed + phase);
        float symmetric = osc * lerp(_CalmAmount, 1.0, gust);
        float oneSided = saturate(lerp(_CalmAmount * 0.5, 1.0, gust) * (0.85 + 0.15 * osc));
        // |_WindDirection| blends symmetric sway into one-sided lean: at ±1 plants only bend downwind.
        return lerp(symmetric, sign(_WindDirection) * oneSided, abs(_WindDirection));
    }

    // Pixel-snapped horizontal shift of the upper rows inside the sprite sheet cell.
    // texSize comes from the bound sprite texture: _MainTex_TexelSize in UnityPerMaterial
    // describes the material's own (empty) _MainTex, not the SpriteRenderer's texture.
    float2 FoliageSwayUV(float2 uv, float2 texSize)
    {
        float2 px = uv * texSize;
        float2 cellMin = floor(px / _CellSizePx) * _CellSizePx;
        float row = floor(px.y - cellMin.y);
        // Linear bend from the bottom row (rooted) to the top row (full sway).
        float mask = saturate(row / max(_CellSizePx - 1, 1));

        float2 plantPos = float2(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13);
        float seed = FoliageHash21(plantPos * 7.31 + cellMin);
        float phase = seed * 6.2831853;
        float strength = 1.0 + (FoliageHash11(seed * 91.7) * 2.0 - 1.0) * _Variation;

        // Upper rows sample slightly older wind, so the tip trails the stem instead of a rigid shear.
        float t = _Time.y - mask * _TipLag;
        float wind = FoliageWind(plantPos.x, t, phase) * strength;
        float offset = round(mask * wind * _SwayAmountPx);

        float x = clamp(floor(px.x) - offset, cellMin.x, cellMin.x + _CellSizePx - 1) + 0.5;
        return float2(x, px.y) / texSize;
    }

    // _MainTex is declared by the per-pass 2D includes, so this expands at the call site.
    #define FOLIAGE_SWAY(uv) \
        { float2 texSize; _MainTex.GetDimensions(texSize.x, texSize.y); uv = FoliageSwayUV(uv, texSize); }
    ENDHLSL

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex LitVertex
            #pragma fragment LitFragment

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color        : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_LIT_OUTPUTS
                half4 color        : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Lit2DCommon.hlsl"

            Varyings LitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonLitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;

                return o;
            }

            half4 LitFragment(Varyings input) : SV_Target
            {
                FOLIAGE_SWAY(input.uv);
                return CommonLitFragment(input, input.color);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering" }

            HLSLPROGRAM
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_NORMALS_INPUTS
                float4 color        : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_NORMALS_OUTPUTS
                half4   color           : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"

            Varyings NormalsRenderingVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonNormalsVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;

                return o;
            }

            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                // Setup instancing for SpriteFlip is used in NormalsRenderingShared
                SetUpSpriteInstanceProperties();

                FOLIAGE_SWAY(input.uv);
                return CommonNormalsFragment(input, input.color);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent" }

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_OUTPUTS
                half4 color : COLOR;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"

            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                FOLIAGE_SWAY(input.uv);
                return CommonUnlitFragment(input, input.color);
            }
            ENDHLSL
        }
    }
}
