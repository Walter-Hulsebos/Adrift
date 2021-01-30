using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{

    namespace Buttons
    {
        public sealed partial class Slider : Knob
        {
            #region Fields

            [Tooltip("How far back the slider's minimum position is")]
            [SerializeField] private float min = -0.5f;
            
            [Tooltip("How far past the minimum position the slider can move")]
            [Min(0)]
            [SerializeField] private float range = 1.0f;
            
            [Min(0)]
            [HideInInspector]
            [SerializeField]
            private float _amountMoved = 0;

            [UsedImplicitly]
            private Vector3 _initialPosition;
            
            [SerializeField] private Renderer renderer;
            
            [SerializeField] private bool snapping = false;

            [SerializeField] private float snapPercentages = 0.1f;

            #endregion

            #region Properties

            [field: SerializeField]
            private float AmountMoved
            {
                get => _amountMoved;
                set => _amountMoved = value.Clamp(min: 0f, max: range);
            }
            
            private float AmountMovedPercentage => (AmountMoved / range);

            #endregion

            #region Methods

            private void Awake()
            {
                renderer ??= GetComponent<Renderer>();
            }

            private void Start()
            {
                _initialPosition = transform.localPosition;

                if (_amountMoved < 0f || _amountMoved > range)
                {
                    Debug.LogWarning(message: "Amount moved should be within the movement range", context: this);
                }
                
                OnValueChanged(AmountMovedPercentage);
            }

            public override void OnHoverEnter()
            {
                base.OnHoverEnter();
                
                renderer.material.color = Color.red;
            }
            public override void OnHoverExit()
            {
                base.OnHoverExit();
                
                renderer.material.color = Color.white;
            }
            
            public override void OnSelect()
            {
                base.OnSelect();
                
                renderer.transform.localScale = Vector3.one * 1.1f;
            }
            public override void OnDeselect()
            {
                base.OnDeselect();
                
                renderer.transform.localScale = Vector3.one * 1f;
            }

            private void OnDrawGizmos()
            {
                if (SelectionManagerExists)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(SelectionManager.MouseRay);

                    Gizmos.color = Color.yellow;
                    Plane __plane = new Plane(inPoint: frame.position, inNormal: frame.up);
                    Gizmos.DrawRay(from: frame.position, direction: __plane.normal);
                }
            }

            protected override void OnSelectUpdate()
            {
                Vector3 __mousePosOnAxis = ClosestUpAxisPosition(MouseRelativeOnPlane);

                float __distance = Vector3.Distance(__mousePosOnAxis, handle.position);
                float __dot = Vector3.Dot(frame.up, __mousePosOnAxis - handle.position); // dot product is -1 when vectors point in opposite directions
            
                AmountMoved += (__distance * (__dot < 0f ? -1f : 1f));
                
                // set the position of the transform based on position
                SetPositionBasedOnAmountMoved();
            }

            protected override void SetKnobPosition(in float percentValue)
            {
                AmountMoved = Mathf.Lerp(0f, range, percentValue);
            }
            
            private void SetPositionBasedOnAmountMoved()
            {
                Vector3 __minPosition = (Vector3.up * this.min);
                handle.localPosition = __minPosition + (Vector3.up * AmountMoved);
                
                OnValueChanged(AmountMovedPercentage);
            }
            
            private Vector3 ClosestUpAxisPosition(in Vector3 point)
            {
                Ray __up = new Ray(origin: frame.position, direction: frame.up);
                return __up.origin + __up.direction * Vector3.Dot(__up.direction, point - __up.origin);
            }

            #endregion
        }
    }
}
