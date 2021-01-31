using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{

    namespace Buttons
    {
        public sealed partial class RotatorHandle
        {
            
            [CustomEditor(typeof(RotatorHandle))]
            private sealed class RotatorEditor : Editor
            {
                private RotatorHandle _rotator;
                
                private void OnSceneGUI()
                {
                    // Get serialized private values
                    float __minAngle = serializedObject.FindProperty(nameof(min)).floatValue;
                    float __angleRange = serializedObject.FindProperty(nameof(range)).floatValue;
                    float __amountRotated = serializedObject.FindProperty(nameof(internalAmountRotated)).floatValue;

                    _rotator = (RotatorHandle)target;

                    Transform __transform = _rotator.frame.transform;
                    Vector3 __position = __transform.position;
                    
                    Vector3 __startingAngle = Quaternion.AngleAxis(__minAngle, __transform.up) * __transform.forward;

                    float __size = __transform.localScale.magnitude;
                    float __radiusIncrement = __size / 10f;

                    // draw filled angle to show current rotation value
                    Handles.color = new Color(1f, 0f, 0f, 0.5f);
                    float __radius = __size;
                    float __filledAngleToDraw = __amountRotated - __minAngle;
                    while (__filledAngleToDraw > 360f)
                    {
                        Handles.DrawSolidArc(__position, __transform.up, __startingAngle, 360f, __radius);
                        __filledAngleToDraw -= 360f;
                        __radius += __radiusIncrement;
                    }
                    Handles.DrawSolidArc(__position, __transform.up, __startingAngle, __filledAngleToDraw, __radius);

                    // draw unfilled angle to show entire range of motion
                    Handles.color = Color.black;
                    __radius = __size;
                    float __anglesToDraw = __angleRange;
                    while (__anglesToDraw > 360f)
                    {
                        Handles.DrawWireArc(__position, __transform.up, __startingAngle, 360f, __radius);
                        __anglesToDraw -= 360f;
                        __radius += __radiusIncrement;
                    }
                    Handles.DrawWireArc(__position, __transform.up, __startingAngle, __anglesToDraw, __radius);

                    // draw min angle as a line
                    Handles.color = Color.yellow;
                    Handles.DrawLine(__position, __position + (__startingAngle * __radius));
                }
            }
        }
    }
}
