Shader "Exported/DisabledBatching_PixelGraphics/Foliage/foliage_pixelart_shader"
{
    Properties
    {
        [HideInInspector]_SBPBreaker("SBPBreaker", Float) = 0
        [NoScaleOffset]_MainTex("Main Tex", 2D) = "white" {}
        Vector1_e19a6f73e9824493998ba7bebae9c03c("Velocity Strength", Float) = 1
        Vector2_2ad9dffd23234809bbb6d55338af2214("Wind Velocity", Vector, 2) = (1, 0, 0, 0)
        Vector1_2d61041f8dfd46289cb8aafd27290417("Wind Strength", Float) = 1
        Vector1_620eb0a4fc1a48c79d9daecb584075d4("Wind Scale", Float) = 1
        _MainTex_TexelSize("MainTex TexelSize", Vector, 4) = (0, 0, 0, 0)
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteLitSubTarget"
        }
        Pass
        {
            Name "Sprite Lit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZWrite Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
float _SBPBreaker;
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_0
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_1
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_2
        #pragma multi_compile _ USE_SHAPE_LIGHT_TYPE_3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_vertex _ SKINNED_SPRITE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_SCREENPOSITION
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITELIT
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
             float4 screenPosition;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 ObjectSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float4 screenPosition : INTERP2;
             float3 positionWS : INTERP3;
             float3 normalWS : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color + (_SBPBreaker);
            output.screenPosition.xyzw = input.screenPosition;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.screenPosition = input.screenPosition.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float Vector1_e19a6f73e9824493998ba7bebae9c03c;
        float2 Vector2_2ad9dffd23234809bbb6d55338af2214;
        float Vector1_2d61041f8dfd46289cb8aafd27290417;
        float Vector1_620eb0a4fc1a48c79d9daecb584075d4;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_PG_VelocityTexture);
        SAMPLER(sampler_PG_VelocityTexture);
        float4 _PG_VelocityTexture_TexelSize;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DisplacementMask);
        SAMPLER(sampler_DisplacementMask);
        float4 _DisplacementMask_TexelSize;
        float4 _PG_PixelScreenParams;
        
        // Graph Includes
        #include_with_pragmas "Assets/Plugins/pixelgraphics-master/Runtime/Shaders/SimplexNoise3D.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Fraction_float2(float2 In, out float2 Out)
        {
            Out = frac(In);
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Posterize_float2(float2 In, float2 Steps, out float2 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float4 SpriteMask;
            float3 NormalTS;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_65a481774e39462988c1629da3193035_Out_0_Vector4 = IN.uv0;
            UnityTexture2D _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_PG_VelocityTexture);
            float4 _ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_B_3_Float = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_A_4_Float = 0;
            float2 _Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2 = float2(_Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float, _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float);
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float = _WorldSpaceCameraPos[0];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float = _WorldSpaceCameraPos[1];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_B_3_Float = _WorldSpaceCameraPos[2];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_A_4_Float = 0;
            float2 _Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2 = float2(_Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float, _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float);
            float2 _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2 = float2(unity_OrthoParams.x, unity_OrthoParams.y);
            float2 _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2, _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2);
            float2 _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2, _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2);
            float4 _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_f5db6e799ed8488dad63158be533559a_R_1_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[0];
            float _Split_f5db6e799ed8488dad63158be533559a_G_2_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[1];
            float _Split_f5db6e799ed8488dad63158be533559a_B_3_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[2];
            float _Split_f5db6e799ed8488dad63158be533559a_A_4_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[3];
            float2 _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2, (_Split_f5db6e799ed8488dad63158be533559a_B_3_Float.xx), _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2);
            float2 _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2;
            Unity_Fraction_float2(_Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2, _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2);
            float3 _Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3 = float3(_Split_f5db6e799ed8488dad63158be533559a_R_1_Float, _Split_f5db6e799ed8488dad63158be533559a_G_2_Float, float(1));
            float2 _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2;
            Unity_Divide_float2(_Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2, (_Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2);
            float2 _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2;
            Unity_Subtract_float2((_ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2);
            float2 _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2;
            Unity_Posterize_float2(_Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2, float2(160, 90), _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2);
            float2 _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2;
            Unity_Add_float2(_Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2, _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2);
            float4 _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.tex, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.samplerstate, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.GetTransformedUV(_Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2) );
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.r;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.g;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_B_6_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.b;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_A_7_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.a;
            float2 _Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2 = float2(_SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float, _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float);
            float _Property_996b03b9050749a6959e97ba632841f3_Out_0_Float = Vector1_e19a6f73e9824493998ba7bebae9c03c;
            float2 _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2, (_Property_996b03b9050749a6959e97ba632841f3_Out_0_Float.xx), _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2);
            UnityTexture2D _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_DisplacementMask);
            float4 _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.tex, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.samplerstate, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.r;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.g;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_B_6_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.b;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_A_7_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.a;
            float2 _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2, (_SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float.xx), _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2);
            float _Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float = Vector1_2d61041f8dfd46289cb8aafd27290417;
            float _Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_3ce07f9210e244a39211fad1725c19c8_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_3ce07f9210e244a39211fad1725c19c8_A_4_Float = 0;
            float2 _Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2 = float2(_Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float, _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float);
            float4 _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_485e13f9e0954088870fd2739a33e10e_R_1_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[0];
            float _Split_485e13f9e0954088870fd2739a33e10e_G_2_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[1];
            float _Split_485e13f9e0954088870fd2739a33e10e_B_3_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[2];
            float _Split_485e13f9e0954088870fd2739a33e10e_A_4_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[3];
            float2 _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2;
            Unity_Posterize_float2(_Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2, (_Split_485e13f9e0954088870fd2739a33e10e_B_3_Float.xx), _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2);
            float2 _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2 = Vector2_2ad9dffd23234809bbb6d55338af2214;
            float _Split_1ba6754f600a46739168a78f05ae4938_R_1_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[0];
            float _Split_1ba6754f600a46739168a78f05ae4938_G_2_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[1];
            float _Split_1ba6754f600a46739168a78f05ae4938_B_3_Float = 0;
            float _Split_1ba6754f600a46739168a78f05ae4938_A_4_Float = 0;
            float _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_R_1_Float, IN.TimeParameters.x, _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float);
            float _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_G_2_Float, IN.TimeParameters.x, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2 = float2(_Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2;
            Unity_Add_float2(_Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2, _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2, _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2);
            float2 _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2;
            Unity_Add_float2(_Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2, (SHADERGRAPH_OBJECT_POSITION.xy), _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2);
            float _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float = Vector1_620eb0a4fc1a48c79d9daecb584075d4;
            float _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3;
            SimplexNoise3D_float((float3(_Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2, 0.0)), _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3);
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[0];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[1];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_B_3_Float = 0;
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_A_4_Float = 0;
            float3 _Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3 = float3(_Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float, _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float, float(10));
            float _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3;
            SimplexNoise3D_float(_Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3, _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3);
            float2 _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2 = float2(_SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float);
            float2 _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float.xx), _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2, _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2);
            float2 _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2;
            Unity_Multiply_float2_float2((_SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float.xx), _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2);
            float2 _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2;
            Unity_Add_float2(_Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2, _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2);
            float4 _Property_126da754180244e88cb50700b5928505_Out_0_Vector4 = _MainTex_TexelSize;
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_R_1_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[0];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_G_2_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[1];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[2];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[3];
            float2 _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2 = float2(_Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float, _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float);
            float2 _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2;
            Unity_Divide_float2(_Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2);
            float2 _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2;
            Unity_Posterize_float2(_Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2);
            float2 _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2;
            Unity_Subtract_float2((_UV_65a481774e39462988c1629da3193035_Out_0_Vector4.xy), _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2, _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2);
            float4 _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.tex, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.samplerstate, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.GetTransformedUV(_Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2) );
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.a;
            float3 _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3 = float3(_SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float);
            surface.BaseColor = _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3;
            surface.SpriteMask = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteLitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Normal"
            Tags
            {
                "LightMode" = "NormalsRendering"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZWrite Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
float _SBPBreaker;
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ SKINNED_SPRITE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITENORMAL
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 ObjectSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 tangentWS : INTERP0;
             float4 texCoord0 : INTERP1;
             float4 color : INTERP2;
             float3 positionWS : INTERP3;
             float3 normalWS : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color + (_SBPBreaker);
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float Vector1_e19a6f73e9824493998ba7bebae9c03c;
        float2 Vector2_2ad9dffd23234809bbb6d55338af2214;
        float Vector1_2d61041f8dfd46289cb8aafd27290417;
        float Vector1_620eb0a4fc1a48c79d9daecb584075d4;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_PG_VelocityTexture);
        SAMPLER(sampler_PG_VelocityTexture);
        float4 _PG_VelocityTexture_TexelSize;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DisplacementMask);
        SAMPLER(sampler_DisplacementMask);
        float4 _DisplacementMask_TexelSize;
        float4 _PG_PixelScreenParams;
        
        // Graph Includes
        #include_with_pragmas "Assets/Plugins/pixelgraphics-master/Runtime/Shaders/SimplexNoise3D.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Fraction_float2(float2 In, out float2 Out)
        {
            Out = frac(In);
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Posterize_float2(float2 In, float2 Steps, out float2 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_65a481774e39462988c1629da3193035_Out_0_Vector4 = IN.uv0;
            UnityTexture2D _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_PG_VelocityTexture);
            float4 _ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_B_3_Float = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_A_4_Float = 0;
            float2 _Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2 = float2(_Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float, _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float);
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float = _WorldSpaceCameraPos[0];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float = _WorldSpaceCameraPos[1];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_B_3_Float = _WorldSpaceCameraPos[2];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_A_4_Float = 0;
            float2 _Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2 = float2(_Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float, _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float);
            float2 _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2 = float2(unity_OrthoParams.x, unity_OrthoParams.y);
            float2 _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2, _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2);
            float2 _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2, _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2);
            float4 _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_f5db6e799ed8488dad63158be533559a_R_1_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[0];
            float _Split_f5db6e799ed8488dad63158be533559a_G_2_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[1];
            float _Split_f5db6e799ed8488dad63158be533559a_B_3_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[2];
            float _Split_f5db6e799ed8488dad63158be533559a_A_4_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[3];
            float2 _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2, (_Split_f5db6e799ed8488dad63158be533559a_B_3_Float.xx), _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2);
            float2 _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2;
            Unity_Fraction_float2(_Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2, _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2);
            float3 _Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3 = float3(_Split_f5db6e799ed8488dad63158be533559a_R_1_Float, _Split_f5db6e799ed8488dad63158be533559a_G_2_Float, float(1));
            float2 _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2;
            Unity_Divide_float2(_Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2, (_Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2);
            float2 _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2;
            Unity_Subtract_float2((_ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2);
            float2 _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2;
            Unity_Posterize_float2(_Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2, float2(160, 90), _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2);
            float2 _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2;
            Unity_Add_float2(_Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2, _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2);
            float4 _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.tex, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.samplerstate, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.GetTransformedUV(_Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2) );
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.r;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.g;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_B_6_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.b;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_A_7_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.a;
            float2 _Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2 = float2(_SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float, _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float);
            float _Property_996b03b9050749a6959e97ba632841f3_Out_0_Float = Vector1_e19a6f73e9824493998ba7bebae9c03c;
            float2 _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2, (_Property_996b03b9050749a6959e97ba632841f3_Out_0_Float.xx), _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2);
            UnityTexture2D _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_DisplacementMask);
            float4 _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.tex, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.samplerstate, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.r;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.g;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_B_6_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.b;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_A_7_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.a;
            float2 _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2, (_SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float.xx), _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2);
            float _Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float = Vector1_2d61041f8dfd46289cb8aafd27290417;
            float _Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_3ce07f9210e244a39211fad1725c19c8_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_3ce07f9210e244a39211fad1725c19c8_A_4_Float = 0;
            float2 _Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2 = float2(_Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float, _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float);
            float4 _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_485e13f9e0954088870fd2739a33e10e_R_1_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[0];
            float _Split_485e13f9e0954088870fd2739a33e10e_G_2_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[1];
            float _Split_485e13f9e0954088870fd2739a33e10e_B_3_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[2];
            float _Split_485e13f9e0954088870fd2739a33e10e_A_4_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[3];
            float2 _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2;
            Unity_Posterize_float2(_Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2, (_Split_485e13f9e0954088870fd2739a33e10e_B_3_Float.xx), _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2);
            float2 _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2 = Vector2_2ad9dffd23234809bbb6d55338af2214;
            float _Split_1ba6754f600a46739168a78f05ae4938_R_1_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[0];
            float _Split_1ba6754f600a46739168a78f05ae4938_G_2_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[1];
            float _Split_1ba6754f600a46739168a78f05ae4938_B_3_Float = 0;
            float _Split_1ba6754f600a46739168a78f05ae4938_A_4_Float = 0;
            float _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_R_1_Float, IN.TimeParameters.x, _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float);
            float _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_G_2_Float, IN.TimeParameters.x, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2 = float2(_Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2;
            Unity_Add_float2(_Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2, _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2, _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2);
            float2 _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2;
            Unity_Add_float2(_Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2, (SHADERGRAPH_OBJECT_POSITION.xy), _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2);
            float _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float = Vector1_620eb0a4fc1a48c79d9daecb584075d4;
            float _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3;
            SimplexNoise3D_float((float3(_Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2, 0.0)), _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3);
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[0];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[1];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_B_3_Float = 0;
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_A_4_Float = 0;
            float3 _Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3 = float3(_Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float, _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float, float(10));
            float _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3;
            SimplexNoise3D_float(_Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3, _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3);
            float2 _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2 = float2(_SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float);
            float2 _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float.xx), _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2, _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2);
            float2 _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2;
            Unity_Multiply_float2_float2((_SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float.xx), _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2);
            float2 _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2;
            Unity_Add_float2(_Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2, _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2);
            float4 _Property_126da754180244e88cb50700b5928505_Out_0_Vector4 = _MainTex_TexelSize;
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_R_1_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[0];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_G_2_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[1];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[2];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[3];
            float2 _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2 = float2(_Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float, _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float);
            float2 _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2;
            Unity_Divide_float2(_Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2);
            float2 _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2;
            Unity_Posterize_float2(_Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2);
            float2 _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2;
            Unity_Subtract_float2((_UV_65a481774e39462988c1629da3193035_Out_0_Vector4.xy), _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2, _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2);
            float4 _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.tex, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.samplerstate, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.GetTransformedUV(_Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2) );
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.a;
            float3 _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3 = float3(_SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float);
            surface.BaseColor = _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3;
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteNormalPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
float _SBPBreaker;
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float Vector1_e19a6f73e9824493998ba7bebae9c03c;
        float2 Vector2_2ad9dffd23234809bbb6d55338af2214;
        float Vector1_2d61041f8dfd46289cb8aafd27290417;
        float Vector1_620eb0a4fc1a48c79d9daecb584075d4;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_PG_VelocityTexture);
        SAMPLER(sampler_PG_VelocityTexture);
        float4 _PG_VelocityTexture_TexelSize;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DisplacementMask);
        SAMPLER(sampler_DisplacementMask);
        float4 _DisplacementMask_TexelSize;
        float4 _PG_PixelScreenParams;
        
        // Graph Includes
        #include_with_pragmas "Assets/Plugins/pixelgraphics-master/Runtime/Shaders/SimplexNoise3D.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Fraction_float2(float2 In, out float2 Out)
        {
            Out = frac(In);
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Posterize_float2(float2 In, float2 Steps, out float2 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_65a481774e39462988c1629da3193035_Out_0_Vector4 = IN.uv0;
            UnityTexture2D _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_PG_VelocityTexture);
            float4 _ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_B_3_Float = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_A_4_Float = 0;
            float2 _Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2 = float2(_Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float, _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float);
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float = _WorldSpaceCameraPos[0];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float = _WorldSpaceCameraPos[1];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_B_3_Float = _WorldSpaceCameraPos[2];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_A_4_Float = 0;
            float2 _Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2 = float2(_Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float, _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float);
            float2 _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2 = float2(unity_OrthoParams.x, unity_OrthoParams.y);
            float2 _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2, _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2);
            float2 _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2, _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2);
            float4 _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_f5db6e799ed8488dad63158be533559a_R_1_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[0];
            float _Split_f5db6e799ed8488dad63158be533559a_G_2_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[1];
            float _Split_f5db6e799ed8488dad63158be533559a_B_3_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[2];
            float _Split_f5db6e799ed8488dad63158be533559a_A_4_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[3];
            float2 _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2, (_Split_f5db6e799ed8488dad63158be533559a_B_3_Float.xx), _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2);
            float2 _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2;
            Unity_Fraction_float2(_Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2, _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2);
            float3 _Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3 = float3(_Split_f5db6e799ed8488dad63158be533559a_R_1_Float, _Split_f5db6e799ed8488dad63158be533559a_G_2_Float, float(1));
            float2 _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2;
            Unity_Divide_float2(_Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2, (_Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2);
            float2 _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2;
            Unity_Subtract_float2((_ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2);
            float2 _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2;
            Unity_Posterize_float2(_Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2, float2(160, 90), _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2);
            float2 _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2;
            Unity_Add_float2(_Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2, _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2);
            float4 _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.tex, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.samplerstate, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.GetTransformedUV(_Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2) );
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.r;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.g;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_B_6_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.b;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_A_7_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.a;
            float2 _Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2 = float2(_SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float, _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float);
            float _Property_996b03b9050749a6959e97ba632841f3_Out_0_Float = Vector1_e19a6f73e9824493998ba7bebae9c03c;
            float2 _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2, (_Property_996b03b9050749a6959e97ba632841f3_Out_0_Float.xx), _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2);
            UnityTexture2D _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_DisplacementMask);
            float4 _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.tex, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.samplerstate, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.r;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.g;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_B_6_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.b;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_A_7_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.a;
            float2 _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2, (_SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float.xx), _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2);
            float _Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float = Vector1_2d61041f8dfd46289cb8aafd27290417;
            float _Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_3ce07f9210e244a39211fad1725c19c8_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_3ce07f9210e244a39211fad1725c19c8_A_4_Float = 0;
            float2 _Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2 = float2(_Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float, _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float);
            float4 _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_485e13f9e0954088870fd2739a33e10e_R_1_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[0];
            float _Split_485e13f9e0954088870fd2739a33e10e_G_2_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[1];
            float _Split_485e13f9e0954088870fd2739a33e10e_B_3_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[2];
            float _Split_485e13f9e0954088870fd2739a33e10e_A_4_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[3];
            float2 _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2;
            Unity_Posterize_float2(_Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2, (_Split_485e13f9e0954088870fd2739a33e10e_B_3_Float.xx), _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2);
            float2 _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2 = Vector2_2ad9dffd23234809bbb6d55338af2214;
            float _Split_1ba6754f600a46739168a78f05ae4938_R_1_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[0];
            float _Split_1ba6754f600a46739168a78f05ae4938_G_2_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[1];
            float _Split_1ba6754f600a46739168a78f05ae4938_B_3_Float = 0;
            float _Split_1ba6754f600a46739168a78f05ae4938_A_4_Float = 0;
            float _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_R_1_Float, IN.TimeParameters.x, _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float);
            float _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_G_2_Float, IN.TimeParameters.x, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2 = float2(_Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2;
            Unity_Add_float2(_Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2, _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2, _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2);
            float2 _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2;
            Unity_Add_float2(_Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2, (SHADERGRAPH_OBJECT_POSITION.xy), _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2);
            float _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float = Vector1_620eb0a4fc1a48c79d9daecb584075d4;
            float _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3;
            SimplexNoise3D_float((float3(_Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2, 0.0)), _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3);
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[0];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[1];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_B_3_Float = 0;
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_A_4_Float = 0;
            float3 _Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3 = float3(_Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float, _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float, float(10));
            float _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3;
            SimplexNoise3D_float(_Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3, _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3);
            float2 _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2 = float2(_SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float);
            float2 _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float.xx), _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2, _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2);
            float2 _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2;
            Unity_Multiply_float2_float2((_SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float.xx), _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2);
            float2 _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2;
            Unity_Add_float2(_Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2, _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2);
            float4 _Property_126da754180244e88cb50700b5928505_Out_0_Vector4 = _MainTex_TexelSize;
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_R_1_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[0];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_G_2_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[1];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[2];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[3];
            float2 _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2 = float2(_Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float, _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float);
            float2 _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2;
            Unity_Divide_float2(_Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2);
            float2 _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2;
            Unity_Posterize_float2(_Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2);
            float2 _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2;
            Unity_Subtract_float2((_UV_65a481774e39462988c1629da3193035_Out_0_Vector4.xy), _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2, _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2);
            float4 _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.tex, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.samplerstate, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.GetTransformedUV(_Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2) );
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.a;
            surface.Alpha = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
float _SBPBreaker;
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float Vector1_e19a6f73e9824493998ba7bebae9c03c;
        float2 Vector2_2ad9dffd23234809bbb6d55338af2214;
        float Vector1_2d61041f8dfd46289cb8aafd27290417;
        float Vector1_620eb0a4fc1a48c79d9daecb584075d4;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_PG_VelocityTexture);
        SAMPLER(sampler_PG_VelocityTexture);
        float4 _PG_VelocityTexture_TexelSize;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DisplacementMask);
        SAMPLER(sampler_DisplacementMask);
        float4 _DisplacementMask_TexelSize;
        float4 _PG_PixelScreenParams;
        
        // Graph Includes
        #include_with_pragmas "Assets/Plugins/pixelgraphics-master/Runtime/Shaders/SimplexNoise3D.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Fraction_float2(float2 In, out float2 Out)
        {
            Out = frac(In);
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Posterize_float2(float2 In, float2 Steps, out float2 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_65a481774e39462988c1629da3193035_Out_0_Vector4 = IN.uv0;
            UnityTexture2D _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_PG_VelocityTexture);
            float4 _ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_B_3_Float = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_A_4_Float = 0;
            float2 _Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2 = float2(_Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float, _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float);
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float = _WorldSpaceCameraPos[0];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float = _WorldSpaceCameraPos[1];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_B_3_Float = _WorldSpaceCameraPos[2];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_A_4_Float = 0;
            float2 _Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2 = float2(_Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float, _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float);
            float2 _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2 = float2(unity_OrthoParams.x, unity_OrthoParams.y);
            float2 _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2, _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2);
            float2 _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2, _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2);
            float4 _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_f5db6e799ed8488dad63158be533559a_R_1_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[0];
            float _Split_f5db6e799ed8488dad63158be533559a_G_2_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[1];
            float _Split_f5db6e799ed8488dad63158be533559a_B_3_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[2];
            float _Split_f5db6e799ed8488dad63158be533559a_A_4_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[3];
            float2 _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2, (_Split_f5db6e799ed8488dad63158be533559a_B_3_Float.xx), _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2);
            float2 _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2;
            Unity_Fraction_float2(_Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2, _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2);
            float3 _Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3 = float3(_Split_f5db6e799ed8488dad63158be533559a_R_1_Float, _Split_f5db6e799ed8488dad63158be533559a_G_2_Float, float(1));
            float2 _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2;
            Unity_Divide_float2(_Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2, (_Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2);
            float2 _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2;
            Unity_Subtract_float2((_ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2);
            float2 _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2;
            Unity_Posterize_float2(_Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2, float2(160, 90), _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2);
            float2 _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2;
            Unity_Add_float2(_Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2, _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2);
            float4 _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.tex, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.samplerstate, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.GetTransformedUV(_Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2) );
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.r;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.g;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_B_6_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.b;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_A_7_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.a;
            float2 _Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2 = float2(_SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float, _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float);
            float _Property_996b03b9050749a6959e97ba632841f3_Out_0_Float = Vector1_e19a6f73e9824493998ba7bebae9c03c;
            float2 _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2, (_Property_996b03b9050749a6959e97ba632841f3_Out_0_Float.xx), _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2);
            UnityTexture2D _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_DisplacementMask);
            float4 _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.tex, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.samplerstate, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.r;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.g;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_B_6_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.b;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_A_7_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.a;
            float2 _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2, (_SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float.xx), _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2);
            float _Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float = Vector1_2d61041f8dfd46289cb8aafd27290417;
            float _Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_3ce07f9210e244a39211fad1725c19c8_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_3ce07f9210e244a39211fad1725c19c8_A_4_Float = 0;
            float2 _Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2 = float2(_Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float, _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float);
            float4 _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_485e13f9e0954088870fd2739a33e10e_R_1_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[0];
            float _Split_485e13f9e0954088870fd2739a33e10e_G_2_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[1];
            float _Split_485e13f9e0954088870fd2739a33e10e_B_3_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[2];
            float _Split_485e13f9e0954088870fd2739a33e10e_A_4_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[3];
            float2 _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2;
            Unity_Posterize_float2(_Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2, (_Split_485e13f9e0954088870fd2739a33e10e_B_3_Float.xx), _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2);
            float2 _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2 = Vector2_2ad9dffd23234809bbb6d55338af2214;
            float _Split_1ba6754f600a46739168a78f05ae4938_R_1_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[0];
            float _Split_1ba6754f600a46739168a78f05ae4938_G_2_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[1];
            float _Split_1ba6754f600a46739168a78f05ae4938_B_3_Float = 0;
            float _Split_1ba6754f600a46739168a78f05ae4938_A_4_Float = 0;
            float _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_R_1_Float, IN.TimeParameters.x, _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float);
            float _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_G_2_Float, IN.TimeParameters.x, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2 = float2(_Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2;
            Unity_Add_float2(_Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2, _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2, _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2);
            float2 _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2;
            Unity_Add_float2(_Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2, (SHADERGRAPH_OBJECT_POSITION.xy), _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2);
            float _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float = Vector1_620eb0a4fc1a48c79d9daecb584075d4;
            float _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3;
            SimplexNoise3D_float((float3(_Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2, 0.0)), _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3);
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[0];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[1];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_B_3_Float = 0;
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_A_4_Float = 0;
            float3 _Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3 = float3(_Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float, _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float, float(10));
            float _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3;
            SimplexNoise3D_float(_Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3, _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3);
            float2 _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2 = float2(_SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float);
            float2 _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float.xx), _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2, _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2);
            float2 _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2;
            Unity_Multiply_float2_float2((_SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float.xx), _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2);
            float2 _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2;
            Unity_Add_float2(_Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2, _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2);
            float4 _Property_126da754180244e88cb50700b5928505_Out_0_Vector4 = _MainTex_TexelSize;
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_R_1_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[0];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_G_2_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[1];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[2];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[3];
            float2 _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2 = float2(_Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float, _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float);
            float2 _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2;
            Unity_Divide_float2(_Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2);
            float2 _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2;
            Unity_Posterize_float2(_Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2);
            float2 _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2;
            Unity_Subtract_float2((_UV_65a481774e39462988c1629da3193035_Out_0_Vector4.xy), _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2, _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2);
            float4 _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.tex, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.samplerstate, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.GetTransformedUV(_Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2) );
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.a;
            surface.Alpha = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZTest LEqual
        ZWrite Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
float _SBPBreaker;
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_vertex _ SKINNED_SPRITE
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 ObjectSpacePosition;
             float2 NDCPosition;
             float2 PixelPosition;
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color + (_SBPBreaker);
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float Vector1_e19a6f73e9824493998ba7bebae9c03c;
        float2 Vector2_2ad9dffd23234809bbb6d55338af2214;
        float Vector1_2d61041f8dfd46289cb8aafd27290417;
        float Vector1_620eb0a4fc1a48c79d9daecb584075d4;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_PG_VelocityTexture);
        SAMPLER(sampler_PG_VelocityTexture);
        float4 _PG_VelocityTexture_TexelSize;
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DisplacementMask);
        SAMPLER(sampler_DisplacementMask);
        float4 _DisplacementMask_TexelSize;
        float4 _PG_PixelScreenParams;
        
        // Graph Includes
        #include_with_pragmas "Assets/Plugins/pixelgraphics-master/Runtime/Shaders/SimplexNoise3D.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Fraction_float2(float2 In, out float2 Out)
        {
            Out = frac(In);
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Posterize_float2(float2 In, float2 Steps, out float2 Out)
        {
            Out = floor(In * Steps) / Steps;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_65a481774e39462988c1629da3193035_Out_0_Vector4 = IN.uv0;
            UnityTexture2D _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_PG_VelocityTexture);
            float4 _ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float = SHADERGRAPH_OBJECT_POSITION[0];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float = SHADERGRAPH_OBJECT_POSITION[1];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_B_3_Float = SHADERGRAPH_OBJECT_POSITION[2];
            float _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_A_4_Float = 0;
            float2 _Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2 = float2(_Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_R_1_Float, _Split_37203ee1d6394a4b8d11f5d0bdf4e1c8_G_2_Float);
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float = _WorldSpaceCameraPos[0];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float = _WorldSpaceCameraPos[1];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_B_3_Float = _WorldSpaceCameraPos[2];
            float _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_A_4_Float = 0;
            float2 _Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2 = float2(_Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_R_1_Float, _Split_0d3fa3fe8e8b4fc1ab8620ea28f8b146_G_2_Float);
            float2 _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2 = float2(unity_OrthoParams.x, unity_OrthoParams.y);
            float2 _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_974fbc4840334dd3a2446fc0e3caae36_Out_0_Vector2, _Vector2_693104a989c249c7baac881c2414a68a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2);
            float2 _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2;
            Unity_Subtract_float2(_Vector2_43e47be3e12c4b70b27b66ff6ad5324a_Out_0_Vector2, _Subtract_02221ad497c641068080295be1036483_Out_2_Vector2, _Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2);
            float4 _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_f5db6e799ed8488dad63158be533559a_R_1_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[0];
            float _Split_f5db6e799ed8488dad63158be533559a_G_2_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[1];
            float _Split_f5db6e799ed8488dad63158be533559a_B_3_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[2];
            float _Split_f5db6e799ed8488dad63158be533559a_A_4_Float = _Property_7a949dfeb794479da980afc0bc61f42c_Out_0_Vector4[3];
            float2 _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Subtract_9121c548c0744c9ebd3e06bf5c985416_Out_2_Vector2, (_Split_f5db6e799ed8488dad63158be533559a_B_3_Float.xx), _Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2);
            float2 _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2;
            Unity_Fraction_float2(_Multiply_6229340ddec1400582922a984a5d356a_Out_2_Vector2, _Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2);
            float3 _Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3 = float3(_Split_f5db6e799ed8488dad63158be533559a_R_1_Float, _Split_f5db6e799ed8488dad63158be533559a_G_2_Float, float(1));
            float2 _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2;
            Unity_Divide_float2(_Fraction_0d50b067cb254a98978745b08f74713d_Out_1_Vector2, (_Vector3_cdfa494c8063446785d6fb9a550140f3_Out_0_Vector3.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2);
            float2 _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2;
            Unity_Subtract_float2((_ScreenPosition_89bb2978ca1f4809b83a3bec5b0cfde9_Out_0_Vector4.xy), _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2);
            float2 _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2;
            Unity_Posterize_float2(_Subtract_f69701f7647448b8be4faff04143a47a_Out_2_Vector2, float2(160, 90), _Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2);
            float2 _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2;
            Unity_Add_float2(_Posterize_a4e1983d7ddb4813b74b2bc6d1139122_Out_2_Vector2, _Divide_7acde764b20a412bad64e52ba7686fa8_Out_2_Vector2, _Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2);
            float4 _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.tex, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.samplerstate, _Property_9086dcf0801249169ecb4a449d8e8633_Out_0_Texture2D.GetTransformedUV(_Add_ad872963e0a040849c0b7b5c60eb5f7b_Out_2_Vector2) );
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.r;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.g;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_B_6_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.b;
            float _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_A_7_Float = _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_RGBA_0_Vector4.a;
            float2 _Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2 = float2(_SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_R_4_Float, _SampleTexture2D_8f8b80af9b5e4ccd9ca5e0923e2984eb_G_5_Float);
            float _Property_996b03b9050749a6959e97ba632841f3_Out_0_Float = Vector1_e19a6f73e9824493998ba7bebae9c03c;
            float2 _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Vector2_bfa7184339dd4588aff2521dda0c7cb7_Out_0_Vector2, (_Property_996b03b9050749a6959e97ba632841f3_Out_0_Float.xx), _Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2);
            UnityTexture2D _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_DisplacementMask);
            float4 _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.tex, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.samplerstate, _Property_cf2b171398ca4c85a200ef2b0d3de18b_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.r;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.g;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_B_6_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.b;
            float _SampleTexture2D_97e11122980446e292902ead83e32d46_A_7_Float = _SampleTexture2D_97e11122980446e292902ead83e32d46_RGBA_0_Vector4.a;
            float2 _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2;
            Unity_Multiply_float2_float2(_Multiply_7cae94312dee4dc4916f037a00492a87_Out_2_Vector2, (_SampleTexture2D_97e11122980446e292902ead83e32d46_R_4_Float.xx), _Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2);
            float _Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float = Vector1_2d61041f8dfd46289cb8aafd27290417;
            float _Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float = IN.ObjectSpacePosition[0];
            float _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float = IN.ObjectSpacePosition[1];
            float _Split_3ce07f9210e244a39211fad1725c19c8_B_3_Float = IN.ObjectSpacePosition[2];
            float _Split_3ce07f9210e244a39211fad1725c19c8_A_4_Float = 0;
            float2 _Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2 = float2(_Split_3ce07f9210e244a39211fad1725c19c8_R_1_Float, _Split_3ce07f9210e244a39211fad1725c19c8_G_2_Float);
            float4 _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4 = _PG_PixelScreenParams;
            float _Split_485e13f9e0954088870fd2739a33e10e_R_1_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[0];
            float _Split_485e13f9e0954088870fd2739a33e10e_G_2_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[1];
            float _Split_485e13f9e0954088870fd2739a33e10e_B_3_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[2];
            float _Split_485e13f9e0954088870fd2739a33e10e_A_4_Float = _Property_9d69932be50e4d389c48d0cdfc47d755_Out_0_Vector4[3];
            float2 _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2;
            Unity_Posterize_float2(_Vector2_be89dc528e844123a4b69ab1240affd2_Out_0_Vector2, (_Split_485e13f9e0954088870fd2739a33e10e_B_3_Float.xx), _Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2);
            float2 _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2 = Vector2_2ad9dffd23234809bbb6d55338af2214;
            float _Split_1ba6754f600a46739168a78f05ae4938_R_1_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[0];
            float _Split_1ba6754f600a46739168a78f05ae4938_G_2_Float = _Property_fd967b42791a42b793e4f1d537525e2f_Out_0_Vector2[1];
            float _Split_1ba6754f600a46739168a78f05ae4938_B_3_Float = 0;
            float _Split_1ba6754f600a46739168a78f05ae4938_A_4_Float = 0;
            float _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_R_1_Float, IN.TimeParameters.x, _Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float);
            float _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float;
            Unity_Multiply_float_float(_Split_1ba6754f600a46739168a78f05ae4938_G_2_Float, IN.TimeParameters.x, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2 = float2(_Multiply_4f670d3f67c842d496bdfbf55182d4d8_Out_2_Float, _Multiply_7aff65c01c5d416fbae8bc37eb839741_Out_2_Float);
            float2 _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2;
            Unity_Add_float2(_Posterize_bbbe93fe11c74d1787339d7e4b1863b7_Out_2_Vector2, _Vector2_1e096e9d34e04670a7aa614ab756a564_Out_0_Vector2, _Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2);
            float2 _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2;
            Unity_Add_float2(_Add_d8221f5c0ffe4e178b9f7dbc9adf6053_Out_2_Vector2, (SHADERGRAPH_OBJECT_POSITION.xy), _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2);
            float _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float = Vector1_620eb0a4fc1a48c79d9daecb584075d4;
            float _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3;
            SimplexNoise3D_float((float3(_Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2, 0.0)), _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Gradient_2_Vector3);
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[0];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float = _Add_b0213b284c164a3da031726999cecfd1_Out_2_Vector2[1];
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_B_3_Float = 0;
            float _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_A_4_Float = 0;
            float3 _Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3 = float3(_Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_R_1_Float, _Split_d7a4c65d80ad401b8e2f2bdbdb7561bd_G_2_Float, float(10));
            float _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float;
            float3 _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3;
            SimplexNoise3D_float(_Vector3_7b66cdc231f84386b877baf60bc2dbe0_Out_0_Vector3, _Property_6fa1d402f6464734a9319496493c2add_Out_0_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Gradient_2_Vector3);
            float2 _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2 = float2(_SimplexNoise3DCustomFunction_154a134882ad40faa9ef59daeedaf96d_Noise_1_Float, _SimplexNoise3DCustomFunction_d2c7053ec98f462ab01cf9f3a3710f60_Noise_1_Float);
            float2 _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2;
            Unity_Multiply_float2_float2((_Property_508bd54927584bf29d23ddd87f4b9f41_Out_0_Float.xx), _Vector2_44e23b8e3d32473f868788d887fb9207_Out_0_Vector2, _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2);
            float2 _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2;
            Unity_Multiply_float2_float2((_SampleTexture2D_97e11122980446e292902ead83e32d46_G_5_Float.xx), _Multiply_adadeb1f51344948b6b821de8b005363_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2);
            float2 _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2;
            Unity_Add_float2(_Multiply_440ea2eee016474586355dfafafde880_Out_2_Vector2, _Multiply_3a0af437e31640a698faecefebd94101_Out_2_Vector2, _Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2);
            float4 _Property_126da754180244e88cb50700b5928505_Out_0_Vector4 = _MainTex_TexelSize;
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_R_1_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[0];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_G_2_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[1];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[2];
            float _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float = _Property_126da754180244e88cb50700b5928505_Out_0_Vector4[3];
            float2 _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2 = float2(_Split_ac7b21c4bd294f64a1064f11ffadabc2_B_3_Float, _Split_ac7b21c4bd294f64a1064f11ffadabc2_A_4_Float);
            float2 _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2;
            Unity_Divide_float2(_Add_10ca8e349f484f21a48cf2cd8d59c95f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2);
            float2 _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2;
            Unity_Posterize_float2(_Divide_f45d8c6ed97d4d5a959e2a01ba1fa09f_Out_2_Vector2, _Vector2_08437cbdf17b45c6accb6a8698d50077_Out_0_Vector2, _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2);
            float2 _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2;
            Unity_Subtract_float2((_UV_65a481774e39462988c1629da3193035_Out_0_Vector4.xy), _Posterize_0afb391dec054d71b42d94ef57e75bbd_Out_2_Vector2, _Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2);
            float4 _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.tex, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.samplerstate, _Property_5bd31f83e4144284ac0f5a4f389a9424_Out_0_Texture2D.GetTransformedUV(_Subtract_efdcd0cbe7a646deb54099fcc2542b4d_Out_2_Vector2) );
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.r;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.g;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.b;
            float _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_RGBA_0_Vector4.a;
            float3 _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3 = float3(_SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_R_4_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_G_5_Float, _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_B_6_Float);
            surface.BaseColor = _Vector3_768535d743c049d7a2dc4ab657c5b5c1_Out_0_Vector3;
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Alpha = _SampleTexture2D_7a2132bf1ccc47bc988a6f64e04f2dc2_A_7_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
            output.uv0 = input.texCoord0;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphSpriteGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}
