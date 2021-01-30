using UnityEngine;

namespace Game
{

    namespace Buttons
    {
        public sealed partial class SliderHandle : Handle
        {
            #region Fields

            [Tooltip("How far back the slider's minimum position is")]
            [SerializeField] private float min = -0.5f;
            
            [Tooltip("How far past the minimum position the slider can move")]
            [Min(0)]
            [SerializeField] private float range = 1.0f;
            
            [Min(0)]
            [HideInInspector]
            [SerializeField] private float amountMoved = 0.5f;
  
            
            [SerializeField] private new Renderer renderer;
            
            [SerializeField] private bool snapping = false;

            [SerializeField] private float snapPercentages = 0.1f;

            #endregion

            #region Properties
            
            private float AmountMoved
            {
                get => amountMoved;
                set => amountMoved = value.Clamp(min: 0f, max: range);
            }
            
            private float AmountMovedPercentage => (AmountMoved / range);

            #endregion

            #region Methods

            private void Awake()
            {
                renderer ??= GetComponent<Renderer>();
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

            protected override void OnSelectUpdate()
            {
                Vector3 __mousePosOnAxis = ClosestUpAxisPosition(MouseRelativeOnPlane);

                float __distance = Vector3.Distance(__mousePosOnAxis, knob.position);
                float __dot = Vector3.Dot(frame.up, __mousePosOnAxis - knob.position); // dot product is -1 when vectors point in opposite directions
            
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
                knob.localPosition = __minPosition + (Vector3.up * AmountMoved);
                
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
