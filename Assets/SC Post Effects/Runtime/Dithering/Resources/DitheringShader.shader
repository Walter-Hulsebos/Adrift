Shader "Hidden/SC Post Effects/Dithering"
{
	HLSLINCLUDE

	#include "../../../Shaders/Pipeline/Pipeline.hlsl"

	DECLARE_TEX(_LUT);

	float4 _Dithering_Coords;
	//X: Size
	//Y: Tiling
	//Z: Luminance influence
	//W: Intensity

	float4 Frag(Varyings i) : SV_Target
	{
		float4 screenColor = SCREEN_COLOR(UV_VR);
#ifdef URP
		//(URP) Because effects can only execute before built-in effects, apply tonemapping to get LDR color
		screenColor.rgb = NeutralTonemap(screenColor.rgb);
#endif
		float luminance = Luminance(screenColor.rgb);

		float2 lutUV = float2(UV.x *= _ScreenParams.x / _ScreenParams.y, UV.y * _ScreenParams.w);

		float lut = SAMPLE_TEX(_LUT, Repeat, lutUV *_Dithering_Coords.y * 32).r;

		float dither = step(lut, luminance / _Dithering_Coords.z);

		return lerp(screenColor, screenColor * saturate(dither), _Dithering_Coords.w);

	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Name "Dithering"
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			ENDHLSL
		}
	}
}