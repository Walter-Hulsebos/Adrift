using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JetBrains.Annotations;

namespace CGTK.Utilities.Singletons
{
	
	#if ODIN_INSPECTOR
	using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
	#else 
	using MonoBehaviour = UnityEngine.MonoBehaviour;
	#endif
	
	/// <summary> Singleton for <see cref="MonoBehaviour"/>s</summary>
	/// <typeparam name="T"> Type of the Singleton. </typeparam>
	public abstract class TestSingleton<T> : MonoBehaviour 
		where T : TestSingleton<T>
	{
		#region Properties

		private static T _internalInstance = null;

		/// <summary> The static reference to the Instance </summary>
		[PublicAPI]
		public static T Instance
		{
			get
			{
				if (InstanceExists) return _internalInstance;
				
				Debug.Log("SEARCHING! 2");
				
				_internalInstance = FindObjectOfType<T>();

				Debug.Log($"Internal Instance = {_internalInstance}");
				
				if (_internalInstance == null)
				{
					
				}
				
				return _internalInstance;
			}
			protected set => _internalInstance = value;
		}

		/// <summary> Whether a Instance of the Singleton exists </summary>
		[PublicAPI]
		public static bool InstanceExists => (_internalInstance != null);

		private bool InstanceIsCurrent => (_internalInstance == this);
		//private bool InstanceIsNotCurrent => !InstanceIsCurrent;

		#endregion

		#region Methods

		/// <summary> OnEnable method to associate Singleton with Instance </summary>
		protected virtual void Awake()
		{
			if (InstanceExists && !InstanceIsCurrent)
			{
				Destroy(Instance);
			}
			
			Instance = this as T;
		}

		/// <summary> OnDisable method to clear Singleton association </summary>
		protected virtual void OnDisable()
		{
			if (InstanceIsCurrent)
			{
				Instance = null;
			}
		}

		#endregion
	}
}
