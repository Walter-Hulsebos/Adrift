using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.InputSystem;

using CGTK.Utilities.Singletons;
using JetBrains.Annotations;

namespace Game
{
    namespace Selection
    {
        public sealed class SelectionManager : Singleton<SelectionManager>
        {
            #region Fields

            [SerializeField] private Camera selectionCamera;

            [SerializeField] private LayerMask selectablesLayer = 1 << 9;
            
            private Controls PlayerControls { get; set; }

            private IHover _currentHovering = null;
            private bool _isHoveringAnything;

            private ISelect _currentSelected = null;
            private bool _isSelectingAnything;

            #endregion

            #region Properties

            [PublicAPI]
            public Vector2 MousePosition => PlayerControls.Cursor.Position.ReadValue<Vector2>();
            [PublicAPI]
            public Ray MouseRay => selectionCamera.ScreenPointToRay(pos: MousePosition);

            #endregion
            
            #region Methods

            private void Reset()
            {
                selectionCamera = Camera.main;
            }

            private void Awake()
            {
                PlayerControls = new Controls();
            }

            private void Start()
            {
                selectionCamera ??= Camera.main;
            }

            protected override void OnEnable()
            {
                base.OnEnable();
                
                PlayerControls.Enable();
                
                PlayerControls.Cursor.Select.started += _ => Select();
                PlayerControls.Cursor.Select.canceled += _ => Deselect();
            }

            protected override void OnDisable()
            {
                base.OnDisable();
                
                PlayerControls.Disable();
            }

            private void Update()
            {
                HandleHovering();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void HandleHovering()
            {
                //Ray __cursorRay = selectionCamera.ScreenPointToRay(pos: Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray: MouseRay, hitInfo: out RaycastHit __hit, maxDistance: Mathf.Infinity, layerMask: selectablesLayer))
                {
                    bool __hoveringAny = __hit.transform.TryGetComponent(component: out IHover __newHoverable);

                    bool __hoveringDifferent = (_currentHovering != __newHoverable);

                    bool __exitedCurrentHovering = (!__hoveringAny || __hoveringDifferent);

                    if (__exitedCurrentHovering)
                    {
                        __HoverExit();
                    }

                    if (__hoveringAny && __hoveringDifferent)
                    {
                        __HoverEnter(__newHoverable);
                    }
                }
                else
                {
                    __HoverExit();
                }
                
                void __HoverExit()
                {
                    if (!_isHoveringAnything) return;
                
                    _currentHovering.OnHoverExit();
                    _currentHovering.IsHovered = false;

                    _currentHovering = null;

                    _isHoveringAnything = false;
                }
                
                void __HoverEnter(in IHover newHoverable)
                {
                    _currentHovering = newHoverable;

                    _currentHovering.OnHoverEnter();
                    _currentHovering.IsHovered = true;

                    _isHoveringAnything = true;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Select()
            {
                if (!_isHoveringAnything) return; //Nothing to select.
                
                if (!(_currentHovering is ISelect __selectable)) return;
                    
                _currentSelected = __selectable;
                _isSelectingAnything = true;

                _currentSelected.OnSelect();
                _currentSelected.IsSelected = true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Deselect()
            {
                if(!_isSelectingAnything) return; //Nothing to deselect.
                
                _currentSelected.OnDeselect();
                _currentSelected.IsSelected = false;
                
                _currentSelected = null;
                _isSelectingAnything = false;
            }

            #endregion

        }
    }
}
