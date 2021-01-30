using UnityEngine;
using UnityEngine.Audio;

namespace Game
{
	namespace Effectors
	{
		public sealed class AudioMixerEffector : BaseEffector
		{
			#region Fields

			[SerializeField] private AudioMixer mixer;

			[SerializeField] private string paramName;

			[SerializeField] private float minValue = -80;

			[SerializeField] private float maxValue = 0f;

			#endregion

			#region Methods

			public override void OnValueChanged(float percentage)
			{
				mixer.SetFloat(name: paramName, value: percentage.Lerp(minValue, maxValue));
			}

			#endregion
		}
		
	}
}