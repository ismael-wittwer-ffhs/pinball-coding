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

            if (shouldPlay) StartWiggle();
        }

        private void StartWiggle()
        {
            if (wiggleCoroutine != null) StopCoroutine(wiggleCoroutine);
            wiggleCoroutine = StartCoroutine(WiggleCoroutine());
        }

        private IEnumerator WiggleCoroutine()
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

            // Initial velocity kick (only on axes with non-zero amplitude)
            if ((wiggleMode & WiggleMode.Scale) != 0)
                scaleVel = new Vector3(scaleAmplitude.x != 0 ? velocityKick : 0, scaleAmplitude.y != 0 ? velocityKick : 0, scaleAmplitude.z != 0 ? velocityKick : 0);
            if ((wiggleMode & WiggleMode.Rotation) != 0)
                rotVel = new Vector3(rotationAmplitude.x != 0 ? velocityKick : 0, rotationAmplitude.y != 0 ? velocityKick : 0, rotationAmplitude.z != 0 ? velocityKick : 0);

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
