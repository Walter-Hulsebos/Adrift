using System;

using JetBrains.Annotations;

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#else
using MonoBehaviour = UnityEngine.MonoBehaviour;
#endif

namespace Game
{
    namespace Actors
    {
        namespace Health
        {
            public abstract class Health : MonoBehaviour, IHealth
            {
                #region Properties

                [field: SerializeField] 
                public byte team;
                
                /// <summary>
                /// The default and max health value.
                /// </summary>
                /// 
                /// <remarks>
                /// Override this if you want a different default health value than 100.
                /// </remarks>
                protected int Max => 100;

                private int HealthPointsInternal { get; set; }
                
                #if ODIN_INSPECTOR
                [ProgressBar(min: 0, maxGetter: nameof(Max), ColorGetter = "GetHealthBarColor")]
                [OdinSerialize]
                #else
                [field: SerializeField]
                #endif
                public int HealthPoints
                {
                    get => HealthPointsInternal;

                    protected set
                    {
                        HealthPointsInternal = Mathf.Clamp(value: value, min: 0, max: Max);

                        OnHealthChanged();

                        if (IsDead && !AlreadyDead)
                        {
                            OnDeath();
                            Kill();
                        }
                    }
                }
                
                [PublicAPI]
                public float Percentage => Mathf.Clamp01(value: HealthPoints / (float)Max);
                
                [PublicAPI]
                public float PercentageInverted => (1 - Percentage);

                [PublicAPI]
                public bool HasDamage => (HealthPoints < Max);

                [PublicAPI] 
                public bool IsDead => (HealthPoints <= 0);
                
                protected bool AlreadyDead { get; set; } = false;

                #endregion

                #region Events

                public event Action<int> OnHealthChanged_Event;
                public event Action OnDeath_Event;

                #endregion

                #region Methods

                [PublicAPI]
                public abstract void Kill();

                protected virtual void Reset() => ResetHealth();
                protected virtual void Awake() => ResetHealth();

                protected virtual void ResetHealth()
                {
                    HealthPoints = Max;
                }

                protected virtual void OnDeath()
                {
                    OnDeath_Event?.Invoke();
                    AlreadyDead = true;
                }

                protected virtual void OnHealthChanged()
                {
                    OnHealthChanged_Event?.Invoke(HealthPoints);
                }

                #endregion

                #region Operators

                public static implicit operator int(in Health health) => health.HealthPoints;
                
                public static Health operator ++(in Health health)
                {
                    health.HealthPoints++;
                    return health;
                }

                public static Health operator --(in Health health)
                {
                    health.HealthPoints--;
                    return health;
                }

                public static Health operator +(in Health health, in int increment)
                {
                    health.HealthPoints += increment;
                    return health;
                }

                public static Health operator -(in Health health, in int decrement)
                {
                    health.HealthPoints -= decrement;
                    return health;
                }

                #endregion

                #region MyRegion

                private Color GetHealthBarColor(float value) => Color.Lerp(Color.red, Color.green, t: Percentage);

                #endregion
            }
        }
    }
}
