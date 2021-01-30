using System;

namespace Game
{
    namespace Selection
    {
        /// <summary>
        /// Hoverable Trait.
        /// </summary>
        public interface IHover
        {
            #region Events

            event Action HoverEnter_Event;
            event Action HoverExit_Event;

            #endregion

            #region Properties

            bool IsHovered { get; set; }

            #endregion

            #region Methods

            /// <summary>
            /// Called when hovered over.
            /// </summary>
            void OnHoverEnter();

            /// <summary>
            /// Called when no longer hovered over.
            /// </summary>
            void OnHoverExit();

            #endregion
        }
    }
}