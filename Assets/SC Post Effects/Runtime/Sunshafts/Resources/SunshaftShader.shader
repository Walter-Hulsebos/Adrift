Shader "Hidden/SC Post Effects/Sun Shafts"
{
	HLSLINCLUDE

	#define REQUIRE_DEPTH
	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_RT(_SunshaftBuffer);

	half _BlendMode;
	float4 _SunThreshold;
	float4 _SunColor;
	float4 _SunPosition;
	float _BlurRadius;

	struct v2f {
		float4 positionCS : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 texcoordStereo : TEXCOORD1;
		float2 blurDir : TEXCOORD2;
	};

	float4 FragSky(Varyings i) : SV_Target
	{
		float2 uv = UV.xy;

		float depth = LINEAR_DEPTH(SAMPLE_DEPTH(UV).r);
		float4 skyColor = SCREEN_COLOR(UV);

		half2 vec = _SunPosition.xy - uv;
		half dist = saturate(_SunPosition.w - length(vec.xy));

		float4 outColor = 0;
		//reject near depth pixels
		if (depth > 0.99) 
		{
			outColor = dot(max(skyColor.rgb - _SunThreshold.rgb, float3(0, 0, 0)), float3(1, 1, 1)) * dist;
		}

		return outColor;
	}

	v2f VertRadialBlur(Attributes v)
	{
		v2f o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

		o.positionCS = OBJECT_TO_CLIP(v);

		o.uv = GET_TRIANGLE_UV(v);
		o.uv = FlipUV(o.uv);
		o.uv = GET_TRIANGLE_UV_VR(o.uv, v.vertexID);

		o.blurDir = (_SunPosition.xy - o.uv.xy) * _BlurRadius;

		return o;
	}

	float4 FragRadialBlur(Varyings i) : SV_Target
	{
		half4 c = half4(0,0,0,0);

		float2 uv = UV;
		for (int s = 0; s < 12; s++)
		{
			half4 color = SCREEN_COLOR(uv);
			c += color;
			//uv.xy += i.blurDir;
			uv.xy += (_SunPosition.xy - uv.xy) * _BlurRadius;
		}
		return c / 12;
	}

	float4 FragBlend(Varyings i) : SV_Target
	{
		float4 screenColor = SCREEN_COLOR(UV_VR);
		//return screenColor;

		float3 sunshafts = SAMPLE_RT(_SunshaftBuffer, Clamp, UV).rgb;
		sunshafts.rgb *= _SunColor.rgb * _SunPosition.z;
		//return float4(sunshafts.rgb, screenColor.a);

		float3 blendedColor = 0;

		if (_BlendMode == 0) blendedColor = BlendAdditive(screenColor.rgb, sunshafts.rgb); //Additive blend
		if (_BlendMode == 1) blendedColor = BlendScreen(sunshafts.rgb, screenColor.rgb); //Screen blend

		return float4(blendedColor.rgb, screenColor.a);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Sunshafts sky mask"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragSky

			ENDHLSL
		}
		Pass
		{
			Name "Sunshafts blur"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragRadialBlur

			ENDHLSL
		}
		Pass
		{
			Name "Sunshafts composite"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlend

			ENDHLSL
		}
	}
}