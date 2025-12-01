// AccelerationTriggerAction.cs : Description : Accelerates the ball in its velocity direction

using UnityEngine;

namespace TriggerSystem
{
    public class AccelerationTriggerAction : TriggerAction
    {
        [Header("Acceleration Settings")]
        [Tooltip("Accelerate the ball when it enters the trigger")]
        public bool accelerateOnEnter = true;
        [Tooltip("Accelerate the ball when it exits the trigger")]
        public bool accelerateOnExit = false;
        [Tooltip("Accelerate the ball continuously while it stays in the trigger")]
        public bool accelerateOnStay = false;
        
        [Header("Force Settings")]
        [Tooltip("The force applied to accelerate the ball in its velocity direction")]
        public float accelerationForce = 10f;
        [Tooltip("The force mode used to apply the acceleration")]
        public ForceMode forceMode = ForceMode.VelocityChange;
        
        [Header("Minimum Velocity")]
        [Tooltip("Minimum velocity magnitude required for acceleration to be applied. Prevents accelerating stationary balls.")]
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

