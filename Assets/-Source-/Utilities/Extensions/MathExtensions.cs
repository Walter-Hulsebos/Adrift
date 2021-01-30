using UnityEngine;

public static class MathExtensions
{
	public static float Clamp(this float value, in float min, in float max)
		=> Mathf.Clamp(value: value, min: min, max: max);
	
	public static float Clamp01(this float value)
		=> Mathf.Clamp01(value: value);

	public static float Lerp(this float t, in float from, in float to)
		=> Mathf.Lerp(from, to, t);

	//public static float Lerp(this (float min, float max), in float t) => 
}
