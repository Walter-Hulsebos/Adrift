namespace Game
{
	using Selection;

	namespace Player.Targeting
	{
		public sealed partial class TargetingScreen // : IHover
		{
			protected static SelectionManager SelectionManager => SelectionManager.Instance;
			protected static bool SelectionManagerExists => SelectionManager.InstanceExists;
		}
	}
}