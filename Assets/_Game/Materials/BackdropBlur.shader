// Full-screen blur used behind pop-up windows (see BackdropBlurFeature).
// Only Blitter draws are used, so it also works where Graphics.CopyTexture is unsupported (WebGL).
Shader "Hidden/Sling/BackdropBlur"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZTest Always
        ZWrite Off
        Cull Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        // xy: sample step in UV, z: composite opacity.
        float4 _BackdropBlurParams;

        half4 SampleSource(float2 uv)
        {
            return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
        }
        ENDHLSL

        Pass
        {
            Name "Downsample"
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            // 4 bilinear taps = 16-texel box, keeps thin bright details from flickering when shrinking.
            half4 Fragment(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 step = _BackdropBlurParams.xy;
                half4 color = SampleSource(uv + float2(-step.x, -step.y));
                color += SampleSource(uv + float2(step.x, -step.y));
                color += SampleSource(uv + float2(-step.x, step.y));
                color += SampleSource(uv + float2(step.x, step.y));
                return color * 0.25h;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Blur"
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            // Separable 9-tap gaussian folded into 5 bilinear taps; direction comes from _BackdropBlurParams.xy.
            half4 Fragment(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 step = _BackdropBlurParams.xy;
                half4 color = SampleSource(uv) * 0.2270270270h;
                color += SampleSource(uv + step * 1.3846153846) * 0.3162162162h;
                color += SampleSource(uv - step * 1.3846153846) * 0.3162162162h;
                color += SampleSource(uv + step * 3.2307692308) * 0.0702702703h;
                color += SampleSource(uv - step * 3.2307692308) * 0.0702702703h;
                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Composite"
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            half4 Fragment(Varyings input) : SV_Target
            {
                return half4(SampleSource(input.texcoord).rgb, _BackdropBlurParams.z);
            }
            ENDHLSL
        }
    }
}
