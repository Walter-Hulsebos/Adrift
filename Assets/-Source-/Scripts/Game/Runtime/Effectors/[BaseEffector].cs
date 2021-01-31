using System;
using UnityEngine;

namespace Game
{
    using Buttons;
    
    namespace Effectors
    {
        public abstract class BaseEffector : MonoBehaviour, IEffector
        {
            #region Properties

            [field: SerializeField]
            public Handle MyHandle { get; private set;  }

            #endregion

            #region Methods

            protected void Reset()
            {
                MyHandle = GetComponent<Handle>();
            }

            protected virtual void OnEnable()
            {
                MyHandle.OnValueChanged_Event += OnValueChanged;
            }

            protected virtual void OnDisable()
            {
                MyHandle.OnValueChanged_Event -= OnValueChanged;
            }

            public abstract void OnValueChanged(float percentage);

            #endregion
        }
    }
}
