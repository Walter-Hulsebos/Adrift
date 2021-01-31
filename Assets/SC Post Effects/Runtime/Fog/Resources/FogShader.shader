Shader "Hidden/SC Post Effects/Fog"
{
	HLSLINCLUDE

#define REQUIRE_DEPTH
#include "../../../Shaders/Pipeline/Pipeline.hlsl"
#include "../../../Shaders/Blurring.hlsl"

		DECLARE_TEX(_NoiseTex);
	DECLARE_TEX(_ColorGradient);
	DECLARE_RT(_SkyboxTex);

#pragma fragmentoption ARB_precision_hint_nicest

	uniform float4 _ViewDir;
	uniform half _FarClippingPlane;
	uniform float4 _HeightParams;
	uniform float4 _DistanceParams;
	uniform int4 _SceneFogMode;
	uniform float4 _SceneFogParams;
	uniform float4 _DensityParams;
	uniform float4 _NoiseParams;
	uniform float4 _FogColor;
	uniform float4x4 clipToWorld;
	uniform float4 _SkyboxParams;
	//X: Influence
	//Y: Mip level
	float4 _DirLightParams;
	//XYZ: Direction
	//W: Intensity
	float4 _DirLightColor; //(a=free)

	//Light scattering
	DECLARE_RT(_BloomTex);
	DECLARE_RT(_AutoExposureTex);

	float  _SampleScale;
	float4 _Threshold; // x: threshold value (linear), y: threshold - knee, z: knee * 2, w: 0.25 / knee
	float4 _ScatteringParams; // x: Sample scale y: Intensity z: 0 w: Itterations

	struct v2f {
		float4 positionCS : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 texcoordStereo : TEXCOORD1;
		float3 worldDirection : TEXCOORD2;
		float time : TEXCOORD3;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f VertWorldSpaceReconstruction(Attributes v) {
		v2f o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);
		o.uv.xy = GET_TRIANGLE_UV(v);

		o.uv = FlipUV(o.uv);

		float4 clip = float4(o.uv.xy * 2 - 1, 0.0, 1.0);
		o.worldDirection = (mul((float4x4)clipToWorld, clip.rgba).xyz - _WorldSpaceCameraPos).xyz;
		o.time = _Time.y;

		//UNITY_SINGLE_PASS_STEREO
		o.texcoordStereo = GET_TRIANGLE_UV_VR(o.uv, v.vertexID);

		return o;
	}

	half ComputeDensity(float coord)
	{
		float fogFac = 0.0;
		if (_SceneFogMode.x == 1) // linear
		{
			// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
			fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
		}
		if (_SceneFogMode.x == 2) // exp
		{
			// factor = exp(-density*z)
			fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
		}
		if (_SceneFogMode.x == 3) // exp2
		{
			// factor = exp(-(density*z)^2)
			fogFac = _SceneFogParams.x * coord; fogFac = exp2(-fogFac * fogFac);
		}
		return saturate(fogFac);
	}

	float ComputeDistance(float3 wpos, float depth)
	{
		float3 wsDir = _WorldSpaceCameraPos.xyz - wpos;
		float dist;
		//Radial distance
		if (_SceneFogMode.y == 1)
			dist = length(wsDir);
		else
			dist = depth * _ProjectionParams.z;
		//Start distance
		dist -= _ProjectionParams.y;
		return dist;
	}

	float ComputeHeight(float3 wpos)
	{
		float3 wsDir = _WorldSpaceCameraPos.xyz - wpos;
		float FH = _HeightParams.x;
		float3 C = _WorldSpaceCameraPos;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = _HeightParams.w * V;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y - FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1 - 2 * k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV + 1.0e-5f));
		return g;
	}

	float3 GetFogColor() {

	}

	float4 ComputeFog(v2f i)
	{
		float depth = SAMPLE_DEPTH(UV_VR);
		float linearDepth = LINEAR_DEPTH(depth);

		float skyMask = 1;
		if (linearDepth > 0.99) skyMask = 0;

		float3 worldPos = i.worldDirection * LINEAR_EYE_DEPTH(depth) + _WorldSpaceCameraPos;

		//Fog start distance
		float g = _DistanceParams.x;

		//Distance fog
		float distanceFog = 0;
		float distanceWeight = 0;
		if (_DistanceParams.z == 1) {
			distanceFog = ComputeDistance(worldPos, linearDepth);

			//Density (seperated so it doesn't affect the UV of a gradient texture)
			distanceWeight = distanceFog * _DensityParams.x;
			g += distanceWeight;
		}

		//Height fog
		float heightFog = 0;
		if (_DistanceParams.w == 1) {
			float noise = 1;
			if (_SceneFogMode.w == 1)
			{
				noise = SAMPLE_TEX(_NoiseTex, Repeat, worldPos.xz * _NoiseParams.x + (i.time * _NoiseParams.y * float2(0, 1))).r;
				noise = lerp(1, noise, _DensityParams.y * skyMask);
			}
			heightFog = ComputeHeight(worldPos);
			g += heightFog * noise;
		}

		//Fog density (Linear/Exp/ExpSqr)
		half fogFac = ComputeDensity(max(0.0, g));

		//Skybox influence
		if (linearDepth > 0.99) fogFac = lerp(1.0, fogFac, _SkyboxParams.x);

		//Color
		float4 fogColor = _FogColor.rgba;
		if (_SceneFogMode.z == 1)
		{
			fogColor = SAMPLE_TEX(_ColorGradient, Clamp, float2(distanceFog / _FarClippingPlane, 0));
		}
		if (_SceneFogMode.z == 2) {
			/*
			float maxMip = 6;
			float nearMip = 8;
			float farMip = 1;

			float mipLevel = (1.0 - maxMip * saturate((fogFac - nearMip) / (farMip - nearMip))) * _SkyboxParams.y;
			*/
			fogColor = SAMPLE_RT_LOD(_SkyboxTex, Clamp, UV, _SkyboxParams.y);
		}

		float NdotL = max(0, dot(i.worldDirection, _DirLightParams.xyz));
		fogColor = lerp(fogColor, _DirLightColor, saturate(NdotL) * _DirLightParams.w);

		fogColor.a = fogFac;

		return fogColor;
	}

	half4 Prefilter(half4 color, float2 uv)
	{
		half autoExposure = SAMPLE_RT(_AutoExposureTex, Clamp, uv).r;
		color *= autoExposure;
		//color = min(_Params.x, color); // clamp to max
		color = QuadraticThreshold(color, _Threshold.x, _Threshold.yzw);
		return color;
	}

	half4 FragPrefilter(Varyings i) : SV_Target
	{
		half4 color = BoxFilter4(RT_PARAM(_MainTex, Clamp), UV, _MainTex_TexelSize.xy, 1);
		return Prefilter(SafeHDR(color), UV);
	}

		half4 FragDownsample(Varyings i) : SV_Target
	{
		half4 color = BoxFilter4(RT_PARAM(_MainTex, Clamp), UV, _MainTex_TexelSize.xy, 1);
		return color;
	}

		half4 Combine(half4 bloom, float2 uv)
	{
		half4 color = SAMPLE_RT(_BloomTex, Clamp, uv);
		return bloom + color;
	}

	half4 FragUpsample(Varyings i) : SV_Target
	{
		half4 bloom = UpsampleBox(RT_PARAM(_MainTex, Clamp), UV, _MainTex_TexelSize.xy, _SampleScale);
		return Combine(bloom, UV_VR);
	}


		float4 FragBlend(v2f i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 screenColor = ScreenColor(UV_VR);

		//Alpha is density, do not modify
		float4 fogColor = ComputeFog(i);

		//screenColor.rgb = lerp(bloom.rgb, screenColor.rgb, fogColor.a);

		//Linear blend
		float3 blendedColor = lerp(fogColor.rgb, screenColor.rgb, fogColor.a);

		//Keep alpha channel for FXAA
		return float4(blendedColor.rgb, screenColor.a);
	}


		float4 FragBlendScattering(v2f i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 screenColor = ScreenColor(UV_VR);
		half4 bloom = SAMPLE_RT(_BloomTex, Clamp, UV_VR) * _ScatteringParams.y;

		//return bloom;

		//Alpha is density, do not modify
		float4 fogColor = ComputeFog(i);

		fogColor.rgb = fogColor.rgb + bloom.rgb;

		screenColor.rgb = lerp(bloom.rgb, screenColor.rgb, fogColor.a);

		//Linear blend
		float3 blendedColor = lerp(fogColor.rgb, screenColor.rgb, fogColor.a);

		//Keep alpha channel for FXAA
		return float4(blendedColor.rgb, screenColor.a);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass //0
		{
			Name "Fog: Light Scattering Prefilter"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragPrefilter
			ENDHLSL
		}

			Pass //1
		{
			Name "Fog: Light Scattering Downsample"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragDownsample
			ENDHLSL
		}
			Pass //2
		{
			Name "Fog: Light Scattering Upsample"
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragUpsample
			ENDHLSL
		}
			Pass //3
		{
			Name "Fog: Composite"
			HLSLPROGRAM
			#pragma vertex VertWorldSpaceReconstruction
			#pragma fragment FragBlend
			ENDHLSL
		}
			Pass //4
		{
			Name "Fog: Light Scattering Composite"
			HLSLPROGRAM
			#pragma vertex VertWorldSpaceReconstruction
			#pragma fragment FragBlendScattering
			ENDHLSL
		}
	}
}