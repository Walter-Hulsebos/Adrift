using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGTK.Utilities.Singletons
{
    
    #if ODIN_INSPECTOR
    using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour; 
    #else 
    using MonoBehaviour = UnityEngine.MonoBehaviour;
    #endif
    
    /// <summary> Singleton for <see cref="MonoBehaviour"/>s - that persists across scenes. </summary>
    /// <typeparam name="T"> Type of the Singleton. </typeparam>
    public abstract class PersistentTestSingleton<T> : TestSingleton<T> 
        where T : PersistentTestSingleton<T>
    {
        protected override void Awake()
        {
            base.Awake();

            if(Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}