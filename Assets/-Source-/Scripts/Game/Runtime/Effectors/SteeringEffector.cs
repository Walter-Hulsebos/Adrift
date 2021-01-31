using UnityEngine;

namespace Game
{
	namespace Effectors
	{
		public sealed class SteeringEffector : BaseEffector
		{
			#region Fields

			[SerializeField] private float min = -1, max = 1;

			#endregion

			#region Methods

			public override void OnValueChanged(float percentage)
			{
				PlayerController.Instance.rotationSlider = percentage.Lerp(min, max);
			}

			#endregion
		}
		
	}
}