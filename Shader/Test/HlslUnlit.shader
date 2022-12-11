
Shader "HlslUnlitShader"
{
    Properties
    {
        _BaseMap("MainTex", 2D) = "White" { }
        _BaseColor("BaseColor", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        // URP的shader要在Tags中注明渲染管线是UniversalPipeline
        Tags
        {
            "RanderPipline" = "UniversalPipeline"
            "RanderType" = "Opaque"
        }

        HLSLINCLUDE

            // 引入Core.hlsl头文件，替换UnityCG中的cginc
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 将不占空间的材质相关变量放在CBUFFER中，为了兼容SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
            CBUFFER_END

            // 材质单独声明，使用DX11风格的新采样方法
            TEXTURE2D (_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };

        ENDHLSL

        Pass
        {
            // 声明Pass名称，方便调用与识别
            Name "ForwardUnlit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

                // 声明顶点/片段着色器对应的函数
                #pragma vertex vert
                #pragma fragment frag

                // 顶点着色器
                Varyings vert(Attributes input)
                {
                    // GetVertexPositionInputs方法根据使用情况自动生成各个坐标系下的定点信息
                    const VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                    Varyings output;
                    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                    output.positionCS = vertexInput.positionCS;
                    return output;
                }

                // 片段着色器
                half4 frag(Varyings input) : SV_Target
                {
                    return SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                }
            
            ENDHLSL
        }
    }
}
