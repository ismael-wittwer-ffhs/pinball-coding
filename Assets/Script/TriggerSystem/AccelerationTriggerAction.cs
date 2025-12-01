// AccelerationTriggerAction.cs : Description : Accelerates the ball in its velocity direction

using UnityEngine;

namespace TriggerSystem
{
    public class AccelerationTriggerAction : TriggerAction
    {
        [Header("Acceleration Settings")]
        public bool accelerateOnEnter = true;
        public bool accelerateOnExit = false;
        public bool accelerateOnStay = false;
        
        [Header("Force Settings")]
        public float accelerationForce = 10f;
        public ForceMode forceMode = ForceMode.VelocityChange;
        
        [Header("Minimum Velocity")]
        public float minimumVelocity = 0.1f;

        public override void Execute(TriggerContext context)
        {
            if (context is CollisionContext collisionContext)
            {
                bool shouldAccelerate = false;

                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        shouldAccelerate = accelerateOnEnter;
                        break;
                    case TriggerType.Exit:
                        shouldAccelerate = accelerateOnExit;
                        break;
                    case TriggerType.Stay:
                        shouldAccelerate = accelerateOnStay;
                        break;
                }

                if (shouldAccelerate)
                {
                    AccelerateBall(context.TriggeringObject);
                }
            }
        }

        private void AccelerateBall(GameObject ballObject)
        {
            if (ballObject == null) return;

            Rigidbody rb = ballObject.GetComponent<Rigidbody>();
            if (rb == null || rb.isKinematic) return;

            Vector3 velocity = rb.linearVelocity;
            float velocityMagnitude = velocity.magnitude;

            // Only accelerate if the ball has a minimum velocity
            if (velocityMagnitude < minimumVelocity) return;

            // Get the direction of the velocity
            Vector3 velocityDirection = velocity.normalized;

            // Apply acceleration in the velocity direction
            rb.AddForce(velocityDirection * accelerationForce, forceMode);
        }
    }
}

