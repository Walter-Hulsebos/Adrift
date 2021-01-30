using System;
using UnityEngine;

namespace Game
{
    namespace Selection
    {
        public abstract class BaseSelectable : MonoBehaviour, ISelect
        {
            #region Events
    
            public event Action Select_Event;
            public event Action Deselect_Event;
            
            public event Action HoverEnter_Event;
            public event Action HoverExit_Event;
            
            #endregion
    
            #region States
    
            public bool IsHovered { get; set; }
            public bool IsSelected { get; set; }
            
            #endregion
    
            #region Methods

			public virtual void OnSelect() => Select_Event?.Invoke();
			public virtual void OnDeselect() => Deselect_Event?.Invoke();
			
			public virtual void OnHoverEnter() => HoverEnter_Event?.Invoke();
			public virtual void OnHoverExit() => HoverExit_Event?.Invoke();
			
			#endregion
        }
    }
}
