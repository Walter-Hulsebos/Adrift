using System;
using UnityEngine;

namespace Game
{
    using Actors.Health;
    
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Projectile : MonoBehaviour
    {
        #region Fields

        [SerializeField] private byte team = 0;

        [SerializeField] private int damage = 10;

        public Rigidbody2D body;

        #endregion

        #region Methods

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            bool __hasShield = other.TryGetComponent(component: out PlayerShield __shield);
            if (__hasShield)
            {
                if (this.team != __shield.team)
                {
                    if (__shield.team == 1) ScreenshakeManager.Instance.ShakeHit(0);

                    __shield.Damage(damage);
                }
            }

            if (other.TryGetComponent(component: out Health __health) && (__hasShield && __shield.currentShieldHealth <= 0))
            {
                if (this.team != __health.team)
                {
                    if (__health.team == 1) ScreenshakeManager.Instance.ShakeHit(1);

                    __health -= damage;   
                }
            }

            //TODO: Call explosion manager.
            
            Destroy(this);
        }

        #endregion
        
    }
}
