using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Game
{
    public sealed class Targeter : MonoBehaviour
    {
        private Ray2D Ray => new Ray2D(origin: transform.position, direction: transform.up);

        [SerializeField] private float maxDistance = 10;

        [SerializeField] private LayerMask layerMask = ~(1 << 14);

        [PublicAPI] 
        [field: SerializeField] 
        public bool LockTarget { get; set; } = false;

        [field: SerializeField]
        private IActor CurrentTarget_Internal { get; set; } = null;

        [PublicAPI]
        //[field: SerializeReference]
        [field: SerializeField]
        public IActor CurrentTarget
        {
            get => CurrentTarget_Internal;
            private set
            {
                CurrentTarget_Internal?.Untarget();
                //Set new
                CurrentTarget_Internal = value;
                
                CurrentTarget_Internal.Target();
            } 
        }

        public bool HasTarget => (CurrentTarget != null);// || !CurrentTarget.Equals(null);

        public Sprite TargetSprite => HasTarget ? CurrentTarget.transform.GetComponent<SpriteRenderer>().sprite : null;

        [PublicAPI]
        public bool TryGetNewTarget(out IActor newTarget)
        {
            RaycastHit2D __hit = Physics2D.Raycast(origin: Ray.origin, direction: Ray.direction, distance: maxDistance, layerMask: layerMask);
            bool __hasHitCollider = (__hit.collider != null);

            if (__hasHitCollider)
            {
                return (__hit.transform.TryGetComponent(out newTarget));
            }

            newTarget = null;
            return false;
        }

        private void Update()
        {
            if (HasTarget && LockTarget) return;
            
            if (TryGetNewTarget(newTarget: out IActor __newTarget))
            {
                CurrentTarget = __newTarget;
                
                Debug.Log("New Target!");
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(Ray.origin, Ray.direction * maxDistance, Color.cyan);
        }
    }
}
