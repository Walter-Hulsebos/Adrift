using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonGames.Utilities.Singletons;

namespace Game
{
    public class EffectManager : PersistentSingleton<EffectManager>
    {
        [SerializeField] private GameObject explosionEffect;

        public void SpawnExplosion(Vector3 location)
        {
            Destroy(Instantiate(explosionEffect, location, Quaternion.identity), 1);
        }

        public void SpawnExplosion(Vector3 location, Color color)
        {
            GameObject effect;
            Destroy(effect = Instantiate(explosionEffect, location, Quaternion.identity), 1);

            effect.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
