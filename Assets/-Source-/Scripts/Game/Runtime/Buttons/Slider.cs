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
        public sealed class Slider : Knob
        {
            #region Fields

            [Tooltip("How far back the slider's minimum position is")]
            [SerializeField] private float min = -5;
            
            [Tooltip("How far past the minimum position the slider can move")]
            [Min(0)]
            [SerializeField] private float range = 10;

            [Tooltip("How far back the slider's minimum position is")]
            [Min(0)]
            [SerializeField] private float amountMoved = 5;

            [UsedImplicitly]
            private Vector3 _initialPosition;
            
            [SerializeField] private Renderer _renderer;
            
            //[PublicAPI] 
            //[field: SerializeField] public float Value { get; private set; }

            [SerializeField] private bool snapping = false;

            [SerializeField] private Transform DebugObject;

            //public float ValueBetweenRange(in float min, in float max) => Mathf.Lerp(min, max, Value);

            #endregion

            #region Properties

            private float AmountMovedPercentage => (amountMoved / range);

            #endregion

            #region Methods

            private void Awake()
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<Renderer>();
                }
            }

            private void Start()
            {
                _initialPosition = transform.localPosition;

                if (amountMoved < 0f || amountMoved > range)
                {
                    Debug.LogWarning(message: "Amount moved should be within the movement range", context: this);
                }
                
                OnValueChanged(AmountMovedPercentage);
            }

            public override void OnHoverEnter()
            {
                base.OnHoverEnter();

                _renderer.material.color = Color.red;
            }
            public override void OnHoverExit()
            {
                base.OnHoverExit();
                
                _renderer.material.color = Color.white;
            }
            
            public override void OnSelect()
            {
                base.OnSelect();
                _renderer.transform.localScale = Vector3.one * 1.1f;
            }
            public override void OnDeselect()
            {
                base.OnDeselect();
                _renderer.transform.localScale = Vector3.one * 1f;
            }

            private void Update()
            {
                if (IsSelected)
                {
                    Debug.Log("Selected");
                    
                    Vector3 __mousePosOnAxis = ClosestUpAxisPosition(MouseRelativeOnPlane);

                    DebugObject.position = __mousePosOnAxis;
                    
                    float __distance = Vector3.Distance(__mousePosOnAxis, handle.position);
                    float __dot = Vector3.Dot(frame.up, __mousePosOnAxis - handle.position); // dot product is -1 when vectors point in opposite directions
                
                    amountMoved += (__distance * (__dot < 0f ? -1f : 1f));

                    // clamp position to position range
                    amountMoved = amountMoved.Clamp(min: 0f, max: range);

                    // set the position of the transform based on position
                    SetPositionBasedOnAmountMoved();   
                }
            }

            protected override void SetKnobPosition(in float percentValue)
            {
                amountMoved = Mathf.Lerp(0f, range, percentValue);
            }
            
            private void SetPositionBasedOnAmountMoved()
            {
                Vector3 __minPosition = (Vector3.up * this.min);
                handle.localPosition = __minPosition + (Vector3.up * amountMoved);
                
                OnValueChanged(AmountMovedPercentage);
            }
            
            private Vector3 ClosestUpAxisPosition(in Vector3 point)
            {
                Ray __up = new Ray(origin: frame.position, direction: frame.up);
                return __up.origin + __up.direction * Vector3.Dot(__up.direction, point - __up.origin);
            }

            #endregion


            #region Editor

            [CustomEditor(typeof(Slider))]
            private sealed class SliderEditor : Editor
            {
                private Slider _slider;
                
                private void OnSceneGUI()
                {
                    float __min = serializedObject.FindProperty("min").floatValue;
                    float __range = serializedObject.FindProperty("range").floatValue;
                    float __amountMoved = serializedObject.FindProperty("amountMoved").floatValue;

                    _slider = (Slider)target;
                    
                    Vector3 __position = _slider.frame.position;
                    Vector3 __up       = _slider.frame.up;
                    Vector3 __right    = _slider.frame.right;
                    
                    Vector3 __localScale = _slider.frame.localScale;
                    
                    float __frameWidth = __localScale.x * 0.3f;
                    Handles.color = Color.green;
                    
                    
                    Vector3 __minPos = __position + (__up * __min * __localScale.z);
                    Vector3 __maxPos = __position + (__up * (__range + __min) * __localScale.z);
                    Handles.DrawLine(
                        p1: __minPos + (__right * __frameWidth),
                        p2: __minPos + (__right * -__frameWidth)
                    );

                    Handles.DrawLine(
                        p1: __maxPos + (__right * __frameWidth),
                        p2: __maxPos + (__right * -__frameWidth)
                    );

                    Handles.DrawLine(
                        __minPos,
                        __maxPos
                    );

                    // draw filled area to show current knob value
                    float __fillWidth = __localScale.x * 0.2f;
                    Handles.color = Color.red;
                    
                    Vector3 __movedToPos = __position + (__up * (__min + __amountMoved) * __localScale.z);
                    Handles.DrawSolidRectangleWithOutline(
                        verts: new[] 
                        {
                            __minPos + (__right * __fillWidth),
                            __minPos + (__right * -__fillWidth),
                            __movedToPos + (__right * -__fillWidth),
                            __movedToPos + (__right * __fillWidth)
                        },
                        new Color(1f, 0.5f, 0.5f, 0.1f),
                        Color.blue
                    );
                }
            }

            #endregion
        }
    }
}
