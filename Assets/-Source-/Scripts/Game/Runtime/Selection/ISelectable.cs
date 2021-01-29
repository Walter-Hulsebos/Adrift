using System;

namespace Game
{
    /// <summary>
    /// Selectable Trait.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Called when selected.
        /// </summary>
        void OnSelect();
        
        /// <summary>
        /// Called when deselected.
        /// </summary>
        void OnDeselect();

        event Action Select_Event;
        event Action Deselect_Event;
    }
}