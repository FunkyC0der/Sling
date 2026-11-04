Shader "Sling/Scrolling Dots"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _ScrollSpeed ("Scroll Speed (tiles/sec)", Float) = 1.5
        _ScrollPixelSteps ("Scroll Steps Per Tile (0 = smooth)", Float) = 16

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
        float _ScrollSpeed;
        float _ScrollPixelSteps;
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
    };

    Varyings ScrollingDotsVertex(Attributes input)
    {
        Varyings o;
        o.positionCS = TransformObjectToHClip(input.positionOS);
        o.uv = input.uv;
        o.color = input.color;
        return o;
    }

    // Line uv.x grows from the line start to its end (LineRenderer Tile mode), so subtracting
    // a time-based offset moves the repeating dot pattern towards the end of the line.
    half4 ScrollingDotsFragment(Varyings input) : SV_Target
    {
        float offset = _Time.y * _ScrollSpeed;
        if (_ScrollPixelSteps > 0)
            offset = floor(offset * _ScrollPixelSteps) / _ScrollPixelSteps;

        float2 uv = float2(input.uv.x - frac(offset), input.uv.y);
        uv = uv * _MainTex_ST.xy + _MainTex_ST.zw;

        return input.color * _Color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
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
            #pragma vertex ScrollingDotsVertex
            #pragma fragment ScrollingDotsFragment
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "Transparent" "RenderType" = "Transparent" }

            HLSLPROGRAM
            #pragma vertex ScrollingDotsVertex
            #pragma fragment ScrollingDotsFragment
            ENDHLSL
        }
    }
}
