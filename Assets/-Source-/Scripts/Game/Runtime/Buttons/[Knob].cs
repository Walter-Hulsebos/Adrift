using UnityEngine;

namespace Game
{
    using Selection;
    
    namespace Buttons
    {
        public abstract class Knob : BaseSelectable
        {
            #region Fields

            [SerializeField] protected Transform center;

            #endregion

            #region Properties

            protected Vector3 MouseRelativeOnPlane
            {
                get
                {
                    Transform __knob = transform;
                    Plane __knobPlane = new Plane(inPoint: __knob.position, inNormal: __knob.up);
                    
                    return Vector3.zero;
                }
            }

            #endregion
            
            #region Methods

            

            #endregion
        }
    }
}
