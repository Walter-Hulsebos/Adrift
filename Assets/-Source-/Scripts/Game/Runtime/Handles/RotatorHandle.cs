using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    namespace Buttons
    {
        public sealed partial class RotatorHandle : Handle
        {
            #region Fields

            [SerializeField] private bool noCap;
            
            [Tooltip("How far back the rotator's minimum rotation is")]
            [SerializeField] private float min = 0;
            //[SerializeField] private float max = 0;

            [Tooltip("How far past the minimum position the slider can move")]
            [Min(0)]
            [SerializeField] private float range = 360;

            [SerializeField] private Transform centerObject, mouseObject;
            
            //[Min(0)]
            //[HideInInspector]
            //[SerializeField] private float amountRotated = 0f;
  
            
            [SerializeField] private new Renderer renderer;
            
            [SerializeField] private bool snapping = false;

            [SerializeField] private float snapPercentages = 0.1f;

            private Vector3 _initialRotation;
            
            private Vector3 _currMousePosOnPlane, _prevMousePosOnPlane;
            private Vector3 _selectionStartOffset;

            #endregion

            #region Properties

            [HideInInspector]
            [SerializeField] private float internalAmountRotated;
            private float AmountRotated
            {
                get => internalAmountRotated;
                set
                {
                    internalAmountRotated = value.Clamp(min: min, max: min + range);
                    
                    frame.localEulerAngles = _initialRotation + (Vector3.up * AmountRotated);
                }
            }
            
            /*
            private float AmountRotated
            {
                get => amountRotated;
                set => amountRotated = value.Clamp(min: 0f, max: range);
            }
            */
            
            //private float AmountMovedPercentage => (AmountMoved / range);

            #endregion

            #region Methods

            private void Awake()
            {
                renderer ??= GetComponent<Renderer>();
                
                if (range < 0f)
                {
                    Debug.LogWarning("AngleRange should be positive", this);
                }

                if (AmountRotated < min || AmountRotated > min + range)
                {
                    Debug.LogWarning("Initial AmountRotated should be within angle range", this);
                }

                _initialRotation = frame.localEulerAngles;
                
                if (frame.localPosition != Vector3.zero)
                {
                    Debug.LogException(new System.InvalidOperationException("Knob knob position needs to be (0, 0, 0)"), this);
                    return;
                }

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

                _selectionStartOffset = (MouseRelativeOnPlane - frame.position);
            }
            public override void OnDeselect()
            {
                base.OnDeselect();
                
                renderer.transform.localScale = Vector3.one * 1f;
            }

            private Vector3 Center => frame.position;

            //TODO: LookAt Mouse Position.
            protected override void OnSelectUpdate()
            {
                _currMousePosOnPlane = MouseRelativeOnPlane - _selectionStartOffset;
                float __angleToRotate = Vector3.SignedAngle(from: _prevMousePosOnPlane - frame.position, to: _currMousePosOnPlane - frame.position, axis: frame.forward);
                _prevMousePosOnPlane = _currMousePosOnPlane;

                AmountRotated += __angleToRotate;
            }

            protected override void SetKnobPosition(in float percentValue)
            {
                //AmountMoved = Mathf.Lerp(0f, range, percentValue);
                
                //AmountRotated = Mathf.Lerp(MinAngle, MinAngle + AngleRange, percentValue);
                
                //SetRotationBasedOnAmountMoved();
            }

            #endregion
        }
    }
}
