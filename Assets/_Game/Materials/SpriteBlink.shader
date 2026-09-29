Shader "Sling/Sprite Blink"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlinkAmount ("Blink Amount", Range(0, 1)) = 0
        _BlinkColor ("Blink Color", Color) = (1, 1, 1, 1)
        [MaterialToggle] _ZWrite("ZWrite", Float) = 0
        [Toggle(_BLINK_MESH_RENDERER)] _MeshRenderer ("Mesh Renderer", Float) = 0

        [HideInInspector] _Color ("Tint", Color) = (1, 1, 1, 1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1, 1, 1, 1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

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

            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE
            #pragma shader_feature_local _ _BLINK_MESH_RENDERER

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _BlinkColor;
                half _BlinkAmount;
            CBUFFER_END

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
            #if !defined(_BLINK_MESH_RENDERER)
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
            #endif

                Varyings output = CommonUnlitVertex(input);
            #if defined(_BLINK_MESH_RENDERER)
                // MeshRenderer leaves unity_SpriteProps/unity_SpriteColor zeroed.
                output.color = input.color * _Color;
            #else
                output.color = input.color * _Color * unity_SpriteColor;
            #endif
                return output;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                half4 spriteColor = CommonUnlitFragment(input, input.color);
                spriteColor.rgb = lerp(spriteColor.rgb, _BlinkColor.rgb, saturate(_BlinkAmount));
                return spriteColor;
            }
            ENDHLSL
        }
    }
}
