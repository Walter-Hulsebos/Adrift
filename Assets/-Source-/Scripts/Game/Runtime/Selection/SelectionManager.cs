using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    namespace Selection
    {
        public sealed class SelectionManager : MonoBehaviour
        {
            #region Fields

            [SerializeField] private Camera selectionCamera;

            [SerializeField] private LayerMask selectablesLayer = 1 << 9;
            
            private Controls PlayerControls { get; set; }

            private ISelectable _currentHovering = null;
            private bool _isHoveringAnything;

            private ISelectable _currentSelected = null;
            private bool _isSelectingAnything;

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

            private void OnEnable()
            {
                PlayerControls.Enable();
                
                PlayerControls.Cursor.Select.performed += _ => Debug.Log("Performed");
                //PlayerControls.Cursor.Select.started += _ => Debug.Log("Started");
                //PlayerControls.Cursor.Select.canceled += _ => Debug.Log("Canceled");
            }

            private void OnDisable()
            {
                PlayerControls.Disable();
            }

            private void Update()
            {
                HandleHovering();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void HandleHovering()
            {
                Ray __cursorRay = selectionCamera.ScreenPointToRay(pos: Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray: __cursorRay, hitInfo: out RaycastHit __hit, maxDistance: Mathf.Infinity, layerMask: selectablesLayer))
                {
                    bool __hoveringAnySelectable = __hit.transform.TryGetComponent(component: out ISelectable __newSelectable);

                    bool __hoveringDifferentSelectable = (_currentHovering != __newSelectable);

                    bool __exitedCurrentHovering = (!__hoveringAnySelectable || __hoveringDifferentSelectable);

                    if (__exitedCurrentHovering)
                    {
                        __HoverExit();
                    }

                    if (__hoveringAnySelectable && __hoveringDifferentSelectable)
                    {
                        __HoverEnter(__newSelectable);
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
                
                void __HoverEnter(in ISelectable newSelectable)
                {
                    _currentHovering = newSelectable;

                    _currentHovering.OnHoverEnter();
                    _currentHovering.IsHovered = true;

                    _isHoveringAnything = true;
                }
            }


            private void HandleSelection()
            {
                void Select()
                {
                
                }

                void Deselect()
                {
                
                }   
            }

            /*
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void HandlePressing()
            {
                if (_isHoveringAnything && Input.GetMouseButtonDown(0))
                {
                    _currentSelected = _currentHovering;

                    if (_currentSelected.Deselect_Event != null)
                    {

                    }

                    _currentSelected.Select_Event?.Invoke();
                    _currentSelected.IsSelected = true;

                    _isSelectingAnything = true;
                }

                if (_isSelectingAnything && Input.GetMouseButtonUp(0))
                {
                    _currentSelected.Deselect_Event?.Invoke();
                    _currentSelected.IsSelected = false;
                    _currentSelected = null;

                    _isSelectingAnything = false;
                }
            }
            */

            #endregion

        }
    }
}
