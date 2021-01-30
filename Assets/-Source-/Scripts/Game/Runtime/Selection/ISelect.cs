using System;

namespace Game
{
    namespace Selection
    {
        /// <summary>
        /// Selectable Trait.
        /// </summary>
        public interface ISelect : IHover
        {
            #region Events

            event Action Select_Event;
            event Action Deselect_Event;

            #endregion

            #region Properties
            
            bool IsSelected { get; set; }

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
}