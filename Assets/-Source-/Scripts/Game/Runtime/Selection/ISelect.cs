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

            //event Action HoverEnter_Event;
            //event Action HoverExit_Event;

            #endregion

            #region Properties

            //bool IsHovered { get; set; }
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

            /*
            /// <summary>
            /// Called when hovered over.
            /// </summary>
            void OnHoverEnter();

            /// <summary>
            /// Called when no longer hovered over.
            /// </summary>
            void OnHoverExit();
            */

            #endregion
        }
    }
}