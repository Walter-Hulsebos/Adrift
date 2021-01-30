using UnityEngine;

namespace Game
{
    namespace Actors
    {
        using Health;
        
        public sealed class EnemyHealth : Health.Health
        {
            public override void Kill()
            {
                Debug.Log("I AM DEAD!");
            }
        }
    }
}
