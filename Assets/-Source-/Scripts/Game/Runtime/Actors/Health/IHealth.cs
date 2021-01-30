using System;

namespace Game
{
    public interface IHealth
    {
        public int HealthPoints { get; }
    
        public void Kill();
        
        public event Action<int> OnHealthChanged_Event;
        public event Action OnDeath_Event;
    }
}