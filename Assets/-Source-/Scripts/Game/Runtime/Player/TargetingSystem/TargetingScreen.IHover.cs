using System;

namespace Game
{
	using Selection;

	namespace Player.Targeting
	{
		public sealed partial class TargetingScreen : IHover
		{
			#region Manager

			protected static SelectionManager SelectionManager => SelectionManager.Instance;
			protected static bool SelectionManagerExists => SelectionManager.InstanceExists;
			
			#endregion
			
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
		}
	}
}