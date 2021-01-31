using Cinemachine;
using UnityEngine;

namespace Game
{
	namespace Effectors
	{
		public sealed class ThrustEffector : BaseEffector
		{
			#region Fields

			[SerializeField] private float min = 0, max = 1;
			
			[Space]
			[SerializeField] private CinemachineVirtualCamera _virtualCamera;

			
			[SerializeField] private NoiseSettings noiseStill;
			[SerializeField] private float stillAmplitude, stillFrequencyGain;

			[SerializeField] private NoiseSettings noiseFlight;
			[SerializeField] private float flightAmpMin, flightAmpMax;
			[SerializeField] private float flightFreqMin, flightFreqMax;
			#endregion

			#region Methods

			public override void OnValueChanged(float percentage)
			{
				PlayerController.Instance.accelerationSlider = percentage.Lerp(min, max);

				if (_virtualCamera == null) return;
				
				CinemachineBasicMultiChannelPerlin __noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

				if (percentage <= 0)
				{
					__noise.m_NoiseProfile = noiseStill;
				}
				else
				{
					__noise.m_NoiseProfile = noiseFlight;

					__noise.m_AmplitudeGain = percentage.Lerp(flightAmpMin, flightAmpMax);
					__noise.m_FrequencyGain = percentage.Lerp(flightFreqMin, flightFreqMax);
				}
			}

			#endregion
		}
		
	}
}