// AccelerationTriggerAction.cs : Description : Accelerates the ball in the collider's forward direction

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
        [Tooltip("The force applied to accelerate the ball in the collider's forward direction")]
        public float accelerationForce = 10f;
        [Tooltip("The force mode used to apply the acceleration")]
        public ForceMode forceMode = ForceMode.VelocityChange;

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

            // Get the forward direction of the collider (this transform)
            Vector3 forwardDirection = transform.forward;

            //TODO: Apply force to the Rigidbody in the collider's forward direction
            // Use the AddForce method with the specified force mode
            // Don't forget to multiply the forward direction by the accelerationForce
        }
    }
}

