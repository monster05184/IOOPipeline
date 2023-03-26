
Shader "GpuDrivenShader"
{
    Properties
    {
        _AlbedoMap("Albedo", 2D) = "white"{}	//反照率
        _BumpMap("Normal Map", 2D) = "bump"{} //法线贴图
    	_MetallicGlossMap("Metallic", 2D) = "white"{} //金属图，r通道存储金属度，a通道存储光滑度
    	_MetallicStrength("MetallicStrength",Range(0,1)) = 1 //金属强度
		_GlossStrength("Smoothness",Range(0,1)) = 0.5 //光滑强度
        _EmissionColor("Emission Color", color) = (0,0,0) //自发光颜色
		_EmissionMap("Emission Map", 2D) = "white"{}//自发光贴图
    }

    SubShader
    {
        Cull Off
       
        Tags
        {
        	"RanderPipline" = "UniversalPipeline"
            "RanderType" = "Opaque"
            "LightMode" = "UniversalForward"
        }

        HLSLINCLUDE

        #include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
            
         //-------------------------------------BRDF-------------------------------------------------//
        inline float D_GGX_Float(float NoH, float roughness)
        {
            float a = NoH * roughness;
            float k = roughness / (1.0f - NoH * NoH + a * a);
            return k * k * (1.0f / UNITY_PI);
        }
        inline half D_GGX_Half(float roughness, float NoH, const half3 n, const half3 h) {
            half3 NxH = cross(n, h);
            half a = NoH * roughness;
            half k = roughness / (dot(NxH, NxH) + a * a);
            half d = k * k * (1.0 / UNITY_PI);
            return saturate(d);
        }

        //计算Smith-Joint阴影遮掩函数，返回的是除以镜面反射项分母的可见性项V
		inline half ComputeSmithJointGGXVisibilityTerm(half nl,half nv,half roughness)
		{
			half ag = roughness * roughness;
			half lambdaV = nl * (nv * (1 - ag) + ag);
			half lambdaL = nv * (nl * (1 - ag) + ag);
			
			return 0.5f/(lambdaV + lambdaL + 1e-5f);
		}
		//计算法线分布函数
		inline half ComputeGGXTerm(half nh,half roughness)
		{
			half a = roughness * roughness;
			half a2 = a * a;
			half d = (a2 - 1.0f) * nh * nh + 1.0f;
			//UNITY_INV_PI定义在UnityCG.cginc  为1/π
			return a2 * UNITY_INV_PI / (d * d + 1e-5f);
		}
		//计算菲涅尔
		inline half3 ComputeFresnelTerm(half3 F0,half cosA)
		{
			return F0 + (1 - F0) * pow(1 - cosA, 5);
		}
		//计算漫反射项
		inline half3 ComputeDisneyDiffuseTerm(half nv,half nl,half lh,half roughness,half3 baseColor)
		{
			half Fd90 = 0.5f + 2 * roughness * lh * lh;
			return baseColor * UNITY_INV_PI * (1 + (Fd90 - 1) * pow(1-nl,5)) * (1 + (Fd90 - 1) * pow(1-nv,5));
		}
		//计算间接光镜面反射菲涅尔项
		inline half3 ComputeFresnelLerp(half3 c0,half3 c1,half cosA)
		{
			half t = pow(1 - cosA,5);
			return lerp(c0,c1,t);
		}
   
        //-------------------------------------BRDF-------------------------------------------------//


        //将法线从DXTnm解码
        inline half3 DecodeDXTnm(float4 packednormal)
        {
		    half3 normal;
		    normal.xy = packednormal.wy * 2 - 1;
		    normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
		    return normal;
        }
        inline float2 DecodeNormalGamma(float2 gamma)
        {
	        return pow(gamma, 2.2);
        }

        
        //-------------------------------------结构体-----------------------------------------------//
        struct Bounds
        {
            float3 extent;
            float3 position;
        };
        struct Cluster
        {
            Bounds bound;
            int objIndex;
        };
        struct MaterialProperty
        {
             float4 mainColor;     
        };
        struct ObjectInfo
        {
            MaterialProperty material_property;
            float4x4 local2WorldMatrix;
        };
        
        struct Vertex
        {
            float3 pos;
            float2 uv;
            float3 normal;
            float4 tangent;
        };
         //--------------------------------------结构体----------------------------------------------//

    
        StructuredBuffer<Vertex> _VertexBuffer;//顶点的数据
        StructuredBuffer<Cluster> _ClusterBuffer;
        StructuredBuffer<ObjectInfo> _RenderObjectBuffer;//渲染物体
        StructuredBuffer<int> _ResultBuffer;//剔除结果

        UNITY_DECLARE_TEX2DARRAY(abedoTexture);
        UNITY_DECLARE_TEX2DARRAY(metallicMap);
        UNITY_DECLARE_TEX2DARRAY(normalTexture);
        
        float4 _AlbedoMap_ST;
        sampler2D _AlbedoMap;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        //Test
        sampler2D _TestTex;
        half _MetallicStrength;
        half _GlossStrength;
        


        struct Attributes
        { 
            uint vertexID : SV_VertexID;
            uint instanceID : SV_InstanceID;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            uint objectIndex  : TEXCOORD0;
            float2 uv         : TEXCOORD1;
            float4 TtoW0      : TEXCOORD2;
            float4 TtoW1      : TEXCOORD3;
            float4 TtoW2      : TEXCOORD4;
            half4 ambientOrLightmapUV:TEXCOORD5;
            
        };

        ENDHLSL

        Pass
        {
            // 声明Pass名称，方便调用与识别
            Name "ForwardUnlit"
            Tags {"LightMode" = "GpuDriven"}
            

            HLSLPROGRAM

            // 声明顶点/片段着色器对应的函数
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #pragma enable_d3d11_debug_symbols //允许调试

            // 顶点着色器
            Varyings vert(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
            {
                //处理Cluster数据
                uint cluster_id = _ResultBuffer[instanceID];
        		cluster_id = instanceID;
                uint renderObjectId = _ClusterBuffer[cluster_id].objIndex;
                //获取渲染物的信息  
                const ObjectInfo object_info= _RenderObjectBuffer[renderObjectId];

                Vertex vertex = _VertexBuffer[vertexID];
                float4 pos = float4(vertex.pos,1);
                float4 posWS = mul(object_info.local2WorldMatrix, pos);
                float4 posCS = mul(unity_MatrixVP, posWS);
                float3 normalWS = normalize(mul((float3x3)object_info.local2WorldMatrix, vertex.normal));
                float3 tangentWS = normalize(mul((float3x3)object_info.local2WorldMatrix, vertex.tangent));
                float3 binormalWS = cross(normalWS, tangentWS) * vertex.tangent.w;
        
                //Output
                Varyings output;
           
                output.positionCS = posCS;
                output.objectIndex = renderObjectId;
                output.TtoW0 = float4(tangentWS.x, binormalWS.x, normalWS.x, posWS.x);
                output.TtoW1 = float4(tangentWS.y, binormalWS.y, normalWS.y, posWS.y);
                output.TtoW2 = float4(tangentWS.z, binormalWS.z, normalWS.z, posWS.z);

                output.uv = TRANSFORM_TEX(vertex.uv, _AlbedoMap);
                
        
                
                return output;
            }


            
        
            // 片段着色器
            half4 frag(Varyings input) : SV_Target
            {
                //数据准备
                MaterialProperty material_property = _RenderObjectBuffer[input.objectIndex].material_property;//材质属性
                float4 mainColor = material_property.mainColor;
                float3 posWS = float3(input.TtoW0.w, input.TtoW1.w, input.TtoW1.w);
            	//half2 metallicGloss = tex2D(_MetallicGlossMap,input.uv).ra;
            	half2 metallicGloss = UNITY_SAMPLE_TEX2DARRAY(metallicMap, half3(input.uv, input.objectIndex)).ra;
				half metallic = metallicGloss.x * _MetallicStrength;//金属度
				half roughness = 1 - metallicGloss.y * _GlossStrength;//粗糙度

            	half4 normalTextureColor = UNITY_SAMPLE_TEX2DARRAY(normalTexture, half3(input.uv, input.objectIndex));
            	half3 normalTS = UnpackNormal(normalTextureColor);
            	
            	
                half3 normalWS = normalize(half3(dot(input.TtoW0.xyz,normalTS),
								dot(input.TtoW1.xyz,normalTS),dot(input.TtoW2.xyz,normalTS)));
                half3 albedo = UNITY_SAMPLE_TEX2DARRAY(abedoTexture, half3(input.uv, input.objectIndex));
                
                half3 lightDir = normalize(UnityWorldSpaceLightDir(posWS));//世界空间下的灯光方向,定义在UnityCG.cginc
			    half3 viewDir = normalize(UnityWorldSpaceViewDir(posWS));//世界空间下的观察方向,定义在UnityCG.cginc
			    half3 refDir = reflect(-viewDir, posWS);//世界空间下的反射方向

            	//计算BRDF需要用到一些项
				half3 halfDir = normalize(lightDir + viewDir);
				half nv = saturate(dot(normalWS,viewDir));
				half nl = saturate(dot(normalWS,lightDir));
				half nh = saturate(dot(normalWS,halfDir));
				half lv = saturate(dot(lightDir,viewDir));
				half lh = saturate(dot(lightDir,halfDir));

            	//计算镜面反射率
				half3 specColor = lerp(unity_ColorSpaceDielectricSpec.rgb,albedo,metallic);
				//计算1 - 反射率,漫反射总比率
				half oneMinusReflectivity = (1- metallic) * unity_ColorSpaceDielectricSpec.a;
				//计算漫反射率
				half3 diffColor = albedo * oneMinusReflectivity;
                
				half V = ComputeSmithJointGGXVisibilityTerm(nl,nv,roughness);//计算BRDF高光反射项，可见性V
				half D = ComputeGGXTerm(nh,roughness);//计算BRDF高光反射项,法线分布函数D
				half3 F = ComputeFresnelTerm(specColor,lh);//计算BRDF高光反射项，菲涅尔项F

            	half3 specularTerm = V * D * F;//计算镜面反射项

				half3 diffuseTerm = ComputeDisneyDiffuseTerm(nv,nl,lh,roughness,diffColor);//计算漫反射项

            	half3 directColor = UNITY_PI * (diffuseTerm + specularTerm) * _LightColor0.rgb * nl;
				
            	half3 cube = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0,refDir,1);

            	
				//return float4(normalTS, 1);
            	return float4(directColor, 1);
            	return normalTS.b;
            }
            
            ENDHLSL
        }
    }
}
