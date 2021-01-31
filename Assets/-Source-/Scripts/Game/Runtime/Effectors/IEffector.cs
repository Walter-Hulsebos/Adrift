namespace Game
{
	using Buttons;
	
	namespace Effectors
	{
		public interface IEffector
		{
			Handle MyHandle { get; }
			
			void OnValueChanged(float percentage);
		}
	}
}
