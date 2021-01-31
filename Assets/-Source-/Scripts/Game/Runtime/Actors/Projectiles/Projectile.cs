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

        [SerializeField] private int damage = 5;

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
            if(other.TryGetComponent(component: out Health __health))
            {
                if (this.team != __health.team)
                {
                    __health -= damage;   
                }
            }

            //TODO: Call explosion manager.
            
            Destroy(this);
        }

        #endregion
        
    }
}
