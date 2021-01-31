using UnityEngine;

namespace Game.Actors.Health
{
    public sealed class EnemyHealth : Health
    {

        public override void Kill()
        {
            EffectManager.Instance.SpawnExplosion(transform.position, Color.red);
        }
    }
}
