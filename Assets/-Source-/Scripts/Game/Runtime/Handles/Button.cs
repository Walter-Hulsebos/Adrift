using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    using Selection;

    namespace Buttons
    {
        public sealed class Button : BaseSelectable, IButton<bool>
        {
            #region Fields

            [SerializeField] private new Renderer renderer;

            #endregion

            #region Properties

            [field: SerializeField]
            public Transform Frame { get; protected set; }
            [field: SerializeField]
            public Transform Knob { get; protected set; }
            
            public bool Value { get; protected set; } = false;

            public UnityEvent OnClick_Event;

            #endregion

            #region Methods

            private void Awake()
            {
                renderer ??= GetComponent<Renderer>();
            }

                #region Selectable

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
                
                #endregion

            #endregion
        }
    }
}
