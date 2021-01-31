Shader "Hidden/SC Post Effects/Blur"
{
	HLSLINCLUDE
	#define MULTI_PASS

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"
	#include "../../../Shaders/Blurring.hlsl"

	//Separate pass, because this shouldn't be looped
	float4 FragBlend(Varyings i) : SV_Target
	{
		STEREO_EYE_INDEX_POST_VERTEX(i);

		return ScreenColor(UV_VR);
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass //0
		{
			Name "Blur Blend"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlend

			ENDHLSL
		}
		Pass //1
		{
			Name "Gaussian Blur"
			HLSLPROGRAM

			#pragma vertex VertGaussian
			#pragma fragment FragBlurGaussian

			ENDHLSL
		}
		Pass //2
		{
			Name "Box Blur"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragBlurBox

			ENDHLSL
		}

	}
}