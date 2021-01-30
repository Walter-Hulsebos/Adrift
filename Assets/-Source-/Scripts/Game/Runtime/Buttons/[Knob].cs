using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
    using Selection;
    
    namespace Buttons
    {
        public abstract class Knob : BaseSelectable
        {
            #region Fields

            [SerializeField] protected Transform frame, handle;

            [SerializeField] private bool useCurve;

            #endregion

            #region Events

            [PublicAPI]
            public event Action<float> OnValueChanged_Event;

            #endregion
            
            #region Properties

            private float _internalValue;
            
            /// <summary>
            /// Value from [0 - 1] that specifies at what percentage the knob is at.
            /// </summary>
            [PublicAPI]
            [field: SerializeField] 
            public float Value 
            {
                get => _internalValue;
                protected set
                {
                    if (value < 0f || value > 1f)
                    {
                        Debug.LogException(new ArgumentOutOfRangeException(nameof(value), value, message: "Setting knob value requires value from [0 - 1]"), context: this);
                        return;
                    }

                    _internalValue = value;

                    SetKnobPosition(value);
                    OnValueChanged_Event?.Invoke(value);
                } 
            }
            
            public float ValueBetweenRange(in float min, in float max) => Mathf.Lerp(min, max, Value);

            private static SelectionManager SelectionManager => SelectionManager.Instance;

            protected Vector3 MouseRelativeOnPlane
            {
                get
                {
                    Transform __frame = frame;
                    Plane __plane = new Plane(inPoint: __frame.position, inNormal: __frame.up);
                    Ray __ray = SelectionManager.MouseRay;

                    return __plane.Raycast(ray: __ray, enter: out float __distance) 
                        ? __ray.GetPoint(__distance) 
                        : Vector3.zero;
                }
            }

            #endregion
            
            #region Methods

            protected virtual void Reset()
            {
                frame  = transform.parent;
                handle = transform;
            }

            protected abstract void SetKnobPosition(in float percentValue);

            protected virtual void OnValueChanged(in float value)
            {
                OnValueChanged_Event?.Invoke(value);
            }

            #endregion
        }
    }
}
