using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private Targeter targeter;

        [SerializeField] private Projectile projectilePrefab;

        #region Properties

        [field: SerializeField]
        public Rigidbody2D Body { get; protected set; }
        
        [PublicAPI]
        public Transform ActorTransform => Body.transform;
        
        [PublicAPI]
        public Vector2 ActorPosition => Body.position;
        
        /// <summary> Direction the actor is traveling in. </summary>
        [PublicAPI]
        public Vector2 VelDir => Body.velocity.normalized;

        /// <summary> Direction the actor is aimed in. </summary>
        [PublicAPI]
        public Vector2 AimDir => ActorTransform.up;

        [PublicAPI]
        public Vector2 DirectionToTarget => targeter.HasTarget ? (Vector2)(targeter.CurrentTarget.transform.position - this.transform.position) : AimDir;

        #endregion

        private void Reset()
        {
            Body = GetComponent<Rigidbody2D>();
            targeter = GetComponent<Targeter>();
        }

        private void Start()
        {
            Body = GetComponent<Rigidbody2D>();
            
            if (targeter == null)
            {
                targeter = GetComponent<Targeter>();
            }
        }

        [PublicAPI]
        public void Fire()
        {
            Vector2 __bulletSpawnPos = ActorPosition + AimDir * 2.5f;
                
            Vector3 __bulletDirection = DirectionToTarget;

            float __angle = Mathf.Atan2(__bulletDirection.y, __bulletDirection.x) * Mathf.Rad2Deg;
            Quaternion __rotation = Quaternion.AngleAxis(__angle, Vector3.forward);
            
            Projectile __projectile = Instantiate(original: projectilePrefab, position: __bulletSpawnPos, rotation: __rotation);
            __projectile.GetComponent<Rigidbody2D>().velocity = __bulletDirection * 500 * Time.deltaTime;  //* Body.velocity.magnitude * 2; 

            //TODO: Muzzle and Sound.
            //MuzzleFlash();
            //playSound(0, 0.5f);
        }
    }
}
