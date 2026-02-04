// WiggleTriggerAction.cs : Description : Elastic wiggle with damping for scale and per-axis rotation

using UnityEngine;
using System.Collections;

namespace TriggerSystem
{
    [System.Flags]
    public enum WiggleMode
    {
        Scale = 1 << 0,
        Rotation = 1 << 1
    }

    public class WiggleTriggerAction : TriggerAction
    {
        [Header("Trigger")]
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool playOnExit = false;
        [SerializeField] private bool playOnStay = false;

        [Header("Wiggle Mode")]
        [SerializeField] private WiggleMode wiggleMode = WiggleMode.Scale | WiggleMode.Rotation;
        [SerializeField] private Vector3 scaleAmplitude = new Vector3(0.2f, 0.2f, 0.2f);
        [SerializeField] private Vector3 rotationAmplitude = new Vector3(0f, 0f, 15f);

        [Header("Spring")]
        [SerializeField] private float stiffness = 50f;
        [SerializeField] private float damping = 4f;
        [SerializeField] private float velocityKick = 5f;

        private Vector3 baseScale;
        private Vector3 baseRotation;
        private bool hasBaseValues;
        private Coroutine wiggleCoroutine;

        private void Awake()
        {
            if (!hasBaseValues)
            {
                baseScale = transform.localScale;
                baseRotation = transform.localEulerAngles;
                hasBaseValues = true;
            }
        }

        private Vector3 GetImpactDirection(CollisionContext collisionContext)
        {
            Vector3 worldDir = Vector3.down; // Default fallback

            if (collisionContext.CollisionData != null && collisionContext.CollisionData.contactCount > 0)
            {
                // Use contact normal (points away from this object)
                worldDir = -collisionContext.CollisionData.GetContact(0).normal;
            }
            else if (collisionContext.TriggeringObject != null)
            {
                // Fallback: direction from triggering object to this object
                worldDir = (transform.position - collisionContext.TriggeringObject.transform.position).normalized;
            }

            // Convert to local space
            return transform.InverseTransformDirection(worldDir).normalized;
        }

        public override void Execute(TriggerContext context)
        {
            if (context is not CollisionContext collisionContext) return;

            bool shouldPlay = collisionContext.Type switch
            {
                TriggerType.Enter => playOnEnter,
                TriggerType.Exit => playOnExit,
                TriggerType.Stay => playOnStay,
                _ => false
            };

            if (shouldPlay) StartWiggle(collisionContext);
        }

        private void StartWiggle(CollisionContext collisionContext)
        {
            if (wiggleCoroutine != null) StopCoroutine(wiggleCoroutine);
            wiggleCoroutine = StartCoroutine(WiggleCoroutine(collisionContext));
        }

        private IEnumerator WiggleCoroutine(CollisionContext collisionContext)
        {
            if (!hasBaseValues)
            {
                baseScale = transform.localScale;
                baseRotation = transform.localEulerAngles;
                hasBaseValues = true;
            }

            transform.localScale = baseScale;
            transform.localEulerAngles = baseRotation;

            Vector3 scaleDisp = Vector3.zero;
            Vector3 scaleVel = Vector3.zero;
            Vector3 rotDisp = Vector3.zero;
            Vector3 rotVel = Vector3.zero;

            // Get impact direction in local space
            Vector3 impactDir = GetImpactDirection(collisionContext);

            // Initial velocity kick based on impact direction
            if ((wiggleMode & WiggleMode.Scale) != 0)
            {
                // Scale: compress along impact direction, with magnitude based on alignment
                scaleVel = new Vector3(
                    scaleAmplitude.x != 0 ? -velocityKick * impactDir.x : 0,
                    scaleAmplitude.y != 0 ? -velocityKick * impactDir.y : 0,
                    scaleAmplitude.z != 0 ? -velocityKick * impactDir.z : 0
                );
            }
            if ((wiggleMode & WiggleMode.Rotation) != 0)
            {
                // Rotation: tilt away from impact direction
                // X rotation (pitch): from Z impacts, Z rotation (roll): from X impacts
                // Y rotation (yaw): from tangential component of horizontal impacts
                rotVel = new Vector3(
                    rotationAmplitude.x != 0 ? velocityKick * impactDir.z : 0,
                    rotationAmplitude.y != 0 ? velocityKick * (impactDir.x - impactDir.z) * 0.5f : 0,
                    rotationAmplitude.z != 0 ? -velocityKick * impactDir.x : 0
                );
            }

            const float threshold = 0.001f;

            while (scaleDisp.magnitude > threshold || rotDisp.magnitude > threshold || scaleVel.magnitude > threshold || rotVel.magnitude > threshold)
            {
                float dt = Time.deltaTime;

                if ((wiggleMode & WiggleMode.Scale) != 0)
                {
                    scaleVel += (-stiffness * scaleDisp - damping * scaleVel) * dt;
                    scaleDisp += scaleVel * dt;
                    transform.localScale = baseScale + Vector3.Scale(scaleDisp, scaleAmplitude);
                }

                if ((wiggleMode & WiggleMode.Rotation) != 0)
                {
                    rotVel += (-stiffness * rotDisp - damping * rotVel) * dt;
                    rotDisp += rotVel * dt;
                    transform.localEulerAngles = baseRotation + Vector3.Scale(rotDisp, rotationAmplitude);
                }

                yield return null;
            }

            transform.localScale = baseScale;
            transform.localEulerAngles = baseRotation;
            wiggleCoroutine = null;
        }
    }
}
