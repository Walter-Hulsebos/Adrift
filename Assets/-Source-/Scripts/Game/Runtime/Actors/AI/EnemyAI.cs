using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Game
{
    public class EnemyAI : Actor
    {
        #region Fields

        [SerializeField] private float closeInRange;
        
        [SerializeField] private float attackRange;
        
        [SerializeField] private float followRange;
        
        [SerializeField] private float desiredGoalAngle = 30;

        #endregion

        #region Properties

        private static PlayerController Player => PlayerController.Instance;
        //private static bool PlayerActive => PlayerController.InstanceExists;
        private static bool PlayerActive = true;
        
        private static Rigidbody2D PlayerBody => Player.GetComponent<Rigidbody2D>();
        //[field: SerializeField] private Rigidbody2D PlayerBody { get; set; }
        
        private Vector2 PlayerPosition => PlayerBody.position;

        private Vector2 VectorToPlayer => (PlayerPosition - this.ActorPosition);
        private Vector2 DirectionToPlayer => VectorToPlayer.normalized;

        private float DistanceToPlayer => VectorToPlayer.magnitude;
        private float DistanceToPlayerSqr => VectorToPlayer.sqrMagnitude;
        
        
        private float AttackRangeSqr => (attackRange * attackRange);
        private float FollowRangeSqr => (followRange * followRange);
        private float CloseInRangeSqr => (closeInRange * closeInRange);

        #endregion

        private void Awake() => SetupEnemy();

        protected virtual void SetupEnemy()
        {
            maxSpeed = Random.Range(25f, 50f);
            rotSpeed = Random.Range(100, 200);
            
            fireDelay = Random.Range(0.5f, 1.25f);
            
            closeInRange = Random.Range(3, 10);
            followRange = Random.Range(2000, 3500);
        }
        
        protected virtual void Update()
        {
            if (PlayerActive)
            {
                Vector3 __dir = DirectionToPlayer;

                float __goalAng = Mathf.Atan2(__dir.y, __dir.x) * Mathf.Rad2Deg;
                //__goalAng = (__goalAng / Mathf.PI) * 180;

                float __rotDiff = NormalizeAngle(__goalAng - ActorRotation + 90);
                float __rotDiffAbs = Mathf.Abs(__rotDiff - 180);

                bool aimingAtTarget = (__rotDiffAbs < desiredGoalAngle);
                
                float __distSqr = DistanceToPlayerSqr;

                bool __isAligned = (__rotDiffAbs < 40);
                
                bool __inFollowRange = (__distSqr < FollowRangeSqr);
                bool __inCloseInRange = (__distSqr < CloseInRangeSqr);
                bool __inAttackRange = (__distSqr < AttackRangeSqr);

                __Aiming();
                __Following();
                __Shooting();
                
                void __Aiming()
                {
                    if (!aimingAtTarget)
                    {
                        //Debug.Log("__Aiming A");
                        
                        if (__rotDiff > 0)
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
                        //Debug.Log("__Aiming B");
                        rotInput = 0;
                    }
                }
                void __Following()
                {
                    if (__isAligned && __inFollowRange) //Align up first, then follow.
                    {
                        thrustInput = 1;
                    }
                    else
                    {
                        thrustInput = 0;
                    }
                    
                    /*
                    if((__distSqr > FollowRangeSqr && __rotDiffAbs < 40))
                    {
                        thrustInput = 1;
                    }
                    else
                    {
                        thrustInput = inCloseInRange ? 0 : 1;
                    }
                    */
                }
                void __Shooting()
                {
                    if (__isAligned && __inAttackRange) //Align up first, then shoot.
                    {
                        Fire();
                    }    
                }
            }
        }

    }
}
