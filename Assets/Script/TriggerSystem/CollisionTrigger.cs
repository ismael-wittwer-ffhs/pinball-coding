// CollisionTrigger.cs : Description : Trigger that responds to collisions

using UnityEngine;

namespace TriggerSystem
{
    [RequireComponent(typeof(Collider))]
    public class CollisionTrigger : BaseTrigger
    {
        [Header("Collision Settings")]
        [SerializeField] private LayerMask collisionLayer = -1; // All layers by default
        [SerializeField] private bool requireTag = false;
        [SerializeField] private string requiredTag = "Ball";

        private Collider triggerCollider;

        protected override void Awake()
        {
            base.Awake();
            triggerCollider = GetComponent<Collider>();
            
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }

        protected override void Trigger(TriggerContext context)
        {
            InvokeTrigger(context);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ShouldProcessCollision(other))
            {
                var context = new CollisionContext(other.gameObject, other, TriggerType.Enter);
                Trigger(context);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ShouldProcessCollision(other))
            {
                var context = new CollisionContext(other.gameObject, other, TriggerType.Exit);
                Trigger(context);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (ShouldProcessCollision(other))
            {
                var context = new CollisionContext(other.gameObject, other, TriggerType.Stay);
                Trigger(context);
            }
        }

        private bool ShouldProcessCollision(Collider other)
        {
            // Check layer mask
            if (((1 << other.gameObject.layer) & collisionLayer) == 0)
            {
                return false;
            }

            // Check tag if required
            if (requireTag && !other.CompareTag(requiredTag))
            {
                return false;
            }

            return true;
        }
    }
}

