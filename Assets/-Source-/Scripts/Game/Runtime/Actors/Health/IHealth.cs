using System;

namespace Game
{
    namespace Actors
    {
        namespace Health
        {
            public interface IHealth
            {
                public int HealthPoints { get; }
    
                /// <summary> Kills the Actor. </summary>
                public void Kill();
        
                /// <summary> Raised when the amount of health of the Actor changes. </summary>
                public event Action<int> OnHealthChanged_Event;
                
                /// <summary> Raised when the Actor is killed. </summary>
                public event Action OnDeath_Event;
            }       
        }
    }
}