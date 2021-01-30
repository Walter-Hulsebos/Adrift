using JetBrains.Annotations;

namespace CommonGames.Utilities.Singletons
{
    
	#if ODIN_INSPECTOR
    using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
	#else 
	using MonoBehaviour = UnityEngine.MonoBehaviour;
	#endif
    
	/// <summary> Singleton for <see cref="MonoBehaviour"/>s</summary>
	/// <typeparam name="T"> Type of the Singleton. </typeparam>
	public abstract class Singleton<T> : MonoBehaviour 
		where T : Singleton<T>
	{
		#region Variables

		private static T _instance = null;

		/// <summary> The static reference to the Singleton's Instance </summary>
		[PublicAPI]
		public static T Instance
		{
			get => _instance; //??= FindObjectOfType<T>();
			protected set => _instance = value;
		}

		/// <summary> Whether a Instance of the Singleton exists </summary>
		[PublicAPI]
		public static bool InstanceExists => (_instance != null);

		#endregion

		#region Methods

		/// <summary> OnEnable method to associate Singleton with Instance </summary>
		protected virtual void OnEnable()
		{
			if (InstanceExists)
			{
				Destroy(gameObject);
			}
			else
			{
				Instance = this as T;
			}
		}

		/// <summary> OnDisable method to clear Singleton association </summary>
		protected virtual void OnDisable()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		#endregion
	}
}