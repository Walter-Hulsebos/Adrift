using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
    public class Actor : MonoBehaviour
    {
        #region Fields

        [field: SerializeField]
        public Rigidbody2D Body { get; protected set; }
        
        private PolygonCollider2D _collider;
        
        //[field: SerializeField]
        //public PolygonCollider2D Collider { get; protected set; }
        
        private const float _MAX_THRUST = 1;

        [SerializeField] protected float rotSpeed = 320;
        [SerializeField] protected float maxSpeed = 40;
        [SerializeField] protected float acceleration = 10;

        private const float _FIRE_DELAY = 0.11f;
        
        private float _nextFire = 0;
        private float _nextRegen = 0;
        private float _nextEnvDamage = 0;
        
        #endregion

        #region Properties
        
        public Vector3 Position => Body.transform.position;

        public bool IsAccelerating => (_thrustInput > 0);
        
        /// <summary> Direction the actor is traveling in. </summary>
        [PublicAPI]
        public Vector2 VelDir => Body.velocity.normalized;

        /// <summary> Direction the actor is aimed in. </summary>
        [PublicAPI]
        public Vector2 AimDir => Body.transform.up;

        /// <summary> Current speed relative to the max speed. </summary>
        public float VelDelta => (Body.velocity.magnitude / maxSpeed);

        #endregion

        #region Methods

        private void Reset()
        {
            Body = GetComponent<Rigidbody2D>();
            _collider = Body.GetComponent<PolygonCollider2D>();
        }

        private void Start()
        {
            Body ??= GetComponent<Rigidbody2D>();
            _collider ??= Body.GetComponent<PolygonCollider2D>();
        }

        #endregion
    }
}
