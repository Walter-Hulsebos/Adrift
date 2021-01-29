// GENERATED AUTOMATICALLY FROM 'Assets/-Source-/Settings/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Cursor"",
            ""id"": ""b2484957-de7d-4e54-b09d-14be6e73babd"",
            ""actions"": [
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""32905103-559e-4eae-b548-14ad5bfa63e3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""6d56b4c1-dbca-4b00-932f-41440b6f8d61"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d6c774c7-f969-4b64-9ff0-aabc3fad6476"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56a8ae1c-2f9e-456f-b482-db63b0bfb5f0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gameplay"",
            ""bindingGroup"": ""Gameplay"",
            ""devices"": []
        }
    ]
}");
        // Cursor
        m_Cursor = asset.FindActionMap("Cursor", throwIfNotFound: true);
        m_Cursor_Position = m_Cursor.FindAction("Position", throwIfNotFound: true);
        m_Cursor_Select = m_Cursor.FindAction("Select", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Cursor
    private readonly InputActionMap m_Cursor;
    private ICursorActions m_CursorActionsCallbackInterface;
    private readonly InputAction m_Cursor_Position;
    private readonly InputAction m_Cursor_Select;
    public struct CursorActions
    {
        private @Controls m_Wrapper;
        public CursorActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Position => m_Wrapper.m_Cursor_Position;
        public InputAction @Select => m_Wrapper.m_Cursor_Select;
        public InputActionMap Get() { return m_Wrapper.m_Cursor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CursorActions set) { return set.Get(); }
        public void SetCallbacks(ICursorActions instance)
        {
            if (m_Wrapper.m_CursorActionsCallbackInterface != null)
            {
                @Position.started -= m_Wrapper.m_CursorActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_CursorActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_CursorActionsCallbackInterface.OnPosition;
                @Select.started -= m_Wrapper.m_CursorActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_CursorActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_CursorActionsCallbackInterface.OnSelect;
            }
            m_Wrapper.m_CursorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
            }
        }
    }
    public CursorActions @Cursor => new CursorActions(this);
    private int m_GameplaySchemeIndex = -1;
    public InputControlScheme GameplayScheme
    {
        get
        {
            if (m_GameplaySchemeIndex == -1) m_GameplaySchemeIndex = asset.FindControlSchemeIndex("Gameplay");
            return asset.controlSchemes[m_GameplaySchemeIndex];
        }
    }
    public interface ICursorActions
    {
        void OnPosition(InputAction.CallbackContext context);
        void OnSelect(InputAction.CallbackContext context);
    }
}
