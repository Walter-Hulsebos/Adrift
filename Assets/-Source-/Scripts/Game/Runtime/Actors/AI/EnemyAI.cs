using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class EnemyAI : Actor
    {
        #region Fields

        [SerializeField] private float closeInRange;
        
        [SerializeField] private float attackRange;
        
        [SerializeField] private float thrustRange;
        
        [SerializeField] private float desiredGoalAngle = 30;

        #endregion

        #region Properties

        private static PlayerController Player => PlayerController.Instance;
        private static bool PlayerActive => PlayerController.InstanceExists;

        private static Rigidbody2D PlayerBody => Player.GetComponent<Rigidbody2D>();
        private static Vector2 PlayerPosition => PlayerBody.transform.position;

        private Vector2 VectorToPlayer => (PlayerPosition - this.ActorPosition);
        private Vector2 DirectionToPlayer => VectorToPlayer.normalized;

        private float DistanceToPlayer => VectorToPlayer.magnitude;
        private float DistanceToPlayerSqr => VectorToPlayer.sqrMagnitude;
        
        
        private float AttackRangeSqr => (attackRange * attackRange);
        private float ThrustRangeSqr => (thrustRange * thrustRange);
        private float CloseInRangeSqr => (closeInRange * closeInRange);

        #endregion

        private void Awake() => SetupEnemy();

        protected virtual void SetupEnemy()
        {
            maxSpeed = Random.Range(25f, 50f);
            rotSpeed = Random.Range(100, 200);
            
            fireDelay = Random.Range(0.5f, 1.25f);
            
            closeInRange = Random.Range(3, 10);
            thrustRange = Random.Range(20, 35);
        }
        
        protected virtual void Update()
        {
            if (PlayerActive)
            {
                Vector3 __dir = DirectionToPlayer;

                float __goalAng = Mathf.Atan2(__dir.y, __dir.x);
                __goalAng = (__goalAng / Mathf.PI) * 180;

                float __diff = NormalizeAngle(__goalAng - ActorRotation + 90);
                float __diffAbs = Mathf.Abs(__diff - 180);
                
                float __distSqr = DistanceToPlayerSqr;

                __Aiming();
                __Thrusting();
                __Shooting();
                
                void __Aiming()
                {
                    if (__diffAbs > desiredGoalAngle)
                    {
                        if (__diff > 0)
                        {
                            rotInput = 1;
                        }
                        else
                        {
                            rotInput = -1;
                        }
                    }
                    else
                    {
                        rotInput = 0;
                    }
                }
                void __Thrusting()
                {
                    if((__distSqr > ThrustRangeSqr && __diffAbs < 40 ))
                    {
                        thrustInput = 1;
                    }
                    else
                    {
                        thrustInput = (__distSqr < CloseInRangeSqr) ? 1 : 0;
                    }
                }
                void __Shooting()
                {
                    if (__diffAbs < 40 && __distSqr < AttackRangeSqr)
                    {
                        Fire();
                    }    
                }
            }
        }

    }
}
