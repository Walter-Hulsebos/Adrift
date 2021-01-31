using UnityEngine;

namespace Game
{
	namespace Effectors
	{
		public sealed class ThrustEffector : BaseEffector
		{
			#region Fields

			[SerializeField] private float min = 0, max = 1;

			#endregion

			#region Methods

			public override void OnValueChanged(float percentage)
			{
				PlayerController.Instance.accelerationSlider = percentage.Lerp(min, max);
			}

			#endregion
		}
		
	}
}