using System;
using UnityEngine;

namespace Game
{
	using Selection;

	namespace Player.Targeting
	{

		public sealed class TargetingScreen : MonoBehaviour, IHover
		{
			#region Fields

			[SerializeField] private float radius = 1;
				
			[SerializeField] private Plane selectionPlane;

			#endregion

			#region Methods

			private void Reset()
			{
				//selectionPlane 
			}

			#endregion

			#region Hoverable

			public event Action HoverEnter_Event;
			public event Action HoverExit_Event;
			public bool IsHovered { get; set; }
			public void OnHoverEnter()
			{
				throw new NotImplementedException();
			}

			public void OnHoverExit()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Editor

			

			#endregion
		}
	}
}