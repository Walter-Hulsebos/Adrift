using System;

namespace Game
{
    /// <summary>
    /// Selectable Trait.
    /// </summary>
    public interface ISelectable
    {
        #region Events

        event Action Select_Event;
        event Action Deselect_Event;

        #endregion

        #region Properties

        bool IsHovered { get; set; }
        bool IsPressed { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when selected.
        /// </summary>
        void OnSelect();
        
        /// <summary>
        /// Called when deselected.
        /// </summary>
        void OnDeselect();

        #endregion
    }
}