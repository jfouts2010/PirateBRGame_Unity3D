Shader "Lux Water/WaterMask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

//	1st pass Box volume
		Pass
		{
			Ztest Off
		//	When inside we have to flip culling
			Cull Front
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ GERSTNERENABLED
				#define USINGWATERVOLUME
				
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

			//	Gerstner Waves
				#if defined(GERSTNERENABLED)
					float3 _GerstnerVertexIntensity;
					float _GerstnerNormalIntensity;
					uniform float3 _LuxWaterMask_GerstnerVertexIntensity;
				 	uniform float4 _LuxWaterMask_GAmplitude;
				    uniform float4 _LuxWaterMask_GFinalFrequency;
				    uniform float4 _LuxWaterMask_GSteepness;
				    uniform float4 _LuxWaterMask_GFinalSpeed;
				    uniform float4 _LuxWaterMask_GDirectionAB;
				    uniform float4 _LuxWaterMask_GDirectionCD;
			    	#include "../Includes/LuxWater_GerstnerWaves.cginc"
			    #endif
				
				v2f vert (appdata v)
				{
					v2f o;

					#if defined(GERSTNERENABLED)
						float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
						_GerstnerVertexIntensity = _LuxWaterMask_GerstnerVertexIntensity;

						half3 vtxForAni = (wpos).xzz;
						half3 offsets;
						GerstnerOffsetOnly (
							offsets, v.vertex.xyz, vtxForAni,							// offsets
							_LuxWaterMask_GAmplitude,									// amplitude
							_LuxWaterMask_GFinalFrequency,								// frequency
							_LuxWaterMask_GSteepness,									// steepness
							_LuxWaterMask_GFinalSpeed,									// speed
							_LuxWaterMask_GDirectionAB,									// direction # 1, 2
							_LuxWaterMask_GDirectionCD									// direction # 3, 4
						);
						wpos.xyz += offsets * v.color.r;
						v.vertex = mul(unity_WorldToObject, wpos);
					#endif

					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = half4(0,1,0, 1);
					return col;
				}
			ENDCG
		}

//	2nd pass water surface from top
		pass
		{
			Ztest Less
			Cull Back
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ GERSTNERENABLED
				#define USINGWATERVOLUME

				#include "UnityCG.cginc"
				
				struct appdata
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float depth : TEXCOORD0;
				};

			//	Gerstner Waves
				#if defined(GERSTNERENABLED)
					float3 _GerstnerVertexIntensity;
					float _GerstnerNormalIntensity;
					uniform float3 _LuxWaterMask_GerstnerVertexIntensity;
				 	uniform float4 _LuxWaterMask_GAmplitude;
				    uniform float4 _LuxWaterMask_GFinalFrequency;
				    uniform float4 _LuxWaterMask_GSteepness;
				    uniform float4 _LuxWaterMask_GFinalSpeed;
				    uniform float4 _LuxWaterMask_GDirectionAB;
				    uniform float4 _LuxWaterMask_GDirectionCD;

				    #include "../Includes/LuxWater_GerstnerWaves.cginc"
				#endif
				
				v2f vert (appdata v)
				{
					v2f o;

					#if defined(GERSTNERENABLED)
						float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
						_GerstnerVertexIntensity = _LuxWaterMask_GerstnerVertexIntensity;

						half3 vtxForAni = (wpos).xzz;
						half3 offsets;
						GerstnerOffsetOnly (
							offsets, v.vertex.xyz, vtxForAni,							// offsets
							_LuxWaterMask_GAmplitude,									// amplitude
							_LuxWaterMask_GFinalFrequency,								// frequency
							_LuxWaterMask_GSteepness,									// steepness
							_LuxWaterMask_GFinalSpeed,									// speed
							_LuxWaterMask_GDirectionAB,									// direction # 1, 2
							_LuxWaterMask_GDirectionCD									// direction # 3, 4
						);
						wpos.xyz += offsets * v.color.r;
						v.vertex = mul(unity_WorldToObject, wpos);
					#endif

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.depth = COMPUTE_DEPTH_01;
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target {
					fixed2 depth = EncodeFloatRG(i.depth);
					fixed4 col = half4(1, 0, depth.x, depth.y);
					return col;
				}
			ENDCG
		}

//	3nd pass water surface from below - here we have to output depth
		pass {
			Ztest LEqual
			Cull Front
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile __ GERSTNERENABLED
				#define USINGWATERVOLUME
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float depth : TEXCOORD0;
				};

			//	Gerstner Waves
				#if defined(GERSTNERENABLED)
					float3 _GerstnerVertexIntensity; // dummy
					float _GerstnerNormalIntensity;	 // dummy
					uniform float3 _LuxWaterMask_GerstnerVertexIntensity;
				 	uniform float4 _LuxWaterMask_GAmplitude;
				    uniform float4 _LuxWaterMask_GFinalFrequency;
				    uniform float4 _LuxWaterMask_GSteepness;
				    uniform float4 _LuxWaterMask_GFinalSpeed;
				    uniform float4 _LuxWaterMask_GDirectionAB;
				    uniform float4 _LuxWaterMask_GDirectionCD;

			    	#include "../Includes/LuxWater_GerstnerWaves.cginc"
			    #endif
				
				v2f vert (appdata v)
				{
					v2f o;

					#if defined(GERSTNERENABLED)
						float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
						_GerstnerVertexIntensity = _LuxWaterMask_GerstnerVertexIntensity;

						half3 vtxForAni = (wpos).xzz;
						half3 offsets;
						GerstnerOffsetOnly (
							offsets, v.vertex.xyz, vtxForAni,							// offsets
							_LuxWaterMask_GAmplitude,									// amplitude
							_LuxWaterMask_GFinalFrequency,								// frequency
							_LuxWaterMask_GSteepness,									// steepness
							_LuxWaterMask_GFinalSpeed,									// speed
							_LuxWaterMask_GDirectionAB,									// direction # 1, 2
							_LuxWaterMask_GDirectionCD									// direction # 3, 4
						);
						wpos.xyz += offsets * v.color.r;
						v.vertex = mul(unity_WorldToObject, wpos);
					#endif

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.depth = COMPUTE_DEPTH_01;
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target {
					fixed2 depth = EncodeFloatRG(i.depth);
					fixed4 col = half4(0,0.5, depth.x, depth.y);
					return col;
				}
			ENDCG
		}
	}
}
