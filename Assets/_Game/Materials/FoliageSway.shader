Shader "Sling/Foliage Sway"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        [MaterialToggle] _ZWrite ("ZWrite", Float) = 0

        _SwayStartPx ("Sway Start Row (px)", Float) = 2
        _SwayEndPx ("Sway Full Row (px)", Float) = 12
        _SwayAmountPx ("Sway Amount (px)", Float) = 2
        _WindSpeed ("Wind Speed", Float) = 1.5
        _WindDirection ("Wind Direction (-1 left, 0 none, 1 right)", Range(-1, 1)) = 0
        _WindPhaseScale ("Wind Phase per World X", Float) = 0.5
        _CellSizePx ("Sprite Cell Size (px)", Float) = 16

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
        float _SwayStartPx;
        float _SwayEndPx;
        float _SwayAmountPx;
        float _WindSpeed;
        float _WindDirection;
        float _WindPhaseScale;
        float _CellSizePx;
    CBUFFER_END

    // Pixel-snapped horizontal shift of the upper rows inside the sprite sheet cell.
    // texSize comes from the bound sprite texture: _MainTex_TexelSize in UnityPerMaterial
    // describes the material's own (empty) _MainTex, not the SpriteRenderer's texture.
    float2 FoliageSwayUV(float2 uv, float2 texSize)
    {
        float2 px = uv * texSize;
        float2 cellMin = floor(px / _CellSizePx) * _CellSizePx;
        float row = floor(px.y - cellMin.y);
        float mask = saturate((row - _SwayStartPx) / max(_SwayEndPx - _SwayStartPx, 1));
        mask *= mask;

        float t = _Time.y * _WindSpeed + unity_ObjectToWorld._m03 * _WindPhaseScale;
        float gust = sin(t) * 0.7 + sin(t * 2.3 + 1.7) * 0.3;
        // |_WindDirection| blends symmetric sway [-1, 1] into one-sided lean [0, sign]:
        // at ±1 plants only bend downwind and gusts swing them between upright and full lean.
        float wind = lerp(gust, sign(_WindDirection) * (gust * 0.5 + 0.5), abs(_WindDirection));
        float offset = round(wind * mask * _SwayAmountPx);

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
