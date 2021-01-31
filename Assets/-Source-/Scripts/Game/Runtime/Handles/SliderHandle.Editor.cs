using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{

    namespace Buttons
    {
        public sealed partial class SliderHandle
        {
            #if UNITY_EDITOR
            
            [CustomEditor(typeof(SliderHandle))]
            private sealed class SliderEditor : Editor
            {
                private SliderHandle _slider;
                
                private void OnSceneGUI()
                {
                    float __min = serializedObject.FindProperty(nameof(min)).floatValue;
                    float __range = serializedObject.FindProperty(nameof(range)).floatValue;
                    float __amountMoved = serializedObject.FindProperty(nameof(amountMoved)).floatValue;

                    _slider = (SliderHandle)target;
                    
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
            
            #endif
            
        }
    }
}
