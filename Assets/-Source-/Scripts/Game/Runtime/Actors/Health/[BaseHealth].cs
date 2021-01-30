using System;

using JetBrains.Annotations;

using UnityEngine;

namespace Game
{
    namespace Actors
    {
        public abstract class BaseHealth : MonoBehaviour, IHealth
        {
            #region Properties
            
            /// <summary>
            /// The default and max health value.
            /// </summary>
            /// 
            /// <remarks>
            /// Override this if you want a different default health value than 100.
            /// </remarks>
            protected int Max => 100;
            
            private int HealthPointsInternal { get; set; }
            public int HealthPoints
            {
                get => HealthPointsInternal;
            
                protected set
                {
                    HealthPointsInternal = Mathf.Clamp(value: value, min: 0, max: Max);

                    OnHealthChanged();

                    if (HealthPointsInternal <= 0)
                    {
                        OnDeath();
                        Kill();   
                    }
                }
            }

            #endregion

            #region Events
            
            public event Action<int> OnHealthChanged_Event;
            public event Action OnDeath_Event;

            #endregion

            #region Methods

            [PublicAPI]
            public abstract void Kill();

            protected virtual void OnDeath()
            {
                OnDeath_Event?.Invoke();
            }

            protected virtual void OnHealthChanged()
            {
                OnHealthChanged_Event?.Invoke(HealthPoints);
            }

            #endregion

            #region Operators

            public static BaseHealth operator ++(in BaseHealth health)
            {
                health.HealthPoints++;
                return health;
            }
        
            public static BaseHealth operator --(in BaseHealth health)
            {
                health.HealthPoints--;
                return health;
            }
        
            public static BaseHealth operator +(in BaseHealth health, in int increment)
            {
                health.HealthPoints += increment;
                return health;
            }
        
            public static BaseHealth operator -(in BaseHealth health, in int decrement)
            {
                health.HealthPoints -= decrement;
                return health;
            }
    
            #endregion
        }
    }
}
