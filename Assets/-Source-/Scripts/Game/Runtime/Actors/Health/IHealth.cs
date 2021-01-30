using System;
using JetBrains.Annotations;

namespace Game
{
    namespace Actors
    {
        namespace Health
        {
            public interface IHealth
            {
                #region Properties

                /// <summary> Current HP. </summary>
                public int HealthPoints { get; }

                /// <summary>
                /// The percentage of health compared to the max.
                /// 
                /// <para>
                /// Range between [0 - 1]
                /// 0: at NO health
                /// 1: at MAX health.
                /// </para>
                /// 
                /// </summary>
                [PublicAPI]
                public float Percentage { get; }

                /// <summary>
                /// The percentage of health compared to the max. (Inverted)
                /// 
                /// <para>
                /// Range between [0 - 1]
                /// 0: at MAX health
                /// 1: at NO health.
                /// </para>
                /// 
                /// </summary>
                [PublicAPI]
                public float PercentageInverted { get; }

                [PublicAPI]
                public bool HasDamage { get; }

                [PublicAPI] 
                public bool IsDead { get; }


                #endregion

                #region Events
                
                /// <summary> Raised when the amount of health of the Actor changes. </summary>
                public event Action<int> OnHealthChanged_Event;
                
                /// <summary> Raised when the Actor is killed. </summary>
                public event Action OnDeath_Event;

                #endregion

                #region Methods

                /// <summary> Kills the Actor. </summary>
                public void Kill();

                #endregion
            }       
        }
    }
}