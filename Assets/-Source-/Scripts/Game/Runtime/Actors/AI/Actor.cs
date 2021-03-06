using System;
using UnityEngine;

using JetBrains.Annotations;

#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;

using MonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;
#else
using MonoBehaviour = UnityEngine.MonoBehaviour;
#endif

namespace Game
{
    public class Actor : MonoBehaviour, IActor
    {
        #region Fields

        public bool IsActivated => true;
        
        [field: SerializeField]
        public Rigidbody2D Body { get; protected set; }

        public byte team;
        
        private PolygonCollider2D _collider;

        #if ODIN_INSPECTOR
        [AssetsOnly]
        #endif
        [SerializeField] protected Projectile projectilePrefab;

        private float _currThrust = 0;
        private float _maxThrust = 1;

        protected float accelerationMultiplier = 1;
        protected float maxSpeedMultiplier = 1;

        [SerializeField] protected float rotSpeed = 175;
        [SerializeField] protected float maxSpeed = 40;
        [SerializeField] protected float acceleration = 10;

        [SerializeField] protected float fireDelay = 0.4f;
        
        private float _nextFire = 0;

        #region Inputs
    
        protected float rotInput = 0;
        protected float thrustInput = 0;

        #endregion
        
        #endregion

        #region Properties
        
        [PublicAPI]
        public Transform ActorTransform => Body.transform;
        
        [PublicAPI]
        public Vector2 ActorPosition => Body.position;

        [PublicAPI] 
        public float ActorRotation => Body.rotation;

        [OdinSerialize]
        public bool IsAccelerating => (thrustInput > 0);

        [OdinSerialize]
        public bool IsRotating => (rotInput != 0);
        
        /// <summary> Direction the actor is traveling in. </summary>
        [PublicAPI]
        public Vector2 VelDir => Body.velocity.normalized;

        /// <summary> Direction the actor is aimed in. </summary>
        [PublicAPI]
        public Vector2 AimDir => ActorTransform.up;

        /// <summary> Current speed relative to the max speed. </summary>
        public float VelDelta => (Body.velocity.magnitude / maxSpeed);
        
        
        protected virtual float RotPower => (IsAccelerating ? 0.3f : 1.0f);

        #endregion

        #region Methods

        #region IActor

        public virtual void Target()
        {
            
        }

        public virtual void Untarget()
        {
            
        }

        #endregion

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

        protected virtual void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (IsRotating)
            {
                float __newRotation = ActorRotation - rotSpeed * RotPower * rotInput * Time.fixedDeltaTime;
                __newRotation = NormalizeAngle(__newRotation);
         
                Body.MoveRotation(angle: __newRotation);
            }
            
            if (IsAccelerating)
            {
                _currThrust = Mathf.Min(_currThrust + 2.5f * Time.fixedDeltaTime, _maxThrust);
                Vector2 __force = AimDir * ((acceleration * accelerationMultiplier) * _currThrust);

                Body.gravityScale = 0;
                Body.AddForce(force: __force, ForceMode2D.Force);
            }
            else
            {
                _currThrust = 0;
                //Body.gravityScale = 3;
            }
            
            //Clamp velocity
            float __clampedVelocity = Body.velocity.magnitude.Clamp(min: 0, max: maxSpeed);

            Body.velocity = VelDir * __clampedVelocity;
        }

        protected virtual void Fire()
        {
            //if (GameManager.IsGamePaused) return;

            if (_nextFire < Time.time )
            {
                Vector2 __bulletSpawnPos = ActorPosition + AimDir * 2.5f;
                
                Vector3 __bulletDirection = Quaternion.Euler(0, 0, z: UnityEngine.Random.Range(-5, 5)) * AimDir;

                float __angle = Mathf.Atan2(__bulletDirection.y, __bulletDirection.x) * Mathf.Rad2Deg;
                Quaternion __rotation = Quaternion.AngleAxis(__angle, Vector3.forward);
                
                Projectile __projectile = Instantiate(original: projectilePrefab, position: __bulletSpawnPos, rotation: __rotation);
                __projectile.GetComponent<Rigidbody2D>().velocity = __bulletDirection * Body.velocity.magnitude * 2; 

                //TODO: Muzzle and Sound.
                //MuzzleFlash();
                //playSound(0, 0.5f);

                _nextFire = (Time.time + fireDelay);
            }
        }
        
        protected static float NormalizeAngle(float angle)
        {
            angle = (angle + 180) % 360;
            if (angle < 0)
            {
                angle += 360;
            }
            return angle - 180;
        }

        #endregion
    }
}
